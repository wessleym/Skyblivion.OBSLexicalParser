using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5Property : ITES5VariableOrProperty
    {
        private const string PROPERTY_SUFFIX = "_p";
        public string OriginalName { get; }
        public bool AllowNameTransformation { get; }
        public string Name { get; private set; }
        private ITES5Type propertyType; //If we"re tracking a script, this won"t be used anymore
        private readonly ITES5Type originalPropertyType;
        public string? ReferenceEDID { get; }
        public bool IsPlayerRef { get; }
        private TES5ScriptHeader? trackedScript;
        public Nullable<int> TES4FormID { get; }
        private readonly Nullable<int> tes5FormID;

        public TES5Property(string name, ITES5Type propertyType, string? referenceEDID, Nullable<int> tes4FormID, Nullable<int> tes5FormID)
        {
            this.IsPlayerRef = name == TES5PlayerReference.PlayerRefName && propertyType == TES5PlayerReference.TES5TypeStatic && referenceEDID == TES5PlayerReference.PlayerRefName;
            this.OriginalName = name;
            this.AllowNameTransformation = !this.IsPlayerRef;
            this.Name = AllowNameTransformation ? AddPropertyNameSuffix(PapyrusCompiler.FixReferenceName(name)) : name;
            this.propertyType = propertyType;
            originalPropertyType = propertyType;
            this.ReferenceEDID = referenceEDID;
            this.TES4FormID = tes4FormID;
            this.tes5FormID = tes5FormID;
            this.trackedScript = null;
        }

        public ITES5Type TES5DeclaredType
        {
            get
            {
                return TES5Type;
            }
        }

        public IEnumerable<string> Output
        {
            get
            {
                string propertyTypeName = this.TES5Type.Output.Single();
                //Todo - Actually differentiate between properties which need and do not need to be conditional
                string output = propertyTypeName + " Property " + this.Name + " Auto Conditional";
                if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(this.TES5Type, originalPropertyType))
                {//If type has not been changed
                    if (TES4FormID != null)
                    {
                        output += ";TES4FormID:" + TES4FormID.Value.ToString() + ";";
                    }
                    if (tes5FormID != null)
                    {
                        output += ";TES5FormID:" + tes5FormID.Value.ToString() + ";";
                    }
                }
                yield return output;
            }
        }

        public void Rename(string newNameWithoutSuffix)
        {
            this.Name = AddPropertyNameSuffix(newNameWithoutSuffix);
        }

        public static string AddPropertyNameSuffix(string propertyName, bool throwExceptionIfSuffixAlreadyPresent = true)
        {
            if (throwExceptionIfSuffixAlreadyPresent && propertyName.EndsWith(PROPERTY_SUFFIX))
            {
                throw new ArgumentException(nameof(propertyName) + " already ended with suffix (" + PROPERTY_SUFFIX + "):  " + propertyName);
            }
            return propertyName + PROPERTY_SUFFIX;//we"re adding _p prefix because papyrus compiler complains about property names named after other scripts, _p makes sure we won"t conflict.
        }

        public ITES5Type TES5Type
        {
            get
            {
                return this.trackedScript != null ? this.trackedScript.ScriptType : this.propertyType;
            }
            set
            {
                if (this.trackedScript != null)
                {
                    this.trackedScript.SetNativeType(value);
                }
                else
                {
                    this.propertyType = value;
                }
            }
        }

        public void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            this.trackedScript = scriptHeader;
            ITES5Type ourNativeType = this.propertyType.NativeType;
            ITES5Type remoteNativeType = this.trackedScript.ScriptType.NativeType;
            /*
             * Scenario 1 - types are equal or the remote type is higher than ours in which case we do nothing as they have the good type anyway
             */
            if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(remoteNativeType, ourNativeType))
            {
                return;
            }

            /*
             * Scenario 2 - Our current native type is extending remote script"s extended type - we need to set it properly
             */
            else if (TES5InheritanceGraphAnalyzer.IsExtending(ourNativeType, remoteNativeType))
            {
                this.trackedScript.SetNativeType(ourNativeType);
            }
            else
            {
                //WTM:  Note:  Special Cases:  Let some script transpile without throwing.
                if (!(
                    (ourNativeType == TES5BasicType.T_ACTIVATOR && remoteNativeType == TES5BasicType.T_OBJECTREFERENCE && (scriptHeader.EDID == "SE01MetronomeScript" || scriptHeader.EDID == "SE11TrigZoneHowlingHallsEnterSCRIPT")) ||
                    (ourNativeType == TES5BasicType.T_ACTORBASE && remoteNativeType == TES5BasicType.T_ACTOR && (scriptHeader.EDID == "HieronymusLexScript" || scriptHeader.EDID == "SERunsInCirclesSCRIPT"))))
                {
                    throw new ConversionException(nameof(TES5Property) + "." + nameof(TrackRemoteScript) + ":  The definitions of local property type and remote property type have diverged.  (Ours: " + ourNativeType.Value + ", remote: " + remoteNativeType.Value + ")");
                }
            }
        }
    }
}