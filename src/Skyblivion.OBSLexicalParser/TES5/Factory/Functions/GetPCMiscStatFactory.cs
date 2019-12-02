﻿using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
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
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        private readonly TES5StaticReferenceFactory staticReferenceFactory;
        public GetPCMiscStatFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5StaticReferenceFactory staticReferenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.staticReferenceFactory = staticReferenceFactory;
        }

        private static readonly Dictionary<int, string> statMap = new Dictionary<int, string>()
        {
            { 7, "Locations Discovered" }
        };
        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string functionName = "QueryStat";
            ITES5Referencer newCalledOn = staticReferenceFactory.Game;
            TES4FunctionArguments functionArguments = function.Arguments;
            int oldArgValue = (int)functionArguments.Single().Data;
            string newArgValue = statMap[oldArgValue];
            TES4FunctionArguments tes4Arguments = new TES4FunctionArguments() { new TES4String(newArgValue) };
            TES5ObjectCallArguments tes5Arguments = objectCallArgumentsFactory.CreateArgumentList(tes4Arguments, codeScope, globalScope, multipleScriptsScope);
            return objectCallFactory.CreateObjectCall(newCalledOn, functionName, tes5Arguments);
        }
    }
}
