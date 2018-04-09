using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPCMiscStatFactory : IFunctionFactory
    {
        //WTM:  Note:  GetPCMiscStat only seems to be used as "GetPCMiscStat 7 >= 30" in Standalone\Source\rusiabradusscript.txt.
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public GetPCMiscStatFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
        }

        private static Dictionary<int, string> statMap = new Dictionary<int, string>()
        {
            { 7, "Locations Discovered" }
        };
        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string functionName = "QueryStat";
            ITES5Referencer newCalledOn = TES5StaticReference.Game;
            TES4FunctionArguments functionArguments = function.getArguments();
            int oldArgValue = (int)functionArguments.Single().getData();
            string newArgValue = statMap[oldArgValue];
            TES4FunctionArguments tes4Arguments = new TES4FunctionArguments();
            tes4Arguments.Add(new TES4String(newArgValue));
            TES5ObjectCallArguments tes5Arguments = objectCallArgumentsFactory.createArgumentList(tes4Arguments, codeScope, globalScope, multipleScriptsScope);
            return objectCallFactory.CreateObjectCall(newCalledOn, functionName, multipleScriptsScope, tes5Arguments);
        }
    }
}
