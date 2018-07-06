using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    abstract class TES5VariableOrProperty : ITES5VariableOrProperty
    {
        public string Name { get; protected set; }
        public virtual ITES5Type TES5Type { get; set; }
        public TES5VariableOrProperty(string name)
        {
            this.Name = name;
        }
        public TES5VariableOrProperty(string name, ITES5Type type)
            : this(name)
        {
            this.TES5Type = type;
        }

        public virtual IEnumerable<string> Output => new string[] { this.TES5Type.Value + " " + this.Name };

        /*
        * Each property may be referencing to a specific EDID ( either it"s a converted property and its name minus prefix should match it, or it"s a new property created and then it ,,inherits" :)
        */
        public abstract string ReferenceEDID { get; }

        public abstract void TrackRemoteScript(TES5ScriptHeader scriptHeader);
    }
}
