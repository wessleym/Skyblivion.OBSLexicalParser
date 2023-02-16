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
    class SayFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SayFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();

            //if (calledOn.TES5Type != TES5BasicType.T_OBJECTREFERENCE)
            //{
            //    TES5Castable calledOnCastable = calledOn as TES5Reference;
            //    if (calledOn != null && TES5InheritanceGraphAnalyzer.IsExtending(TES5BasicType.T_ACTOR, calledOn.TES5Type))
            //    {
            //        calledOnCastable.ManualCastTo = TES5BasicType.Tl_OBJECTREFERENCE;
            //    }
            //}
            //arguments.Add(calledOn);

            ITES5Value argument0 = this.valueFactory.CreateValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope);
            //WTM:  Change:  Why is the below necessary?
            /*if (argument0.TES5Type != TES5BasicType.T_TOPIC)
            {
                TES5Reference? argument0Reference = argument0 as TES5Reference;
                if (argument0Reference != null && TES5InheritanceGraphAnalyzer.IsExtending(TES5BasicType.T_TOPIC, argument0.TES5Type))
                {
                    argument0Reference.ManualCastTo = TES5BasicType.T_TOPIC;
                }
            }*/
            arguments.Add(argument0);

            //ITES4StringValue optionalFlag = functionArguments.GetOrNull(2);
            //if (optionalFlag != null)
            //{
            //    string optionalFlagDataString = optionalFlag.StringValue;
            //    if (this.analyzer.GetFormTypeByEDID(optionalFlagDataString).Value!= TES4RecordType.REFR.Name)
            //    {
            //        this.metadataLogService.WriteLine("ADD_SPEAK_AS_ACTOR", new string[] { optionalFlagDataString });
            //        optionalFlag = new TES4ApiToken(optionalFlag.Data+"Ref");
            //    }
            //    arguments.Add(this.valueFactory.CreateValue(optionalFlag, codeScope, globalScope, multipleScriptsScope));
            //}
            //else
            //{
            //    arguments.Add(new TES5None());
            //}

            arguments.Add(new TES5Bool(true));

            //TES5LocalScope localScope = codeScope.LocalScope;
            //ITES5Referencer timerReference = this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope);
            //return this.objectCallFactory.CreateObjectCall(timerReference, "LegacySay", multipleScriptsScope, arguments);
            return this.objectCallFactory.CreateObjectCall(calledOn, "LegacySay", arguments, comment: function.Comment);
        }
    }
}