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
        public static BuildTargetCollection getCollection(string targetsString, Build build, BuildLogServices buildLogServices)
        {
            string[] targets = targetsString.Split(',');
            BuildTargetCollection collection = new BuildTargetCollection();
            foreach (var target in targets)
            {
                collection.add(get(target, build, buildLogServices));
            }
            return collection;
        }

        public static BuildTarget get(string target, Build build, BuildLogServices buildLogServices)
        {
            switch (target)
            {
                case BuildTarget.BUILD_TARGET_STANDALONE:
                    {
                        StandaloneParsingService standaloneParsingService = new StandaloneParsingService(new SyntaxErrorCleanParser(new TES4OBScriptGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_STANDALONE, TES5TypeFactory.TES4Prefix, build, buildLogServices.MetadataLogService, new Standalone.TranspileCommand(standaloneParsingService), new Standalone.CompileCommand(), new Standalone.ASTCommand(), new Standalone.BuildScopeCommand(standaloneParsingService), new Standalone.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_TIF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_TIF, "", build, buildLogServices.MetadataLogService, new TIF.TranspileCommand(fragmentsParsingService), new TIF.CompileCommand(), new TIF.ASTCommand(), new TIF.BuildScopeCommand(), new TIF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_PF:
                    {
                        return new BuildTarget(BuildTarget.BUILD_TARGET_PF, "", build, buildLogServices.MetadataLogService, new PF.TranspileCommand(), new PF.CompileCommand(), new PF.ASTCommand(), new PF.BuildScopeCommand(), new PF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_QF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        ESMAnalyzer analyzer = new ESMAnalyzer(DataDirectory.TES4GameFileName);
                        TES5TypeInferencer typeInferencer = new TES5TypeInferencer(analyzer, BuildTarget.StandaloneSourcePath);
                        TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
                        TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
                        TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory);
                        TES5VariableAssignationFactory assignationFactory = new TES5VariableAssignationFactory(referenceFactory);
                        TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory, assignationFactory, objectPropertyFactory, analyzer, typeInferencer, buildLogServices.MetadataLogService);
                        TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
                        TES5ValueFactoryFunctionFiller.fillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, assignationFactory, objectPropertyFactory, analyzer, typeInferencer, buildLogServices.MetadataLogService);
                        TES5BranchFactory branchFactory = new TES5BranchFactory(valueFactory);
                        TES5VariableAssignationFactory variableAssignationFactory = new TES5VariableAssignationFactory(referenceFactory);
                        return new BuildTarget(BuildTarget.BUILD_TARGET_QF, "", build, buildLogServices.MetadataLogService, new QF.TranspileCommand(fragmentsParsingService), new QF.CompileCommand(), new QF.ASTCommand(), new QF.BuildScopeCommand(), new QF.WriteCommand(new QFFragmentFactory(buildLogServices.MappedTargetsLogService, new ObjectiveHandlingFactory(branchFactory, variableAssignationFactory, referenceFactory))));
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown target " + target);
                    }
            }
        }
    }
}