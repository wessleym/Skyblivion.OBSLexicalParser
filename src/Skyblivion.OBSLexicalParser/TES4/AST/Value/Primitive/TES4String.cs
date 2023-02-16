using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    class TES4String : ITES4Primitive
    {
        private readonly string data;
        public TES4String(string data)
        {
            this.data = data.Trim('"');
        }

        public object Constant => StringValue;

        public string StringValue => this.data;

        public TES4Type Type => TES4Type.T_STRING;
    }
}
