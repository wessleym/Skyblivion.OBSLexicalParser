using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5FragmentFunctionScopeFactory
    {
        public static TES5FunctionScope createFromFragmentType(string fragmentName, TES5FragmentType fragmentType)
        {
            TES5FunctionScope localScope = new TES5FunctionScope(fragmentName);
            if (fragmentType == TES5FragmentType.T_TIF)
            {
                localScope.AddVariable(new TES5LocalVariable("akSpeakerRef", TES5BasicType.T_OBJECTREFERENCE, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
            }
            else if (fragmentType == TES5FragmentType.T_PF)
            {
                localScope.AddVariable(new TES5LocalVariable("akActor", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
            }
            return localScope;
        }
    }
}