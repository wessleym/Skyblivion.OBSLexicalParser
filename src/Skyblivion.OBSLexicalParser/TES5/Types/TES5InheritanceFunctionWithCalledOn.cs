namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    internal class TES5InheritanceFunctionWithCalledOn : TES5InheritanceFunctionSignature
    {
        public TES5BasicType CalledOn { get; }
        public TES5InheritanceFunctionWithCalledOn(string name, TES5BasicType[] arguments, ITES5Type returnType, TES5BasicType calledOn) : base(name, arguments, returnType)
        {
            CalledOn = calledOn;
        }
        public TES5InheritanceFunctionWithCalledOn(TES5InheritanceFunctionSignature function, TES5BasicType calledOn) : base(function.Name, function.Arguments, function.ReturnType)
        {
            CalledOn = calledOn;
        }
    }
}
