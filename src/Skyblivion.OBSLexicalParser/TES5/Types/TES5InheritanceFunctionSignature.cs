namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    class TES5InheritanceFunctionSignature
    {
        public string Name { get; private set; }
        public string[] Arguments { get; private set; }
        public string ReturnType { get; private set; }
        public TES5InheritanceFunctionSignature(string name, string[] arguments, string returnType)
        {
            Name = name;
            Arguments = arguments;
            ReturnType = returnType;
        }
    }
}
