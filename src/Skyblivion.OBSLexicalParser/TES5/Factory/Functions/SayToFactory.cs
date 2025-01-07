using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SayToFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SayToFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            //Simple implementation without looking.
            TES5ObjectCallArguments arguments = [calledOn];
            //if (calledOn.TES5Type != TES5BasicType.T_OBJECTREFERENCE)
            //{
            //    TES5Castable calledOnCastable = calledOn as TES5Reference;
            //    if (calledOn != null && TES5InheritanceGraphAnalyzer.IsExtending(TES5BasicType.T_ACTOR, calledOn.TES5Type))
            //    {
            //        calledOnCastable.ManualCastTo = TES5BasicType.T_OBJECTREFERENCE;
            //    }
            //}
            //arguments.Add(calledOn);
            arguments.Add(this.valueFactory.CreateValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope));
            ITES5Value argument1 = this.valueFactory.CreateValue(functionArguments[1], codeScope, globalScope, multipleScriptsScope);
            //WTM:  Change:  Why is the below necessary?
            /*if (argument1.TES5Type != TES5BasicType.T_TOPIC)
            {
                TES5Reference? argument1Reference = argument1 as TES5Reference;
                if (argument1Reference != null && TES5InheritanceGraphAnalyzer.IsExtending(TES5BasicType.T_TOPIC, argument1.TES5Type))
                {
                    argument1Reference.ManualCastTo = TES5BasicType.T_TOPIC;
                }
            }*/
            arguments.Add(argument1);
            arguments.Add(new TES5Bool(true));

            //TES5LocalScope localScope = codeScope.LocalScope;
            //ITES5Referencer timerReference = this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope);
            return this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Create(TES5BasicType.T_TES4ObjectReferenceUtility), "LegacySayTo", arguments, comment: function.Comment, inference: false);
        }
    }
}