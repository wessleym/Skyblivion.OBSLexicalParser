using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPlayerInSEWorldFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly CellToLocationFinder locationFinder;
        public const string SEWorldLocationPropertyName = "SEWorldLocation";
        public GetPlayerInSEWorldFactory(TES5ObjectCallFactory objectCallFactory, CellToLocationFinder locationFinder)
        {
            this.objectCallFactory = objectCallFactory;
            this.locationFinder = locationFinder;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string seWorldLocationEditorID = TES5TypeFactory.TES4Prefix + SEWorldLocationPropertyName;
            int seWorldLocationTES5FormID = locationFinder.GetLocationFormIDByLocationEditorID(seWorldLocationEditorID);
            TES5Property? seWorldLocationProperty = globalScope.TryGetPropertyByName(SEWorldLocationPropertyName);
            if (seWorldLocationProperty == null)
            {
                seWorldLocationProperty = TES5PropertyFactory.ConstructWithTES5FormID(SEWorldLocationPropertyName, TES5BasicType.T_LOCATION, SEWorldLocationPropertyName, seWorldLocationTES5FormID);
                globalScope.AddProperty(seWorldLocationProperty);
            }
            ITES5Referencer seWorldLocationReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(seWorldLocationProperty);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { seWorldLocationReference };
            TES5ObjectCall isInLocation = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToPlayer(globalScope), "IsInLocation", arguments);
            return isInLocation;
        }
    }
}