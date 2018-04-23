using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    class TES5TypeInferencer
    {
        private string otherScriptsFolder;
        private string[] otherScriptsLower;
        private ESMAnalyzer esmAnalyzer;
        public TES5TypeInferencer(ESMAnalyzer ESMAnalyzer, string otherScriptsFolder)
        {
            this.esmAnalyzer = ESMAnalyzer;
            this.otherScriptsFolder = otherScriptsFolder;
            otherScriptsLower = Directory.EnumerateFiles(this.otherScriptsFolder).Select(path => Path.GetFileNameWithoutExtension(path).ToLower()).ToArray();
        }

        /*
        * Inference the type by analyzing the object call.
         * Please note: It is not able to analyze calls to another scripts, but those weren"t used in oblivion anyways
        */
        public void inferenceObjectByMethodCall(TES5ObjectCall objectCall, TES5MultipleScriptsScope multipleScriptsScope)
        {
            this.inferenceTypeOfCalledObject(objectCall, multipleScriptsScope);
            if (objectCall.Arguments!= null)
            {
                this.inferenceTypeOfMethodArguments(objectCall, multipleScriptsScope);
            }
        }

        private void inferenceTypeOfMethodArguments(TES5ObjectCall objectCall, TES5MultipleScriptsScope multipleScriptsScope)
        {
            /*
             * Inference the arguments
             */
            int argumentNumber = 0;
            ITES5Type calledOnType = objectCall.AccessedObject.TES5Type.NativeType;
            foreach (ITES5Value argument in objectCall.Arguments)
            {
                /*
                 * Get the argument type according to TES5Inheritance graph.
                 */
                ITES5Type argumentTargetType = TES5InheritanceGraphAnalyzer.findTypeByMethodParameter(calledOnType, objectCall.FunctionName, argumentNumber);
                if (argument.TES5Type == argumentTargetType)
                {
                    argumentNumber++;
                    continue; //Same type matched. We do not need to do anything :)
                }

                /*
                 * todo - maybe we should move getReferencesTo() to TES5Value and make all of the rest TES5Values just have null references as they do not reference anything? :)
                 */
                ITES5Referencer referencerArgument = argument as ITES5Referencer;
                if (referencerArgument != null && TES5InheritanceGraphAnalyzer.isExtending(argumentTargetType, argument.TES5Type.NativeType))
                { //HACKY!
                    this.inferenceType(referencerArgument.ReferencesTo, argumentTargetType, multipleScriptsScope);
                }
                else
                {
                    //So there"s one , one special case where we actually have to cast a var from one to another even though they are not ,,inheriting" from themselves, because they are primitives.
                    //Scenario: there"s an T_INT argument, and we feed it with a T_FLOAT variable reference. It won"t work :(
                    //We need to cast it on call level ( NOT inference it ) to make it work and not break other possible scenarios ( more specifically, when a float would be inferenced to int and there"s a
                    //float assigment somewhere in the code )
                    if (argumentTargetType == TES5BasicType.T_INT && argument.TES5Type == TES5BasicType.T_FLOAT)
                    {
                        TES5Castable argumentCastable = argument as TES5Castable;
                        if (argumentCastable != null)
                        { //HACKY! When we"ll clean up this interface, it will dissapear :)
                            argumentCastable.ManualCastTo = argumentTargetType;
                        }
                    }
                    //WTM:  Note:  I added the below as a safeguard, but it threw exceptions a lot.  So I'm removing it for now.  Maybe someday I'll find the right criteria to bring it back.
                    /*else if (!TES5InheritanceGraphAnalyzer.isExtending(argument.getType(), argumentTargetType))
                    {
                        throw new InvalidOperationException("Argument type mismatch.  Expected " + argumentTargetType.getOriginalName() + ".  Found " + argument.getType().getOriginalName() + ".");
                    }*/
                }

                argumentNumber++;
            }
        }

        private void inferenceTypeOfCalledObject(TES5ObjectCall objectCall, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5Type inferencableType = objectCall.AccessedObject.TES5Type.NativeType;
            /*
             * Check if we have something to inference inside the code, not some static class or method call return
             */
            if (objectCall.AccessedObject.ReferencesTo != null)
            {
                //this is not "exactly" nice solution, but its enough. For now.
                ITES5Type inferenceType = TES5InheritanceGraphAnalyzer.findTypeByMethod(objectCall);
                if (inferencableType == null)
                {
                    throw new ConversionException("Cannot inference a null type");
                }

                if (inferencableType == inferenceType)
                {
                    return; //We already have the good type.
                }

                if (this.inferenceType(objectCall.AccessedObject.ReferencesTo, inferenceType, multipleScriptsScope))
                {
                    return;
                }
            }
        }

        private void inferenceWithCustomType(ITES5Variable variable, ITES5Type type, TES5MultipleScriptsScope multipleScriptsScope)
        {
            /*
             * We"re referencing another script - find the script and make it a variable that property will track remotely
             */
            TES5ScriptHeader scriptHeader = multipleScriptsScope.getScriptHeaderOfScript(type.OriginalName);
            variable.TrackRemoteScript(scriptHeader);
        }

        /*
        * Try to inference variable"s type with type.
         * 
         * 
         *  Needed for proxifying the properties to other scripts
         *  - Will return true if inferencing succeeded, false otherwise.
         * @throws ConversionException
        */
        private bool inferenceType(ITES5Variable variable, ITES5Type type, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (!TES5InheritanceGraphAnalyzer.isExtending(type, variable.PropertyType.NativeType))
            {
                return false;
            }

            variable.PropertyType = type;
            return true;
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type resolveInferenceTypeByReferenceEdid(ITES5Variable variable)
        {
            string edid = variable.ReferenceEDID;
            //WTM:  Change:  Without this if statement, SEBrithaurRef finds a reference
            //from qf_se35_01044c44_40_0 to SEBrithaurScript instead of
            //from qf_se35_01044c44_40_0 to SE35BrithaurScript
            //So the content of this if statement must be skipped.
            if (edid != "SEBrithaurRef")
            {
                //WTM:  Change:  I added the "FurnScript" (for qf_housebravil_01085480_10_0 and other house quests) and "QuestScript" (for MQDragonArmorQuestScript) items.
                List<string> namesToTry = new List<string>() { edid, edid + "QuestScript", edid + "FurnScript", edid + "Script" };
                int edidLength = edid.Length;
                if (edid.Substring(edidLength - 3, 3).Equals("ref", StringComparison.OrdinalIgnoreCase))
                {
                    string tryAsRef = edid.Substring(0, edidLength - 3);
                    namesToTry.AddRange(new string[] { tryAsRef, tryAsRef + "Script" });
                }
                namesToTry = namesToTry.Distinct().ToList();
                string firstNameMatch = namesToTry.Where(n => this.otherScriptsLower.Contains(n.ToLower())).FirstOrDefault();
                if (firstNameMatch != null) { return TES5TypeFactory.memberByValue(firstNameMatch); }
            }

            //If it"s not found, we"re forced to scan the ESM to see, how to resolve the ref name to script type
            return this.esmAnalyzer.resolveScriptTypeByItsAttachedName(variable.ReferenceEDID);
        }

        /*
        * Inference the variable by its reference EDID
        * 
        * @throws ConversionException
        */
        public void inferenceVariableByReferenceEdid(ITES5Variable variable, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //Check if it was inferenced to custom type already
            if (!variable.PropertyType.IsNativePapyrusType)
            {
                return; //Do not even try to inference a type which is already non-native.
            }

            this.inferenceWithCustomType(variable, this.resolveInferenceTypeByReferenceEdid(variable), multipleScriptsScope);
        }

        public void inferenceObjectByAssignation(ITES5Referencer reference, ITES5Value value, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (reference.ReferencesTo != null && !reference.TES5Type.IsPrimitive)
            {
                this.inferenceType(reference.ReferencesTo, value.TES5Type.NativeType, multipleScriptsScope);
            }
        }
    }
}