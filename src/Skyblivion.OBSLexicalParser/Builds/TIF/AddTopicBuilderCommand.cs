using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    static class AddTopicBuilderCommand
    {
        public static void GenerateINFOAddTopicScripts(ESMAnalyzer esmAnalyzer, BuildTracker buildTracker, IBuildTarget tifBuildTarget)
        {
            TES5TypeInferencer typeInferencer = new TES5TypeInferencer(esmAnalyzer);
            TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
            TES4TopicsToTES5GlobalVariableFinder globalVariableFinder = new TES4TopicsToTES5GlobalVariableFinder();
            TES5GlobalVariables globalVariables = esmAnalyzer.GlobalVariables;
            var builtTIFs = buildTracker.GetBuiltScripts(BuildTargetFactory.TIFName);
            foreach (TES4LoadedRecord infoRecord in esmAnalyzer.GetRecords().Where(r => r.RecordType == TES4RecordType.INFO))
            {
                byte[][] names = infoRecord.GetSubrecords("NAME").ToArray();
                if (names.Any())
                {
                    string fragment0Name = TES5FragmentFactory.GetFragmentName(0);
                    string nameTES5FormIDHex = (infoRecord.FormID + 0x01000000).ToString("x2").PadLeft(8, '0');
                    string scriptName = TES5ReferenceFactory.tif_Prefix + "_" + nameTES5FormIDHex;
                    TES5Script? infoTIF = builtTIFs.Where(s => s.Key == scriptName).Select(s => s.Value.Script).FirstOrDefault();
                    TES5FunctionCodeBlock? fragment0;
                    if (infoTIF != null)
                    {
                        fragment0 = infoTIF.BlockList.Blocks.OfType<TES5FunctionCodeBlock>().Where(b => b.BlockName == fragment0Name).First();
                    }
                    else
                    {
                        TES5ScriptHeader scriptHeader = TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(scriptName, TES5BasicType.T_TOPICINFO, TES5TypeFactory.TES4_Prefix, true);
                        TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
                        TES5FunctionScope functionScope = new TES5FunctionScope(fragment0Name);
                        functionScope.AddParameter(new TES5SignatureParameter("akSpeakerRef", TES5BasicType.T_OBJECTREFERENCE, true));
                        TES5LocalScope localScope = new TES5LocalScope(functionScope);
                        TES5CodeScope codeScope = TES5CodeScopeFactory.CreateCodeScope(localScope);
                        fragment0 = new TES5FunctionCodeBlock(functionScope, codeScope, TES5VoidType.Instance, false, true);
                        TES5BlockList blockList = new TES5BlockList() { fragment0 };
                        infoTIF = new TES5Script(globalScope, blockList, true);
                        string outputPath = tifBuildTarget.GetTranspileToPath(scriptName);
                        TES5Target target = new TES5Target(infoTIF, outputPath);
                        buildTracker.RegisterBuiltScript(tifBuildTarget, target);
                    }
                    foreach (byte[] name in names)
                    {
                        int formID = infoRecord.ExpandBytesIntoFormID(name);
                        TES4LoadedRecord addedTopic = esmAnalyzer.GetRecordByFormID(formID);
                        Tuple<int, string>? globalVariable = globalVariableFinder.GetGlobalVariableNullable(addedTopic.FormID);
                        string globalVariableEditorID = globalVariable != null ? globalVariable.Item2 : globalVariableFinder.GetGlobalVariableEditorID(addedTopic.GetEditorID());
                        Nullable<int> globalVariableTES5FormID = globalVariable != null ? globalVariable.Item1 : (Nullable<int>)null;
                        TES5Property topicAddedProperty = TES5PropertyFactory.ConstructWithTES5FormID(globalVariableEditorID, TES5BasicType.T_GLOBALVARIABLE, globalVariableEditorID, globalVariableTES5FormID);
                        infoTIF.GlobalScope.AddPropertyIfNotExists(topicAddedProperty);
                        TES5Reference topicAddedReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(topicAddedProperty);
                        fragment0.AddChunk(objectCallFactory.CreateObjectCall(topicAddedReference, "SetValueInt", new TES5ObjectCallArguments() { new TES5Integer(1) }));
                    }
                }
            }
        }
    }
}
