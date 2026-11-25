using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface ITranspileCommand
    {
        TES5Script Transpile(string sourcePath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope);
    }
}