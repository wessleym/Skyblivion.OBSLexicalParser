using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5CustomType : ITES5Type, IEquatable<ITES5Type>
    {
        private readonly string prefix;
        public TES5BasicType NativeType { get; set; }
        private static readonly EquatableUtility<TES5CustomType, ITES5Type> equatableUtility = new EquatableUtility<TES5CustomType, ITES5Type>((left, right) =>
          {
              if (left.Value == right.Value && left.NativeType == right.NativeType)
              {
                  TES5CustomType? rightCustomType = right as TES5CustomType;
                  return rightCustomType is null || left.prefix == rightCustomType.prefix;
              }
              return false;
          });

        public TES5CustomType(string originalName, string prefix, TES5BasicType nativeType)
        {
            this.Value = NameTransformer.Limit(originalName, prefix);
            this.prefix = prefix;
            this.OriginalName = originalName;
            this.NativeType = nativeType;
            //qt = new ReflectionClass(get_class(this));//WTM:  Change:  Unused
            //this.constants = qt.getConstants();
        }

        private static bool Equals(TES5CustomType left, ITES5Type? right)
        {
            return equatableUtility.Equals(left, right);
        }
        public bool Equals(ITES5Type? other)
        {
            return Equals(this, other);
        }
        public override bool Equals(object? obj)
        {
            return equatableUtility.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 1325511845;
            hashCode = hashCode * -1521134295 + EqualityComparer<ITES5Type>.Default.GetHashCode(NativeType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(prefix);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public string Value { get; private set; }

        public IEnumerable<string> Output
        {
            get
            {
                bool includePrefix = this.Value != TES5BasicType.TES4TimerHelperName && this.Value != TES5BasicType.TES4ContainerName;//no time to refactor now, later.
                yield return (includePrefix ? this.prefix : "") + this.Value;
            }
        }

        public bool IsPrimitive => this.NativeType.IsPrimitive;

        public bool IsNativePapyrusType => false;

        public bool AllowInference => false;

        public bool AllowNativeTypeInference => true;

        public string OriginalName { get; private set; }
    }
}