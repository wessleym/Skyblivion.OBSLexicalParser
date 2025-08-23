using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.DI;
using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    static class BuildTargetFactory
    {
        public const string StandaloneName = "Standalone";
        public const string TIFName = "TIF";
        public const string QFName = "QF";
        public const string PFName = "PF";
        public const string DefaultNames = StandaloneName + "," + TIFName + "," + QFName;

        public static BuildTarget[] ParseCollection(string names, Build build)
        {
            return names.Split(',').Select(n => Construct(n, build)).ToArray();
        }

        public static BuildTargetSimple[] GetCollection(IEnumerable<BuildTarget> buildTargets)
        {
            return buildTargets.Select(bt => ConstructSimple(bt)).ToArray();
        }

        public static BuildTargetAdvancedCollection GetCollection(Build build, IEnumerable<BuildTargetSimple> buildTargets)
        {
            BuildLogServiceCollection buildLogServices = BuildLogServiceCollection.DeleteAndStartNewFiles(build);
            TES5ScriptHeaderFactory scriptHeaderFactory = new TES5ScriptHeaderFactory();
            ESMAnalyzer esmAnalyzer = ESMAnalyzer.Load(scriptHeaderFactory);
            TES5TypeInferencer typeInferencer = new TES5TypeInferencer(esmAnalyzer);
            BuildTargetAdvancedCollection collection = new BuildTargetAdvancedCollection(buildLogServices, scriptHeaderFactory, esmAnalyzer);
            foreach (var target in buildTargets)
            {
                collection.Add(ConstructAdvanced(target, buildLogServices, scriptHeaderFactory, esmAnalyzer, typeInferencer));
            }
            return collection;
        }

        public static BuildTarget Construct(string name, Build build)
        {
            switch (name)
            {
                case StandaloneName:
                    {
                        return new BuildTarget(StandaloneName, TES5TypeFactory.TES4Prefix, build);
                    }

                case TIFName:
                    {
                        return new BuildTarget(TIFName, TES5TypeFactory.TES4_Prefix, build);
                    }

                case PFName:
                    {
                        return new BuildTarget(PFName, "", build);
                    }

                case QFName:
                    {
                        return new BuildTarget(QFName, TES5TypeFactory.TES4_Prefix, build);
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown target " + name);
                    }
            }
        }

        public static BuildTargetSimple ConstructSimple(BuildTarget buildTarget)
        {
            switch (buildTarget.Name)
            {
                case StandaloneName:
                    {
                        return new BuildTargetSimple(buildTarget, new Standalone.CompileCommand(), new Standalone.ASTCommand());
                    }

                case TIFName:
                    {
                        return new BuildTargetSimple(buildTarget, new TIF.CompileCommand(), new TIF.ASTCommand());
                    }

                case PFName:
                    {
                        return new BuildTargetSimple(buildTarget, new PF.CompileCommand(), new PF.ASTCommand());
                    }

                case QFName:
                    {
                        return new BuildTargetSimple(buildTarget, new QF.CompileCommand(), new QF.ASTCommand());
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown target " + buildTarget.Name);
                    }
            }
        }

        public static BuildTargetAdvanced ConstructAdvanced(BuildTargetSimple buildTarget, BuildLogServiceCollection buildLogServices, TES5ScriptHeaderFactory scriptHeaderFactory, ESMAnalyzer esmAnalyzer, TES5TypeInferencer typeInferencer)
        {
            switch (buildTarget.Name)
            {
                case StandaloneName:
                    {
                        StandaloneParsingService standaloneParsingService = new StandaloneParsingService(new SyntaxErrorCleanParser(new TES4OBScriptGrammar()));
                        return new BuildTargetAdvanced(buildTarget, new Standalone.TranspileCommand(standaloneParsingService, buildLogServices.MetadataLogService, esmAnalyzer), new Standalone.BuildScopeCommand(standaloneParsingService, scriptHeaderFactory, esmAnalyzer, new TES5PropertyFactory(esmAnalyzer)), new Standalone.WriteCommand());
                    }

                case TIFName:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        return new BuildTargetAdvanced(buildTarget, new TIF.TranspileCommand(fragmentsParsingService, buildLogServices.MetadataLogService, esmAnalyzer), new TIF.BuildScopeCommand(new TES5PropertyFactory(esmAnalyzer), new FragmentsReferencesBuilder(esmAnalyzer), scriptHeaderFactory), new TIF.WriteCommand());
                    }

                case PFName:
                    {
                        return new BuildTargetAdvanced(buildTarget, new PF.TranspileCommand(), new PF.BuildScopeCommand(), new PF.WriteCommand());
                    }

                case QFName:
                    {
                        FragmentsParsingService fragmentsParsingService = new FragmentsParsingService(new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar()));
                        TES5ObjectCallFactory objectCallFactory = new TES5ObjectCallFactory(typeInferencer);
                        TES5ObjectPropertyFactory objectPropertyFactory = new TES5ObjectPropertyFactory(typeInferencer);
                        TES5ReferenceFactory referenceFactory = new TES5ReferenceFactory(objectCallFactory, objectPropertyFactory, esmAnalyzer);
                        TES5ValueFactory valueFactory = new TES5ValueFactory(objectCallFactory, referenceFactory, esmAnalyzer);
                        TES5ObjectCallArgumentsFactory objectCallArgumentsFactory = new TES5ObjectCallArgumentsFactory(valueFactory);
                        TES5ValueFactoryFunctionFiller.FillFunctions(valueFactory, objectCallFactory, objectCallArgumentsFactory, referenceFactory, objectPropertyFactory, buildLogServices.MetadataLogService, esmAnalyzer);
                        TES5VariableAssignationConversionFactory assignationConversionFactory = new TES5VariableAssignationConversionFactory(objectCallFactory, referenceFactory, valueFactory);
                        TES5ReturnFactory returnFactory = new TES5ReturnFactory(objectCallFactory);
                        TES5ChainedCodeChunkFactory chainedCodeChunkFactory = new TES5ChainedCodeChunkFactory(valueFactory, returnFactory, assignationConversionFactory);
                        TES5FragmentFactory fragmentFactory = new TES5FragmentFactory(chainedCodeChunkFactory);
                        ObjectiveHandlingFactory objectiveHandlingFactory = new ObjectiveHandlingFactory(objectCallFactory);
                        QFFragmentFactory qfFragmentFactory = new QFFragmentFactory(buildLogServices.MappedTargetsLogService, objectiveHandlingFactory, scriptHeaderFactory, esmAnalyzer);
                        QF.WriteCommand writeCommand = new QF.WriteCommand(qfFragmentFactory);
                        return new BuildTargetAdvanced(buildTarget, new QF.TranspileCommand(fragmentsParsingService, fragmentFactory), new QF.BuildScopeCommand(new TES5PropertyFactory(esmAnalyzer), new FragmentsReferencesBuilder(esmAnalyzer), scriptHeaderFactory), writeCommand);
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown target " + buildTarget.Name);
                    }
            }
        }
    }
}
