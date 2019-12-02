using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5StaticReference : ITES5Referencer
    {
        public string Name { get; private set; }
        private readonly ESMAnalyzer esmAnalyzer;
        public TES5StaticReference(string name, ESMAnalyzer esmAnalyzer)
        {
            this.Name = name;
            this.esmAnalyzer = esmAnalyzer;
        }

        public IEnumerable<string> Output => new string[] { this.Name };

        public ITES5Type TES5Type => TES5TypeFactory.MemberByValue(this.Name, null, esmAnalyzer);

        public ITES5VariableOrProperty? ReferencesTo => null;
    }
}