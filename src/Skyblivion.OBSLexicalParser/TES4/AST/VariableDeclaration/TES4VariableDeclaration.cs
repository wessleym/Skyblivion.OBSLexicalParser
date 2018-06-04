using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration
{
    class TES4VariableDeclaration : ITES4CodeChunk//WTM:  Change:  I added ITES4CodeChunk to this class.  It was previously used on TES4VariableDeclarationList.
    {
        public string VariableName { get; private set; }
        public TES4Type VariableType { get; private set; }
        public TES4VariableDeclaration(string variableName, TES4Type variableType)
        {
            this.VariableName = PapyrusCompiler.FixReferenceName(variableName);
            this.VariableType = variableType;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}