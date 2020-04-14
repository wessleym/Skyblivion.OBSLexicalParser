using System;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    public class EquatableUtility<T, TBase> where T : class where TBase : class
    {
        private readonly Func<T, TBase, bool> equalsFunc;
        public EquatableUtility(Func<T, TBase, bool> equalsFunc)
        {
            this.equalsFunc = equalsFunc;
        }

        public bool Equals(T? left, TBase? right)
        {
            if (object.ReferenceEquals(left, right)) { return true; }
            if (left is null || right is null) { return (left is null) == (right is null); }
            return equalsFunc(left, right);
        }
        public bool Equals(T left, object obj)
        {
            TBase? baseObj = obj as TBase;
            return !(baseObj is null) ? Equals(left, baseObj) : false;
        }
    }
}
