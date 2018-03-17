using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4FunctionArguments
    {
        private List<ITES4StringValue> values = new List<ITES4StringValue>();
        public void add(ITES4StringValue declaration)
        {
            this.values.Add(declaration);
        }

        public int count()
        {
            return this.values.Count;
        }

        public List<ITES4StringValue> getValues()
        {
            return this.values;
        }

        public ITES4StringValue popValue(int index)
        {
            ITES4StringValue[] valuesArray = values.ToArray();
            List<ITES4StringValue> newValues = new List<ITES4StringValue>();
            ITES4StringValue toReturn = null;
            for (int i=0;i< valuesArray.Length;i++)
            {
                var value = valuesArray[i];
                if (i == index)
                {
                    toReturn = value;
                }
                else
                {
                    newValues.Add(value);
                }
            }

            if (toReturn == null)
            {
                throw new ConversionException("Cannot pop index "+index.ToString());
            }

            this.values = newValues;
            return toReturn;
        }

        public ITES4StringValue getValue(int i)
        {//previously isset
            try
            {
                return values[i];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public void setValue(int i, ITES4StringValue value)
        {
            this.values[i] = value;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.values.SelectMany(v => v.filter(predicate)).ToArray();
        }

        internal bool Any()
        {
            throw new NotImplementedException();
        }
    }
}