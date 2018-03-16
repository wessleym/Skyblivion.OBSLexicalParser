using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class TranspileCommand : ITranspileCommand
    {
        private FragmentsParsingService parsingService;
        private TES4ToTES5ASTTIFFragmentConverter converter;
        private FragmentsReferencesBuilder fragmentsReferenceBuilder;
        public TranspileCommand(FragmentsParsingService fragmentsParsingService)
        {
            this.parsingService = fragmentsParsingService;
        }

        public void initialize(Build build)
        {
            converter = TES4ToTES5ASTTIFFragmentConverterFactory.GetConverter(build);
            this.fragmentsReferenceBuilder = new FragmentsReferencesBuilder();
        }

        public TES5Target transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4CodeChunks AST = this.parsingService.parseScript(sourcePath);
            TES5Target convertedScript = this.converter.convert(new TES4FragmentTarget(AST, outputPath), globalScope, multipleScriptsScope);
            return convertedScript;
        }
    }
}