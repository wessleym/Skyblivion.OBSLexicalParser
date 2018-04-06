using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5Property : ITES5Variable
    {
        const string PROPERTY_SUFFIX = "_p";
        /*
        * The property"s name as seen in script
        */
        private string propertyName;
        /*
        * Property"s type
        */
        private ITES5Type propertyType;
        /*
        * Each property may be referencing to a specific EDID ( either it"s a converted property and its name minus prefix should match it, or it"s a new property created and then it ,,inherits" :)
        */
        private string referenceEdid;
        /*
        * Tracked remote script, if any
        */
        private TES5ScriptHeader trackedScript;
        public TES5Property(string propertyName, ITES5Type propertyType, string referenceEdid)
        {
            this.propertyName = AddPropertyNameSuffix(propertyName);
            this.propertyType = propertyType; //If we"re tracking a script, this won"t be used anymore
            this.referenceEdid = referenceEdid;
            this.trackedScript = null;
        }

        public IEnumerable<string> Output
        {
            get
            {
                string propertyTypeName = this.getPropertyType().Output.Single();
                //Todo - Actually differentiate between properties which need and do not need to be conditional
                return new string[] { propertyTypeName + " Property " + this.propertyName + " Auto Conditional" };
            }
        }

        public string getPropertyName()
        {
            return this.propertyName;
        }

        public void renameTo(string newName)
        {
            this.propertyName = AddPropertyNameSuffix(newName);
        }

        public string GetPropertyNameWithoutSuffix()
        {
            return RemovePropertyNameSuffix(this.propertyName);
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

        public ITES5Type getPropertyType()
        {
            return this.trackedScript != null ? this.trackedScript.getScriptType() : this.propertyType;
        }

        public void setPropertyType(ITES5Type type)
        {
            if (this.trackedScript != null)
            {
                this.trackedScript.setNativeType(type);
            }
            else
            {
                this.propertyType = type;
            }
        }

        public string getReferenceEdid()
        {
            return this.referenceEdid;
        }

        public void trackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            this.trackedScript = scriptHeader;
            ITES5Type ourNativeType = this.propertyType.getNativeType();
            ITES5Type remoteNativeType = this.trackedScript.getScriptType().getNativeType();
            /*
             * Scenario 1 - types are equal or the remote type is higher than ours in which case we do nothing as they have the good type anyway
             */
            if (ourNativeType == remoteNativeType || TES5InheritanceGraphAnalyzer.isExtending(remoteNativeType, ourNativeType))
            {
                return;
            }

            /*
             * Scenario 2 - Our current native type is extending remote script"s extended type - we need to set it properly
             */
            else if(TES5InheritanceGraphAnalyzer.isExtending(ourNativeType, remoteNativeType))
            {
                this.trackedScript.setNativeType(ourNativeType);
            }
            else 
            {
                throw new ConversionException("TES5Property.trackRemoteScript() - The definitions of local property type and remote property type have diverged(ours: " + ourNativeType.value() + ", remote: " + remoteNativeType.value());
            }
        }
    }
}