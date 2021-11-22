using Skyblivion.ESReader.Extensions;
using Skyblivion.ESReader.TES4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    static class StageMapFromESMBuilder
    {
        public static string BuildString(TES4Record qust)
        {
            const string indx = "INDX", qsta = "QSTA", ctda = "CTDA";
            var subrecords = qust.GetSubrecords(new string[] { indx, qsta, ctda });
            Nullable<int> targetIndex = null;
            int maxTargetIndex = 0;
            List<int> stageIndexes = new List<int>();
            Dictionary<int, List<Tuple<Func<int, bool>, bool>>> targetFuncs = new Dictionary<int, List<Tuple<Func<int, bool>, bool>>>();
            foreach (var subrecord in subrecords)
            {
                if (subrecord.Key == indx)
                {
                    int stageIndex = (int)subrecord.Value.FirstByte();
                    stageIndexes.Add(stageIndex);
                    targetIndex = null;
                }
                else if (subrecord.Key == qsta)
                {
                    targetIndex = targetIndex == null ? 0 : targetIndex.Value + 1;
                    if (targetIndex.Value > maxTargetIndex) { maxTargetIndex = targetIndex.Value; }
                }
                else if (subrecord.Key == ctda)
                {
                    if (targetIndex == null) { continue; }
                    int targetIndexInt = targetIndex.Value;
                    IReadOnlyList<byte> bytes = subrecord.Value.Bytes;
                    int functionIndex = BitConverter.ToInt32(bytes.Skip(8).Take(4).ToArray(), 0);
                    bool getStage = functionIndex == 58, getStageDone = functionIndex == 59;
                    if (getStage)
                    {
                        bool anyRemainingNonZero = bytes.Skip(16).Where(b => b != (byte)0).Any();
                        if (anyRemainingNonZero) { throw new InvalidOperationException("anyRemainingNonZero"); }
                        int formID1 = BitConverter.ToInt32(bytes.Skip(12).Take(4).ToArray(), 0);
                        //int formID2 = BitConverter.ToInt32(bytes.Skip(16).Take(4).ToArray(), 0);
                        if (qust.FormID != formID1) { continue; }
                        byte firstByte = bytes[0];
                        int compareOperator = firstByte & 0b11110000;
                        int flags = firstByte & 0b00001111;
                        bool equalTo = compareOperator == 0, notEqualTo = compareOperator == 0b00100000, lessThan = compareOperator == 0b10000000, greaterThan = compareOperator == 0b01000000, greaterThanOrEqualTo = compareOperator == 0b01100000, lessThanOrEqualTo = compareOperator == 0b10100000;
                        if (!(equalTo || notEqualTo || lessThan || greaterThan || greaterThanOrEqualTo || lessThanOrEqualTo)) { throw new InvalidOperationException("Invalid compareOperator"); }
                        bool flagsNone = flags == 0, flagsOr = flags == 1;
                        if (!(flagsNone || flagsOr)) { throw new InvalidOperationException("Invalid flags"); }
                        var currentList = targetFuncs.GetOrAdd(targetIndexInt, () => new List<Tuple<Func<int, bool>, bool>>(new List<Tuple<Func<int, bool>, bool>>()));
                        Func<int, bool> newFunc;
                        float comparisonValue = BitConverter.ToSingle(bytes.Skip(4).Take(4).ToArray(), 0);//stage
                        if (equalTo) { newFunc = stageID => stageID == comparisonValue; }
                        else if (notEqualTo) { newFunc = stageID => stageID != comparisonValue; }
                        else if (lessThan) { newFunc = stageID => stageID < comparisonValue; }
                        else if (lessThanOrEqualTo) { newFunc = stageID => stageID <= comparisonValue; }
                        else if (greaterThan) { newFunc = stageID => stageID > comparisonValue; }
                        else if (greaterThanOrEqualTo) { newFunc = stageID => stageID >= comparisonValue; }
                        else { throw new InvalidOperationException(); }
                        currentList.Add(new Tuple<Func<int, bool>, bool>(newFunc, flagsOr));
                    }
                    else if (getStageDone)
                    {
                        bool anyRemainingNonZero = bytes.Skip(20).Where(b => b != (byte)0).Any();
                        if (anyRemainingNonZero) { throw new InvalidOperationException("anyRemainingNonZero"); }
                        int formID1 = BitConverter.ToInt32(bytes.Skip(12).Take(4).ToArray(), 0);
                        int formID2 = BitConverter.ToInt32(bytes.Skip(16).Take(4).ToArray(), 0);
                        if (qust.FormID != formID1) { continue; }
                        byte firstByte = bytes[0];
                        int compareOperator = firstByte & 0b11110000;
                        int flags = firstByte & 0b00001111;
                        if (!(compareOperator == 0)) { throw new InvalidOperationException("Invalid compareOperator"); }
                        if (!(flags == 0/*none*/ || flags == 1/*or*/)) { throw new InvalidOperationException("Invalid flags"); }
                        float comparisonValue = BitConverter.ToSingle(bytes.Skip(4).Take(4).ToArray(), 0);//is "done"
                        /*if (comparisonValue == 1) { targetFuncs.GetOrAddNewIfNotContainsKey(targetIndexInt).Add(stageID => stageID > comparisonValue); }
                        else if (comparisonValue == 0) { targetFuncs.GetOrAddNewIfNotContainsKey(targetIndexInt).Add(stageID => stageID <= comparisonValue); }*/
                    }
                }
            }
            var simplifiedTargetFuncs = targetFuncs.ToDictionary(kvp => kvp.Key, kvp =>
            {
                List<Tuple<Func<int, bool>, bool>> currentList = kvp.Value;
                List<Func<int, bool>> reducedList = new List<Func<int, bool>>();
                for (int j = 0; j < currentList.Count; j++)
                {
                    var currentTuple = currentList[j];
                    if (currentTuple.Item2 || j + 1 >= currentList.Count) { reducedList.Add(currentTuple.Item1); }
                    else
                    {
                        var nextTuple = currentList[j + 1];
                        Func<int, bool> newFunc = si => currentTuple.Item1(si) && nextTuple.Item1(si);
                        bool or = nextTuple.Item2;
                        currentList[j] = Tuple.Create(newFunc, or);
                        currentList.RemoveAt(j + 1);
                        j--;
                    }
                }
                return currentList.Select(x => x.Item1).ToArray();
            });
            Dictionary<int, Dictionary<int, bool>> stageDictionary = stageIndexes.ToDictionary(stageIndex => stageIndex, stageIndex =>
            {
                Dictionary<int, bool> innerDictionary = new Dictionary<int, bool>();
                for (int i = 0; i <= maxTargetIndex; i++)
                {
                    Func<int, bool>[] currentList;
                    bool result;
                    if (!simplifiedTargetFuncs.TryGetValue(i, out currentList)) { result = true; }
                    else
                    {
                        result = currentList.Any(x => x(stageIndex));
                    }
                    innerDictionary[i] = result;
                }
                return innerDictionary;
            });
            StringBuilder contents = new StringBuilder();
            bool anyTargets = targetIndex != null;
            foreach (var stage in stageDictionary.OrderBy(s => s.Key))
            {
                var rowDictionary = stage.Value;
                contents.Append(stage.Key).Append(" - ");
                if (anyTargets)
                {
                    for (int i = 0; i <= maxTargetIndex; i++)
                    {
                        bool cell;
                        contents.Append(rowDictionary.TryGetValue(i, out cell) && cell ? "1" : "0").Append(" ");
                    }
                }
                contents.AppendLine();
            }
            return contents.ToString();
        }

        public static StageMap BuildStageMap(TES4Record qust)
        {
            string contentsString = BuildString(qust);
            var stageMapDictionary = StageMapDictionaryBuilder.Build(contentsString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            return new StageMap(stageMapDictionary, false);
        }
    }
}
