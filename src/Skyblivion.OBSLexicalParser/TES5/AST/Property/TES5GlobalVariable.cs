using System;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5GlobalVariable : TES5VariableOrProperty
    {
        public TES5GlobalVariable(string name)
            : base(name, null)
        { }

        public override string ReferenceEDID => throw new NotImplementedException();

        public override void TrackRemoteScript(TES5ScriptHeader scriptHeader)
        {
            throw new NotImplementedException();
        }
    }
}