#if ALTERNATE_TYPE_MAPPING
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    //WTM:  Change:  Added
    //This class allows types to start out as something (a Door, for example).
    //Then, if the inferencer needs because the Door doesn't support a particular method, it can be converted to a Form one time.
    //This will probably cause runtime errors, but it allows compilation for now.
    //See other references to ALTERNATE_TYPE_MAPPING.
    class TES5BasicTypeRevertible : ITES5Type, IEquatable<ITES5Type>
    {
        private TES5BasicType nativeType;

        public TES5BasicTypeRevertible(TES5BasicType nativeType)
        {
            this.nativeType = nativeType;
            mayRevertToForm = true;
        }

        private static readonly EquatableUtility<TES5BasicTypeRevertible, ITES5Type> equatableUtility = new EquatableUtility<TES5BasicTypeRevertible, ITES5Type>((left, right) =>
        {
            return left.Value == right.Value && left.NativeType == right.NativeType;
        });

        private static bool Equals(TES5BasicTypeRevertible? left, ITES5Type? right)
        {
            return equatableUtility.Equals(left, right);
        }
        public bool Equals(ITES5Type other)
        {
            return Equals(this, other);
        }
        public override bool Equals(object obj)
        {
            return equatableUtility.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            int hashCode = -1291633308;
            hashCode = hashCode * -1521134295 + EqualityComparer<TES5BasicType>.Default.GetHashCode(NativeType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public TES5BasicType NativeType { get => nativeType; set { mayRevertToForm = false; nativeType = value; } }

        public TES5BasicTypeRevertible Revertible => this;

        public string Value => NativeType.Value;

        public bool IsPrimitive => NativeType.IsPrimitive;

        public bool AllowInference => NativeType.AllowInference;

        public bool AllowNativeTypeInference => true;

        public bool IsNativePapyrusType => NativeType.IsNativePapyrusType;

        public string Name => NativeType.Name;

        public string OriginalName => NativeType.OriginalName;

        public IEnumerable<string> Output => NativeType.Output;

        private bool mayRevertToForm;

        private bool TryRevertToForm(bool throwException)
        {
            if (OriginalName.ToLower().Contains("Dark11ChorrolDrop".ToLower()) || OriginalName == "Dark11ChorrolDropScript")
            { }
            if (!mayRevertToForm)
            {
                if (throwException) { throw new InvalidOperationException("!" + nameof(mayRevertToForm)); }
                return false;
            }
            NativeType = TES5BasicType.T_FORM;
            return true;
        }

        public bool TryRevertToForm()
        {
            return TryRevertToForm(false);
        }
    }
}
#endif