namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5InheritanceFunctionSignature
    {
        public string Name { get; }
        public TES5BasicType[] Arguments { get; }
        public ITES5Type ReturnType { get; }
        public TES5InheritanceFunctionSignature(string name, TES5BasicType[] arguments, ITES5Type returnType)
        {
            Name = name;
            Arguments = arguments;
            ReturnType = returnType;
        }
    }
}
