using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class PayFineFactory : IFunctionFactory
    {
        private TES5ReferenceFactory referenceFactory;
        private TES5ObjectCallFactory objectCallFactory;
        public PayFineFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer faction = this.referenceFactory.CreateCyrodiilCrimeFactionReadReference(globalScope, multipleScriptsScope, localScope);
            /*TES5PlayerReference player = this.referenceFactory.createReferenceToPlayer();
            const string functionName = "PayCrimeGold";
            TES5ObjectCallArguments argumentList = new TES5ObjectCallArguments();
            argumentList.add(new TES5Integer(1));
            argumentList.add(new TES5Integer(1));
            argumentList.add(faction);*/
            //WTM:  Change:
            const string functionName = "PlayerPayCrimeGold";
            TES5ObjectCallArguments argumentList = new TES5ObjectCallArguments();
            argumentList.Add(new TES5Bool(true));//Remove gold.
            argumentList.Add(new TES5Bool(false));//Do not send to jail.
            return this.objectCallFactory.CreateObjectCall(faction, functionName, multipleScriptsScope, argumentList);
        }
    }
}