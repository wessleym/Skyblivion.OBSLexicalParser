using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5FragmentFunctionScopeFactory
    {
        public TES5FunctionScope createFromFragmentType(string fragmentName, TES5FragmentType fragmentType)
        {
            TES5FunctionScope localScope = new TES5FunctionScope(fragmentName);
            if (fragmentType == TES5FragmentType.T_TIF)
            {
                localScope.addVariable(new TES5LocalVariable("akSpeakerRef", TES5BasicType.T_OBJECTREFERENCE, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
            }
            else if (fragmentType == TES5FragmentType.T_PF)
            {
                localScope.addVariable(new TES5LocalVariable("akActor", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
            }
            return localScope;
        }
    }
}