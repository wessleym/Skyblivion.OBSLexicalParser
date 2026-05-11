using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    class TES5TypeInferencer
    {
        private readonly ESMAnalyzer esmAnalyzer;
        //private readonly string otherScriptsFolder;
        //private readonly string[] otherScriptsLower;
        public TES5TypeInferencer(ESMAnalyzer esmAnalyzer/*, string otherScriptsFolder*/)
        {
            this.esmAnalyzer = esmAnalyzer;
            //this.otherScriptsFolder = otherScriptsFolder;
            //otherScriptsLower = Directory.EnumerateFiles(this.otherScriptsFolder).Select(path => Path.GetFileNameWithoutExtension(path).ToLower()).ToArray();
        }

        private static void InferenceTypeOfCalledObject(TES5ObjectCall objectCall, TES5InheritanceFunctionWithCalledOn function)
        {
            /*
             * Check if we have something to inference inside the code, not some static class or method call return
             */
            if (objectCall.AccessedObject.ReferencesTo == null) { return; }
            ITES5Type inferencableType = objectCall.AccessedObject.TES5Type.NativeType;
            //this is not "exactly" nice solution, but its enough. For now.
            TES5BasicType inferenceType = function.CalledOn;
            if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(inferencableType, inferenceType))
            {
                return; //We already have the good type.
            }
            InferenceType(objectCall.AccessedObject.ReferencesTo, inferenceType);
        }

        private static void InferenceTypeOfMethodArgument(TES5ObjectCall objectCall, ITES5Value argument, int argumentIndex, TES5InheritanceFunctionSignature function)
        {
            if (argumentIndex > function.Arguments.Length - 1)
            {
                throw new InvalidOperationException(function.Name + " does not have an argument at index " + argumentIndex);
            }
            TES5BasicType argumentTargetType = function.Arguments[argumentIndex];
            if (argument.TES5Type != argumentTargetType)
            {
                /*
                 * todo - maybe we should move getReferencesTo() to TES5Value and make all of the rest TES5Values just have null references as they do not reference anything? :)
                 */
                ITES5Referencer? referencerArgument = argument as ITES5Referencer;
                if (referencerArgument != null && TES5InheritanceGraphAnalyzer.IsExtending(argumentTargetType, argument.TES5Type.NativeType))
                { //HACKY!
                    if (referencerArgument.ReferencesTo == null) { throw new NullableException(nameof(referencerArgument.ReferencesTo)); }
                    InferenceType(referencerArgument.ReferencesTo, argumentTargetType);
                }
                else
                {
                    //So there's one , one special case where we actually have to cast a var from one to another even though they are not "inheriting" from themselves, because they are primitives.
                    //Scenario: there's an T_INT argument, and we feed it with a T_FLOAT variable reference. It won't work :(
                    //We need to cast it on call level ( NOT inference it ) to make it work and not break other possible scenarios ( more specifically, when a float would be inferenced to int and there's a
                    //float assigment somewhere in the code )
                    if (argumentTargetType == TES5BasicType.T_INT && argument.TES5Type == TES5BasicType.T_FLOAT)
                    {
                        TES5Castable? argumentCastable = argument as TES5Castable;
                        if (argumentCastable != null)
                        { //HACKY! When we clean up this interface, it will dissapear :)
                            argumentCastable.ManualCastTo = argumentTargetType;
                        }
                    }
                    else if (
                        !TES5InheritanceGraphAnalyzer.IsExtending(argument.TES5Type, argumentTargetType) &&
                        !TES5InheritanceGraphAnalyzer.IsNumberTypeOrBoolAndInt(argument.TES5Type, argumentTargetType) &&
                        !(argument is TES5None && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(argumentTargetType, TES5None.TES5TypeStatic)))
                    {
                        throw new ConversionTypeMismatchException("Argument type mismatch at " + objectCall.FunctionName + " index " + argumentIndex + ".  Expected " + argumentTargetType.Name + ".  Found " + argument.TES5Type.OriginalName + " : " + argument.TES5Type.NativeType.OriginalName + ".");
                    }
                }
            }
        }

        private static void InferenceTypeOfMethodArguments(TES5ObjectCall objectCall, TES5InheritanceFunctionSignature function)
        {
            /*
             * Inference the arguments
             */
            int argumentIndex = 0;
            foreach (ITES5Value argument in objectCall.Arguments)
            {
                InferenceTypeOfMethodArgument(objectCall, argument, argumentIndex, function);
                argumentIndex++;
            }
        }

        /*
        * Inference the type by analyzing the object call.
         * Please note: It is not able to analyze calls to another scripts, but those weren't used in oblivion anyways
        */
        public void InferenceObjectByMethodCall(TES5ObjectCall objectCall)
        {
            TES5InheritanceFunctionWithCalledOn function = TES5InheritanceGraphAnalyzer.FindFunction(objectCall, esmAnalyzer);
            InferenceTypeOfCalledObject(objectCall, function);
            InferenceTypeOfMethodArguments(objectCall, function);
            if (!objectCall.FunctionName.Equals(function.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The found function name (" + function.Name + ") did not match the original function name (" + objectCall.FunctionName + ").");
            }
            objectCall.FunctionName = function.Name;
        }

        private static void InferenceWithCustomType(ITES5VariableOrProperty variable, ITES5Type type, TES5MultipleScriptsScope multipleScriptsScope)
        {
            /*
             * We're referencing another script - find the script and make it a variable that property will track remotely
             */
            TES5ScriptHeader scriptHeader = multipleScriptsScope.GetScriptHeaderOfScript(type.OriginalName);
            variable.TrackRemoteScript(scriptHeader);
        }

        /*
        * Try to inference variable's type with type.
         * 
         * 
         *  Needed for proxifying the properties to other scripts
         *  - Will return true if inferencing succeeded, false otherwise.
         * @throws ConversionTypeMismatchException
        */
        private static void InferenceType(ITES5VariableOrProperty variable, TES5BasicType type)
        {
            if (!TES5InheritanceGraphAnalyzer.IsTypeOrExtendsTypeOrIsNumberType(type, variable.TES5Type.NativeType, false))
            {
                if (TES5InheritanceGraphAnalyzer.IsExtending(variable.TES5Type.NativeType, type))
                {
                    return;
                }
                throw new ConversionTypeMismatchException("Could not extend " + variable.TES5Type.NativeType.Value + " to " + type.Value + ".");
            }
            if (variable.TES5Type.AllowInference) { variable.TES5Type = type; }
            else if (variable.TES5Type.AllowNativeTypeInference) { variable.TES5Type.NativeType = type; }
            else { throw new ConversionTypeMismatchException(variable.Name + " (" + variable.TES5DeclaredType.OriginalName + " : " + variable.TES5DeclaredType.NativeType.Name + ") could not be inferenced to a " + type.Name + " because inference was not allowed."); }
        }

        /*
             * @throws ConversionTypeMismatchException
        */
        public ITES5Type GetScriptTypeByReferenceEdid(ITES5VariableOrProperty variable)
        {
            string? edid = variable.ReferenceEDID;
            if (edid == null) { throw new NullableException(nameof(edid)); }
            //WTM:  Change:  The below section seemed to only introduce issues.
            /*
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
                if (firstNameMatch != null) { return TES5TypeFactory.MemberByValue(firstNameMatch, null, esmAnalyzer); }
            }
            */
            //If it's not found, we're forced to scan the ESM to see, how to resolve the ref name to script type
            return this.esmAnalyzer.GetScriptTypeByEDID(edid);
        }

        /*
        * Inference the variable by its reference EDID
        * 
        * @throws ConversionTypeMismatchException
        */
        public void InferenceVariableByReferenceEdid(ITES5VariableOrProperty variable, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (variable.TES5Type.AllowInference)
            {
                ITES5Type type = this.GetScriptTypeByReferenceEdid(variable);
                InferenceWithCustomType(variable, type, multipleScriptsScope);
            }
        }

        public static void InferenceObjectByAssignation(ITES5Referencer reference, ITES5Value value)
        {
            if (reference.ReferencesTo != null && !reference.TES5Type.IsPrimitive)
            {
                InferenceType(reference.ReferencesTo, value.TES5Type.NativeType);
            }
        }
    }
}