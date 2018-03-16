using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service;
using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTargetFactory
    {
        public static BuildTargetCollection getCollection(string targetsString, Build build)
        {
            string[] targets = targetsString.Split(',');
            BuildTargetCollection collection = new BuildTargetCollection();
            foreach (var target in targets)
            {
                collection.add(get(target, build));
            }
            return collection;
        }

        public static BuildTarget get(string target, Build build)
        {
            switch (target)
            {
                case BuildTarget.BUILD_TARGET_STANDALONE:
                    {
                        StandaloneParsingService standaloneParsingService = new StandaloneParsingService(new SyntaxErrorCleanParser(new TES4OBScriptGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_STANDALONE, "TES4", build, new TES5NameTransformer(), new Skyblivion.OBSLexicalParser.Builds.Standalone.TranspileCommand(standaloneParsingService), new Skyblivion.OBSLexicalParser.Builds.Standalone.CompileCommand(), new Skyblivion.OBSLexicalParser.Builds.Standalone.ASTCommand(), new Skyblivion.OBSLexicalParser.Builds.Standalone.BuildScopeCommand(standaloneParsingService), new Skyblivion.OBSLexicalParser.Builds.Standalone.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_TIF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        return new BuildTarget(BuildTarget.BUILD_TARGET_TIF, "", build, new TES5NameTransformer(), new Skyblivion.OBSLexicalParser.Builds.TIF.TranspileCommand(fragmentsParsingService), new Skyblivion.OBSLexicalParser.Builds.TIF.CompileCommand(), new Skyblivion.OBSLexicalParser.Builds.TIF.ASTCommand(), new Skyblivion.OBSLexicalParser.Builds.TIF.BuildScopeCommand(), new Skyblivion.OBSLexicalParser.Builds.TIF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_PF:
                    {
                        return new BuildTarget(BuildTarget.BUILD_TARGET_PF, "", build, new TES5NameTransformer(), new Skyblivion.OBSLexicalParser.Builds.PF.TranspileCommand(), new Skyblivion.OBSLexicalParser.Builds.PF.CompileCommand(), new Skyblivion.OBSLexicalParser.Builds.PF.ASTCommand(), new Skyblivion.OBSLexicalParser.Builds.PF.BuildScopeCommand(), new Skyblivion.OBSLexicalParser.Builds.PF.WriteCommand());
                    }

                case BuildTarget.BUILD_TARGET_QF:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        TypeMapper typeMapper = new TypeMapper();
                        ESMAnalyzer analyzer = new ESMAnalyzer(typeMapper, "Oblivion.esm");
                        TES5PrimitiveValueFactory primitiveValueFactory = new TES5PrimitiveValueFactory();
                        MetadataLogService metadataLogService = new MetadataLogService(build);
                        TES5CodeScopeFactory codeScopeFactory = new TES5CodeScopeFactory();
                        TES5ExpressionFactory expressionFactory = new TES5ExpressionFactory();
                        TES5TypeInferencer typeInferencer = new TES5TypeInferencer(analyzer, "./BuildTargets/Standalone/Source/");
                        TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
                        TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
                        TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory);
                        TES5VariableAssignationFactory assignationFactory = new TES5VariableAssignationFactory(referenceFactory);
                        TES5LocalScopeFactory localScopeFactory = new TES5LocalScopeFactory();
                        TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory, expressionFactory, assignationFactory, objectPropertyFactory, analyzer, primitiveValueFactory, typeInferencer, metadataLogService);
                        TES5ValueFactoryFunctionFiller filler = new TES5ValueFactoryFunctionFiller();
                        TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
                        filler.fillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, expressionFactory, assignationFactory, objectPropertyFactory, analyzer, primitiveValueFactory, typeInferencer, metadataLogService);
                        TES5BranchFactory branchFactory = new TES5BranchFactory(localScopeFactory, codeScopeFactory, valueFactory);
                        TES5VariableAssignationFactory variableAssignationFactory = new TES5VariableAssignationFactory(referenceFactory);
                        return new BuildTarget(BuildTarget.BUILD_TARGET_QF, "", build, new TES5NameTransformer(), new Skyblivion.OBSLexicalParser.Builds.QF.TranspileCommand(fragmentsParsingService), new Skyblivion.OBSLexicalParser.Builds.QF.CompileCommand(), new Skyblivion.OBSLexicalParser.Builds.QF.ASTCommand(), new Skyblivion.OBSLexicalParser.Builds.QF.BuildScopeCommand(), new Skyblivion.OBSLexicalParser.Builds.QF.WriteCommand(new QFFragmentFactory(new MappedTargetsLogService(build), new ObjectiveHandlingFactory(new TES5FragmentFunctionScopeFactory(), codeScopeFactory, localScopeFactory, branchFactory, variableAssignationFactory, referenceFactory, expressionFactory))));
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown target " + target);
                    }
            }
        }
    }
}