using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5CustomType : ITES5Type, IEquatable<TES5CustomType>
    {
        const string T_TES4CONTAINER = "TES4Container";
        const string T_TES4TIMERHELPER = "TES4TimerHelper";
        private string typeName;
        private ITES5Type nativeType;
        /*
        * Original type name
         * Needed only for compilation graph build.. will be scrapped once this is cleaned up properly.
        */
        private string originalName;
        private string prefix;
        public TES5CustomType(string typeName, string prefix, string originalName, ITES5Type nativeType)
        {
            this.typeName = typeName;
            this.prefix = prefix;
            this.originalName = originalName;
            //qt = new ReflectionClass(get_class(this));WTM:  Change:  Unused
            //this.constants = qt.getConstants();
            this.nativeType = nativeType;
        }

        private static bool Equals(TES5CustomType left, TES5CustomType right)
        {
            if (object.ReferenceEquals(left, right)) { return true; }
            bool leftIsNull = object.ReferenceEquals(left, null);
            bool rightIsNull = object.ReferenceEquals(right, null);
            if (leftIsNull && rightIsNull) { return true; }
            if (leftIsNull || rightIsNull) { return false; }
            return
                left.typeName == right.typeName &&
                left.nativeType == right.nativeType &&
                left.prefix == right.prefix;
        }
        public bool Equals(TES5CustomType other)
        {
            return Equals(this, other);
        }
        public override bool Equals(object obj)
        {
            TES5CustomType customType = obj as TES5CustomType;
            return !object.ReferenceEquals(customType, null) ? Equals(customType) : false;
        }

        public override int GetHashCode()
        {
            return (typeName + "|" + nativeType + "|" + prefix).GetHashCode();
        }

        public static bool operator ==(TES5CustomType left, TES5CustomType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TES5CustomType left, TES5CustomType right)
        {
            return !Equals(left, right);
        }

        public string value()
        {
            return this.typeName;
        }

        public IEnumerable<string> output()
        {
            bool includePrefix = this.typeName != "TES4TimerHelper" && this.typeName != "TES4Container";//no time to refactor now, later.
            return new string[] { (includePrefix ? this.prefix : "") + this.value() };
        }

        public bool isPrimitive()
        {
            return this.nativeType.isPrimitive();
        }

        public bool isNativePapyrusType()
        {
            return false;
        }

        public ITES5Type getNativeType()
        {
            return this.nativeType;
        }

        public void setNativeType(ITES5Type basicType)
        {
            this.nativeType = basicType;
        }

        public string getOriginalName()
        {
            return this.originalName;
        }
    }
}