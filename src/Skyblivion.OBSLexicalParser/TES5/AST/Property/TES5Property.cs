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
        public string ReferenceEDID { get; private set; }
        public bool IsPlayerRef { get; private set; }
        private TES5ScriptHeader trackedScript;

        public TES5Property(string name, ITES5Type propertyType, string referenceEDID, bool isPlayerRef = false)
        {
            this.OriginalName = name;
            this.AllowNameTransformation = !isPlayerRef;
            this.Name = AllowNameTransformation ? AddPropertyNameSuffix(name) : name;
            this.propertyType = propertyType;
            this.ReferenceEDID = referenceEDID;
            this.IsPlayerRef = isPlayerRef;
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
                yield return propertyTypeName + " Property " + this.Name + " Auto Conditional";
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

        public void TrackRemoteScript(TES5ScriptHeader scriptHeader)
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