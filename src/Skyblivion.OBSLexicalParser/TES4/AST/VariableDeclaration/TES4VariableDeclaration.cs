using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration
{
    class TES4VariableDeclaration : ITES4CodeChunk//WTM:  Change:  I added ITES4CodeChunk to this class.  It was previously used on TES4VariableDeclarationList.
    {
        private string variableName;
        private TES4Type variableType;
        public TES4VariableDeclaration(string variableName, TES4Type variableType)
        {
            this.variableName = variableName;
            this.variableType = variableType;
        }

        public string getVariableName()
        {
            //Papyrus compiler somehow treats properties with ,,temp" in them in a special way, so we change them to tmp to accomodate that.
            variableName = Regex.Replace(variableName, "temp", "tmp", RegexOptions.IgnoreCase);
            return variableName;
        }

        public TES4Type getVariableType()
        {
            return this.variableType;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}