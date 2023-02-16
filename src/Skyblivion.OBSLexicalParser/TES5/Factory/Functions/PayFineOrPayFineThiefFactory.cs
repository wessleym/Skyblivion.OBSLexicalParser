using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class PayFineOrPayFineThiefFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly bool payFineThief;
        protected PayFineOrPayFineThiefFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, bool payFineThief)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
            this.payFineThief = payFineThief;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
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
            TES5ObjectCallArguments argumentList = new TES5ObjectCallArguments()
            {
                new TES5Bool(true),//Remove gold.
                new TES5Bool(payFineThief)//Do or do not send to jail based on payFineThief.
            };
            return this.objectCallFactory.CreateObjectCall(faction, functionName, argumentList, comment: function.Comment);
        }
    }
}
