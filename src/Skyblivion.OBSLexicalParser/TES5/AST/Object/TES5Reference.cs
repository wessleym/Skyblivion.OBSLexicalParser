using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5Reference : TES5Castable, ITES5Referencer
    {
        public ITES5VariableOrProperty ReferencesTo { get; }
        public TES5Reference(ITES5VariableOrProperty referencesTo)
        {
            this.ReferencesTo = referencesTo;
        }

        public string OutputSingle
        {
            get
            {
                //First let's check if we do the hacky int-float cast magic
                //If yes, then use that one
                if (ManualCastTo != null)
                {
                    return ReferencesTo.Name + ManualCastToOutput;
                }
                else
                {
                    //Then, let's check if declaring type of our referenced variable
                    //is same as the inferenced type
                    //They're sometimes not the same ( for instance, when we have 
                    //a set-in-stone native function signature variable and we cannot infer
                    //the type at will and change the function's signature
                    if (!ReferencesTo.TES5DeclaredType.Equals(ReferencesTo.TES5Type))
                    {
                        //These parentheses are required since some other code might call
                        //(obj as Other).Invoke()
                        //which, without parentheses, becomes
                        //obj as Other.Invoke()
                        return "(" + ReferencesTo.Name + " as " + ReferencesTo.TES5Type.Value + ")";
                    }
                    else
                    {
                        return ReferencesTo.Name;
                    }
                }
            }
        }

        public IEnumerable<string> Output => [OutputSingle];

        public string Name => this.ReferencesTo.Name;

        public ITES5Type TES5Type => this.ReferencesTo.TES5Type;
    }
}