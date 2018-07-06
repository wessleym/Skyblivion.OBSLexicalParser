using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    class TES4Float : ITES4Primitive
    {
        private readonly float data;
        public TES4Float(float data)
        {
            this.data = data;
        }

        public object Data => this.data;

        public string StringValue => this.data.ToString(CultureInfo.InvariantCulture);

        public TES4Type Type => TES4Type.T_FLOAT;

        public bool HasFixedValue => true;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}