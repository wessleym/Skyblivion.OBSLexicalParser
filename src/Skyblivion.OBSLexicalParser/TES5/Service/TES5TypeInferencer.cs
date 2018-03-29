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
        private string[] otherScripts;
        private ESMAnalyzer esmAnalyzer;
        public TES5TypeInferencer(ESMAnalyzer ESMAnalyzer, string otherScriptsFolder)
        {
            this.esmAnalyzer = ESMAnalyzer;
            this.otherScriptsFolder = otherScriptsFolder;
            otherScripts = Directory.EnumerateFiles(this.otherScriptsFolder).Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
        }

        /*
        * Inference the type by analyzing the object call.
         * Please note: It is not able to analyze calls to another scripts, but those weren"t used in oblivion anyways
        */
        public void inferenceObjectByMethodCall(TES5ObjectCall objectCall, TES5MultipleScriptsScope multipleScriptsScope)
        {
            this.inferenceTypeOfCalledObject(objectCall, multipleScriptsScope);
            if (objectCall.getArguments() != null)
            {
                this.inferenceTypeOfMethodArguments(objectCall, multipleScriptsScope);
            }
        }

        private void inferenceTypeOfMethodArguments(TES5ObjectCall objectCall, TES5MultipleScriptsScope multipleScriptsScope)
        {
            /*
             * Inference the arguments
             */
            TES5ObjectCallArguments arguments = objectCall.getArguments();
            int argumentNumber = 0;
            ITES5Type calledOnType = objectCall.getAccessedObject().getType().getNativeType();
            foreach (ITES5Value argument in arguments.getArguments())
            {
                /*
                 * Get the argument type according to TES5Inheritance graph.
                 */
                ITES5Type argumentTargetType = TES5InheritanceGraphAnalyzer.findTypeByMethodParameter(calledOnType, objectCall.getFunctionName(), argumentNumber);
                if (argument.getType() == argumentTargetType)
                {
                    argumentNumber++;
                    continue; //Same type matched. We do not need to do anything :)
                }

                /*
                 * todo - maybe we should move getReferencesTo() to TES5Value and make all of the rest TES5Values just have null references as they do not reference anything? :)
                 */
                ITES5Referencer referencerArgument = argument as ITES5Referencer;
                if (referencerArgument != null && TES5InheritanceGraphAnalyzer.isExtending(argumentTargetType, argument.getType()))
                { //HACKY!
                    this.inferenceType(referencerArgument.getReferencesTo(), argumentTargetType, multipleScriptsScope);
                }
                else
                {
                    //So there"s one , one special case where we actually have to cast a var from one to another even though they are not ,,inheriting" from themselves, because they are primitives.
                    //Scenario: there"s an T_INT argument, and we feed it with a T_FLOAT variable reference. It won"t work :(
                    //We need to cast it on call level ( NOT inference it ) to make it work and not break other possible scenarios ( more specifically, when a float would be inferenced to int and there"s a
                    //float assigment somewhere in the code )
                    if (argumentTargetType == TES5BasicType.T_INT && argument.getType() == TES5BasicType.T_FLOAT)
                    {
                        TES5Reference referenceArgument = argument as TES5Reference;
                        if (referenceArgument != null)
                        { //HACKY! When we"ll clean up this interface, it will dissapear :)
                            referenceArgument.setManualCastTo(TES5BasicType.T_INT);
                        }
                    }
                }

                ++argumentNumber;
            }
        }

        private void inferenceTypeOfCalledObject(TES5ObjectCall objectCall, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES5Type inferencableType = objectCall.getAccessedObject().getType().getNativeType();
            /*
             * Check if we have something to inference inside the code, not some static class or method call return
             */
            if (objectCall.getAccessedObject().getReferencesTo() != null)
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

                if (this.inferenceType(objectCall.getAccessedObject().getReferencesTo(), inferenceType, multipleScriptsScope))
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
            TES5ScriptHeader scriptHeader = multipleScriptsScope.getScriptHeaderOfScript(type.value());
            variable.trackRemoteScript(scriptHeader);
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
            if (!TES5InheritanceGraphAnalyzer.isExtending(type, variable.getPropertyType().getNativeType()))
            {
                return false;
            }

            variable.setPropertyType(type);
            return true;
        }

        /*
             * @throws ConversionException
        */
        public ITES5Type resolveInferenceTypeByReferenceEdid(ITES5Variable variable)
        {
            string baseEDID = variable.getReferenceEdid();
            List<string> namesToTry = new List<string>() { baseEDID, baseEDID + "Script" };
            int baseEDIDLength = baseEDID.Length;
            if (baseEDID.Substring(baseEDIDLength - 3, 3).Equals("ref", StringComparison.OrdinalIgnoreCase))
            {
                string tryAsRef = baseEDID.Substring(0, baseEDIDLength - 3);
                namesToTry.AddRange(new string[] { tryAsRef, tryAsRef + "Script" });
            }

            namesToTry = namesToTry.Distinct().ToList();
            foreach (var nameToTry in namesToTry)
            {
                if (this.otherScripts.Contains(nameToTry.ToLower()))
                {
                    return TES5TypeFactory.memberByValue(nameToTry);
                }
            }

            //If it"s not found, we"re forced to scan the ESM to see, how to resolve the ref name to script type
            return this.esmAnalyzer.resolveScriptTypeByItsAttachedName(variable.getReferenceEdid());
        }

        /*
        * Inference the variable by its reference EDID
        * 
        * @throws ConversionException
        */
        public void inferenceVariableByReferenceEdid(ITES5Variable variable, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //Check if it was inferenced to custom type already
            if (!variable.getPropertyType().isNativePapyrusType())
            {
                return; //Do not even try to inference a type which is already non-native.
            }

            this.inferenceWithCustomType(variable, this.resolveInferenceTypeByReferenceEdid(variable), multipleScriptsScope);
        }

        public void inferenceObjectByAssignation(ITES5Referencer reference, ITES5Value value, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (reference.getReferencesTo() != null && !reference.getType().isPrimitive())
            {
                this.inferenceType(reference.getReferencesTo(), value.getType().getNativeType(), multipleScriptsScope);
            }
        }
    }
}