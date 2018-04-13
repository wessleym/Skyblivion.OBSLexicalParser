using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    /*
     * Class StageMap
     * Represents the stage map. Will map original target indexes to mapped target indexes and duplicate them as needed
     * Target duplication algorithm assumes that stages go only forward, i.e., that the quest progression is stage by
     * stage.
     */
    class StageMap
    {
        private OrderableDictionary<int, List<int>> stageMap =  new OrderableDictionary<int, List<int>>();
        public Dictionary<int, List<int>> MappedTargetsIndex { get; private set; } = new Dictionary<int, List<int>>();
        public StageMap(IDictionary<int, List<int>> stageMapUnordered)
        {
            stageMap = new OrderableDictionary<int, List<int>>(stageMapUnordered);
            /*
             * Sort them by stage ids
             */
            stageMap.OrderBy(s => s.Key);
            MappedTargetsIndex = new Dictionary<int, List<int>>();
            /*
             * Do some validation
             */
            OrderableDictionary<int, List<int>> resultStageMap;
            if (stageMap.Any())
            {
                Nullable<int> length = null;
                foreach (var kvp in stageMap)
                {
                    var stageId = kvp.Key;
                    var rows = kvp.Value;
                    if (length == null)
                    {
                        length = rows.Count;
                    }
                    else if (length.Value != rows.Count)
                    {
                        throw new ConversionException("Invalid stage map metadata - stageID " + stageId + " expected " + length.Value + " rows but had " + rows.Count);
                    }
                }

                int nextFreeIndex = length.Value; //Next free index equals length, as per all 0-N arrays
                resultStageMap = new OrderableDictionary<int, List<int>>(stageMap.ToDictionary(m => m.Key, m => m.Value.ToList()));//Copy dictionary
                resultStageMap.OrderBy(s => s.Key);
                Dictionary<int, bool> targetsStateMap = new Dictionary<int, bool>();
                /*
                 * Traverse through the map, and duplicate targets as needed
                 */
                foreach (var kvp in stageMap)
                {
                    var stageId = kvp.Key;
                    var rows = kvp.Value;
                    int targetIndex = 0;
                    foreach (var row in rows)
                    {
                        if (row != 0)
                        {
                            bool targetState;
                            if (targetsStateMap.TryGetValue(targetIndex, out targetState))
                            {
                                if (!targetState)
                                {
                                    //We're changing the state from not used to used, in which case we need to do the duplication
                                    MappedTargetsIndex.AddNewListIfNotContainsKeyAndAddValueToList(targetIndex, nextFreeIndex);
                                    int fillInValue = 0;
                                    foreach (var kvp2 in resultStageMap)
                                    {
                                        var resultStageId = kvp2.Key;
                                        var resultRows = kvp2.Value;
                                        //This stage id marks the start of the block, so we switch the fillin value
                                        if (resultStageId == stageId)
                                        {
                                            fillInValue = 1;
                                        }

                                        //Check if this stage id result is 0, if so - switch out the block
                                        if (stageMap[resultStageId][targetIndex] == 0)
                                        {
                                            fillInValue = 0;
                                        }

                                        resultRows.Add(fillInValue);
                                        /*
                                         * If we already started filling out, remove original target as it wont be used anymore
                                         */
                                        if (resultStageId >= stageId)
                                        {
                                            resultRows[targetIndex] = 0;
                                        }
                                    }

                                    ++nextFreeIndex; //Increase the index
                                }
                            }

                            //We mark that the target is now used, perhaps was already used in which case this operation
                            //is doing nothing
                            targetsStateMap[targetIndex] = true;
                        }
                        else
                        {
                            //We mark that the target state changed to false ( perhaps it was already false, in which case
                            //this operation is doing nothing
                            //If the target was not used before, then we just continue, as it"s still not used.
                            targetsStateMap.SetIfContainsKey(targetIndex, false);
                        }

                        ++targetIndex;
                    }
                }
            }
            else
            {
                resultStageMap = stageMap;
            }
            this.stageMap = resultStageMap;
        }

        public IEnumerable<int> StageIDs => stageMap.Select(s => s.Key);

        public List<int> GetStageTargetsMap(int stageId)
        {
            return this.stageMap[stageId];
        }
    }
}