using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5CustomType : ITES5Type, IEquatable<TES5CustomType>
    {
        private string escapedName;
        public ITES5Type NativeType { get; set; }
        /*
        * Original type name
         * Needed only for compilation graph build.. will be scrapped once this is cleaned up properly.
        */
        private string originalName;
        private string prefix;
        public TES5CustomType(string originalName, string prefix, ITES5Type nativeType)
        {
            this.escapedName = NameTransformer.Limit(originalName, prefix);
            this.prefix = prefix;
            this.originalName = originalName;
            //qt = new ReflectionClass(get_class(this));WTM:  Change:  Unused
            //this.constants = qt.getConstants();
            this.NativeType = nativeType;
        }

        private static bool Equals(TES5CustomType left, TES5CustomType right)
        {
            if (object.ReferenceEquals(left, right)) { return true; }
            bool leftIsNull = object.ReferenceEquals(left, null);
            bool rightIsNull = object.ReferenceEquals(right, null);
            if (leftIsNull && rightIsNull) { return true; }
            if (leftIsNull || rightIsNull) { return false; }
            return
                left.escapedName == right.escapedName &&
                left.NativeType == right.NativeType &&
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
            return (escapedName + "|" + NativeType.Value + "|" + prefix).GetHashCode();
        }

        public static bool operator ==(TES5CustomType left, TES5CustomType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TES5CustomType left, TES5CustomType right)
        {
            return !Equals(left, right);
        }

        public string Value => this.escapedName;

        public IEnumerable<string> Output
        {
            get
            {
                bool includePrefix = this.escapedName != TES5BasicType.TES4TimerHelperName && this.escapedName != TES5BasicType.TES4ContainerName;//no time to refactor now, later.
                return new string[] { (includePrefix ? this.prefix : "") + this.escapedName };
            }
        }

        public bool IsPrimitive => this.NativeType.IsPrimitive;

        public bool IsNativePapyrusType => false;

        public string OriginalName => this.originalName;
    }
}