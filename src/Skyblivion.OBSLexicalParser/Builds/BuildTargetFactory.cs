using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.Builds
{
    static class BuildTargetFactory
    {
        public static BuildTargetCollection GetCollection(string targetsString, Build build, BuildLogServices buildLogServices, bool loadESMAnalyzerLazily, out ESMAnalyzer esmAnalyzer, out TES5TypeInferencer typeInferencer)
        {
            esmAnalyzer = new ESMAnalyzer(loadESMAnalyzerLazily);
            typeInferencer = new TES5TypeInferencer(esmAnalyzer/*, BuildTarget.StandaloneSourcePath*/);
            string[] targets = targetsString.Split(',');
            BuildTargetCollection collection = new BuildTargetCollection();
            foreach (var target in targets)
            {
                collection.Add(Get(target, build, buildLogServices, esmAnalyzer, typeInferencer));
            }
            return collection;
        }

        public static BuildTarget Get(string target, Build build, BuildLogServices buildLogServices, ESMAnalyzer esmAnalyzer, TES5TypeInferencer typeInferencer)
        {
            switch (target)
            {
                case BuildTarget.BUILD_TARGET_STANDALONE:
                    {
                        StandaloneParsingService standaloneParsingService = new StandaloneParsingService(new SyntaxErrorCleanParser(new TES4OBScriptGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_STANDALONE, TES5TypeFactory.TES4Prefix, build, new Standalone.TranspileCommand(standaloneParsingService, buildLogServices.MetadataLogService, esmAnalyzer), new Standalone.CompileCommand(), new Standalone.ASTCommand(), new Standalone.BuildScopeCommand(standaloneParsingService, esmAnalyzer, new TES5PropertyFactory(esmAnalyzer)), new Standalone.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_TIF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_TIF, "", build, new TIF.TranspileCommand(fragmentsParsingService, buildLogServices.MetadataLogService, esmAnalyzer), new TIF.CompileCommand(), new TIF.ASTCommand(), new TIF.BuildScopeCommand(new TES5PropertyFactory(esmAnalyzer)), new TIF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_PF:
                    {
                        return new BuildTarget(BuildTarget.BUILD_TARGET_PF, "", build, new PF.TranspileCommand(), new PF.CompileCommand(), new PF.ASTCommand(), new PF.BuildScopeCommand(), new PF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_QF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
                        TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
                        TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory, esmAnalyzer);
                        TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory, esmAnalyzer);
                        TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
                        TES5ValueFactoryFunctionFiller.FillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, objectPropertyFactory, esmAnalyzer, buildLogServices.MetadataLogService);
                        TES5VariableAssignationConversionFactory assignationConversionFactory = new TES5VariableAssignationConversionFactory(objectCallFactory, referenceFactory, valueFactory, typeInferencer);
                        TES5ReturnFactory returnFactory = new TES5ReturnFactory(objectCallFactory);
                        TES5ChainedCodeChunkFactory chainedCodeChunkFactory = new TES5ChainedCodeChunkFactory(valueFactory, returnFactory, assignationConversionFactory);
                        TES5FragmentFactory fragmentFactory = new TES5FragmentFactory(chainedCodeChunkFactory);
                        ObjectiveHandlingFactory objectiveHandlingFactory = new ObjectiveHandlingFactory(objectCallFactory);
                        QFFragmentFactory qfFragmentFactory = new QFFragmentFactory(buildLogServices.MappedTargetsLogService, objectiveHandlingFactory);
                        QF.WriteCommand writeCommand = new QF.WriteCommand(qfFragmentFactory);
                        return new BuildTarget(BuildTarget.BUILD_TARGET_QF, "", build, new QF.TranspileCommand(fragmentsParsingService, fragmentFactory), new QF.CompileCommand(), new QF.ASTCommand(), new QF.BuildScopeCommand(new TES5PropertyFactory(esmAnalyzer)), writeCommand);
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown target " + target);
                    }
            }
        }
    }
}