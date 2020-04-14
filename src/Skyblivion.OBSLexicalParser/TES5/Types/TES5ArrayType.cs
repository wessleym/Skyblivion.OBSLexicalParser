namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5ArrayType : TES5BasicType//WTM:  Added
    {
        private TES5ArrayType(TES5BasicType elementType)
            : base(elementType.Name + "[]")
        { }

        public static readonly TES5ArrayType
            ArrayBool = new TES5ArrayType(T_BOOL),
            ArrayFloat = new TES5ArrayType(T_FLOAT),
            ArrayInt = new TES5ArrayType(T_INT),
            ArrayString = new TES5ArrayType(T_STRING);
    }
}
