namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5InheritanceFunctionSignature
    {
        public string Name { get; private set; }
        public TES5BasicType[] Arguments { get; private set; }
        public ITES5Type ReturnType { get; private set; }
        public TES5InheritanceFunctionSignature(string name, TES5BasicType[] arguments, ITES5Type returnType)
        {
            Name = name;
            Arguments = arguments;
            ReturnType = returnType;
        }
    }
}
