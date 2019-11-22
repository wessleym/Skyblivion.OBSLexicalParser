using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;

namespace Skyblivion.OBSLexicalParser.Builds
{
    static class BuildTargetFactory
    {
        public static BuildTargetCollection GetCollection(string targetsString, Build build, BuildLogServices buildLogServices, bool loadESMAnalyzerLazily)
        {
            string[] targets = targetsString.Split(',');
            BuildTargetCollection collection = new BuildTargetCollection();
            foreach (var target in targets)
            {
                collection.Add(Get(target, build, buildLogServices, loadESMAnalyzerLazily));
            }
            return collection;
        }

        public static BuildTarget Get(string target, Build build, BuildLogServices buildLogServices, bool loadESMAnalyzerLazily)
        {
            switch (target)
            {
                case BuildTarget.BUILD_TARGET_STANDALONE:
                    {
                        StandaloneParsingService standaloneParsingService = new StandaloneParsingService(new SyntaxErrorCleanParser(new TES4OBScriptGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_STANDALONE, TES5TypeFactory.TES4Prefix, build, new Standalone.TranspileCommand(standaloneParsingService, buildLogServices.MetadataLogService, loadESMAnalyzerLazily), new Standalone.CompileCommand(), new Standalone.ASTCommand(), new Standalone.BuildScopeCommand(standaloneParsingService, loadESMAnalyzerLazily), new Standalone.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_TIF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_TIF, "", build, new TIF.TranspileCommand(fragmentsParsingService, buildLogServices.MetadataLogService, loadESMAnalyzerLazily), new TIF.CompileCommand(), new TIF.ASTCommand(), new TIF.BuildScopeCommand(), new TIF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_PF:
                    {
                        return new BuildTarget(BuildTarget.BUILD_TARGET_PF, "", build, new PF.TranspileCommand(), new PF.CompileCommand(), new PF.ASTCommand(), new PF.BuildScopeCommand(), new PF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_QF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        ESMAnalyzer analyzer = new ESMAnalyzer(loadESMAnalyzerLazily, DataDirectory.TES4GameFileName);
                        TES5TypeInferencer typeInferencer = new TES5TypeInferencer(analyzer, BuildTarget.StandaloneSourcePath);
                        TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
                        TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
                        TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory);
                        TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory);
                        TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
                        TES5ValueFactoryFunctionFiller.FillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, objectPropertyFactory, analyzer, typeInferencer, buildLogServices.MetadataLogService);
                        QFFragmentFactory qfFragmentFactory = new QFFragmentFactory(buildLogServices.MappedTargetsLogService);
                        QF.WriteCommand writeCommand = new QF.WriteCommand(qfFragmentFactory);
                        return new BuildTarget(BuildTarget.BUILD_TARGET_QF, "", build, new QF.TranspileCommand(fragmentsParsingService, build, buildLogServices.MetadataLogService, loadESMAnalyzerLazily), new QF.CompileCommand(), new QF.ASTCommand(), new QF.BuildScopeCommand(), writeCommand);
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown target " + target);
                    }
            }
        }
    }
}