using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModPCMiscStatFactory : IFunctionFactory//WTM:  Change:  Added
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        public ModPCMiscStatFactory(TES5ObjectCallFactory objectCallFactory, LogUnknownFunctionFactory logUnknownFunctionFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments tes4Arguments = function.Arguments;
            int statInt = ((TES4Integer)tes4Arguments[0]).IntValue;
            string statName;
            if (statInt == 14) { statName = "Horses Owned"; }
            else if (statInt == 15) { statName = "Houses Owned"; }
            else if (statInt == 16) { statName = "Stores Invested In"; }
            else if (statInt == 27) { statName = "Nirnroots Found"; }
            else
            {
                //Oblivion contains many calls to statInt = 19 (Artifacts Found)
                return logUnknownFunctionFactory.CreateLogCall(function);
            }
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            arguments.Add(new TES5String(statName));
            if (tes4Arguments.Count > 1)
            {
                int modAmount = ((TES4Integer)tes4Arguments[1]).IntValue;
                arguments.Add(new TES5Integer(modAmount));
            }
            return objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Game, "IncrementStat", arguments);
        }
    }
}
