using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System;

namespace Skyblivion.OBSLexicalParser.Builds.PF
{
    class TranspileCommand : ITranspileCommand
    {
        public void initialize(Build build)
        {
        // TODO: Implement initialize() method.
        }

        public TES5Target transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            // TODO: Implement transpile() method.
            throw new NotImplementedException();
        }
    }
}