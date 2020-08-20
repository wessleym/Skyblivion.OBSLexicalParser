using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5Property : ITES5VariableOrProperty
    {
        private const string PROPERTY_SUFFIX = "_p";
        public string OriginalName { get; private set; }
        public bool AllowNameTransformation { get; private set; }
        public string Name { get; private set; }
        private ITES5Type propertyType; //If we"re tracking a script, this won"t be used anymore
        private readonly ITES5Type originalPropertyType; //If we"re tracking a script, this won"t be used anymore
        public string? ReferenceEDID { get; private set; }
        public bool IsPlayerRef { get; private set; }
        private TES5ScriptHeader? trackedScript;
        private readonly List<int> tes4FormIDs;

        public TES5Property(string name, ITES5Type propertyType, string? referenceEDID, List<int> tes4FormIDs)
        {
            this.IsPlayerRef = name == TES5PlayerReference.PlayerRefName && propertyType == TES5PlayerReference.TES5TypeStatic && referenceEDID == TES5PlayerReference.PlayerRefName;
            this.OriginalName = name;
            this.AllowNameTransformation = !this.IsPlayerRef;
            this.Name = AllowNameTransformation ? AddPropertyNameSuffix(name) : name;
            this.propertyType = propertyType;
            originalPropertyType = propertyType;
            this.ReferenceEDID = referenceEDID;
            this.tes4FormIDs = tes4FormIDs;
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
                //if property has a TES4 form ID and its type has not been changed
                if (tes4FormIDs.Any() && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(this.TES5Type, originalPropertyType))
                {
                    bool multiple = tes4FormIDs.Count > 1;
                    output += ";TES4FormID" + (multiple ? "Multiple" : "") + ":" + string.Join(",", tes4FormIDs) + ";";
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