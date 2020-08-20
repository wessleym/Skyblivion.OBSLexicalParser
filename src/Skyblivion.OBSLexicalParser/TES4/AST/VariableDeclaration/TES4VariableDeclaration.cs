using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration
{
    class TES4VariableDeclaration : ITES4CodeChunk//WTM:  Change:  I added ITES4CodeChunk to this class.  It was previously used on TES4VariableDeclarationList.
    {
        public string VariableName { get; private set; }
        public TES4Type VariableType { get; private set; }
        public readonly List<int> FormIDs;//WTM:  Change:  Added
        public readonly TES5BasicType? TES5Type;//WTM:  Change:  Added
        public TES4VariableDeclaration(string variableName, TES4Type variableType, List<int>? formIDs = null, TES5BasicType? tes5Type = null)
        {
            this.VariableName = PapyrusCompiler.FixReferenceName(variableName);
            this.VariableType = variableType;
            FormIDs = formIDs != null ? formIDs : new List<int>();
            TES5Type = tes5Type;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}