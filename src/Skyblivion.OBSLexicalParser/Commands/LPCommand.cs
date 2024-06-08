using Skyblivion.OBSLexicalParser.Data;
using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public abstract class LPCommand
    {
        public string CommandName { get; }
        public string FriendlyName { get; }
        public string? Description { get; }
        protected LPCommandInput Input = new LPCommandInput();
        protected LPCommand(string commandName, string friendlyName, string? description)
        {
            CommandName = commandName;
            FriendlyName = friendlyName;
            Description = description;
        }

        public abstract void Execute();

        protected bool PreExecutionChecks(bool requireESM, bool requireBuildTargets, bool requireGraph, bool requireCompiler)
        {
            if (requireESM)
            {
                string esmPath = DataDirectory.ESMDefaultFilePath;
                if (!File.Exists(esmPath))
                {
                    Console.WriteLine("Please add " + esmPath);
                    return false;
                }
            }
            if (requireBuildTargets)
            {
                string buildTargetsPath = DataDirectory.BuildTargetsPath;
                if (!Directory.Exists(buildTargetsPath))
                {
                    Console.WriteLine("Please add " + buildTargetsPath + " with all of its contents.");
                    return false;
                }
            }
            if (requireGraph)
            {
                string graphPath = DataDirectory.GraphDirectoryPath;
                if (!Directory.Exists(graphPath))
                {
                    Console.WriteLine("Please add " + graphPath + " by running " + BuildInteroperableCompilationGraphs.FriendlyNameConst + ".");
                    return false;
                }
            }
            if (requireCompiler)
            {
                string compilerPath = DataDirectory.CompilerDirectoryPath;
                if (!Directory.Exists(compilerPath))
                {
                    Console.WriteLine("Please add compiler binaries to " + compilerPath + ".");
                    return false;
                }
            }
            Directory.CreateDirectory(DataDirectory.BuildPath);
            return true;
        }
    }
}
