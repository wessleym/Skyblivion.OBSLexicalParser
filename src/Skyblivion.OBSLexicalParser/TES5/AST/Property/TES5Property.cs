using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5Property : TES5VariableOrProperty
    {
        private const string PROPERTY_SUFFIX = "_p";
        private ITES5Type propertyType;
        private TES5ScriptHeader trackedScript;
        private readonly string referenceEDID;
        public TES5Property(string propertyName, ITES5Type propertyType, string referenceEDID)
            : base(AddPropertyNameSuffix(propertyName))
        {
            this.propertyType = propertyType; //If we"re tracking a script, this won"t be used anymore
            this.referenceEDID = referenceEDID;
            this.trackedScript = null;
        }

        public override IEnumerable<string> Output
        {
            get
            {
                string propertyTypeName = this.TES5Type.Output.Single();
                //Todo - Actually differentiate between properties which need and do not need to be conditional
                yield return propertyTypeName + " Property " + this.Name + " Auto Conditional";
            }
        }

        public override string ReferenceEDID => referenceEDID;

        public string GetPropertyNameWithoutSuffix()
        {
            return RemovePropertyNameSuffix(this.Name);
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

        private static string RemovePropertyNameSuffix(string propertyName)
        {
            return propertyName.Substring(0, propertyName.Length - PROPERTY_SUFFIX.Length);
        }

        public override ITES5Type TES5Type
        {
            get
            {
                return this.trackedScript != null ? this.trackedScript.ScriptType: this.propertyType;
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

        public override void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            this.trackedScript = scriptHeader;
            ITES5Type ourNativeType = this.propertyType.NativeType;
            ITES5Type remoteNativeType = this.trackedScript.ScriptType.NativeType;
            /*
             * Scenario 1 - types are equal or the remote type is higher than ours in which case we do nothing as they have the good type anyway
             */
            if (ourNativeType == remoteNativeType || TES5InheritanceGraphAnalyzer.IsExtending(remoteNativeType, ourNativeType))
            {
                return;
            }

            /*
             * Scenario 2 - Our current native type is extending remote script"s extended type - we need to set it properly
             */
            else if(TES5InheritanceGraphAnalyzer.IsExtending(ourNativeType, remoteNativeType))
            {
                this.trackedScript.SetNativeType(ourNativeType);
            }
            else 
            {
                throw new ConversionException(nameof(TES5Property) + "." + nameof(TrackRemoteScript) + ":  The definitions of local property type and remote property type have diverged.  (Ours: " + ourNativeType.Value + ", remote: " + remoteNativeType.Value);
            }
        }
    }
}