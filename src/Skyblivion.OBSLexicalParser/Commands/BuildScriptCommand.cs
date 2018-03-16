using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class BuildScriptCommand : LPCommand
    {
        protected BuildScriptCommand()
        {
            Name = "skyblivion:parser:buildScript";
            Description = "Create artifact from OBScript source";
            Input.AddArgument(new LPCommandArgument("scriptName", "Script name"));
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        protected void execute(LPCommandInput input, TES5GlobalScope globalScope, TES5MultipleScriptsScope compilingScope)
        {//WTM:  Change:  Below, transpileJob.run requires TES5GlobalScope and TES5MultipleScriptsScope.  I added them above.
            set_time_limit(60);
            try
            {
                string targets = input.GetArgumentValue("targets");
                string scriptName = input.GetArgumentValue("scriptName");
                string buildPath = input.GetArgumentValue("buildPath");
                Build build = new Build(buildPath);
                BuildTargetCollection buildTargets = BuildTargetFactory.getCollection(targets, build);
                if (!buildTargets.canBuild())
                {
                    Console.WriteLine("Targets current build dir not clean, archive them manually or run ./clean.sh.");
                    return;
                }

                try
                {
                    TranspileScriptJob transpileJob = new TranspileScriptJob(buildTargets, scriptName);
                    transpileJob.run(globalScope, compilingScope);
                }
                catch (ConversionException e)
                {
                    Console.WriteLine("Exception occured.\r\n" + e.GetType().FullName + "\r\n" + e.Message);
                    return;
                }

                Console.WriteLine("Preparing build workspace...");
                /*
                 *
                 * @TODO - Create a factory that will provide a PrepareWorkspaceJob based on running system, so we can provide a
                 * native implementation for Windows
                 */
                PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(buildTargets);
                prepareCommand.run();
                Console.WriteLine("Workspace prepared...");
                CompileScriptJob compileJob = new CompileScriptJob(buildTargets, build.getCompileLogPath());
                compileJob.run();
                Console.WriteLine("Build completed.");
                string compileLog = File.ReadAllText(build.getCompileLogPath());
                Console.WriteLine(compileLog);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("LogicException " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }
    }
}