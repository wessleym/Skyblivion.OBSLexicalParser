using Skyblivion.OBSLexicalParser.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public static class AllCommandsRunner
    {
        private static readonly LPCommand[] commands = new LPCommand[]
        {
            new BuildInteroperableCompilationGraphs(),
            new BuildTargetCommand(),
            new TestStageMap(),
            new BuildFileDeleteCommand()
        };

        public static void Run()
        {
            string dataPath = "Data" + Path.DirectorySeparatorChar;
            Console.WriteLine("Ensure you've placed all relevant data files in a Data folder in the same directory as this executable:");
            Console.WriteLine("    " + dataPath + "Build" + Path.DirectorySeparatorChar);
            Console.WriteLine("    " + dataPath + "BuildTargets" + Path.DirectorySeparatorChar);
            Console.WriteLine("    " + dataPath + "Compiler" + Path.DirectorySeparatorChar);
            Console.WriteLine("    " + dataPath + "Graph" + Path.DirectorySeparatorChar + " [Generated from " + BuildInteroperableCompilationGraphs.FriendlyNameConst + "]");
            Console.WriteLine("    " + dataPath + DataDirectory.TES4GameFileName);
            Console.WriteLine("Type a number below to run a command.");
            for (int i = 0; i < commands.Length; i++)
            {
                Console.WriteLine((i + 1).ToString() + ".  " + commands[i].FriendlyName);
            }
            int commandNumber = GetCommandNumberFromUser(commands.Select((c, i) => i + 1).ToArray());
            int commandIndex = commandNumber - 1;
            LPCommand command = commands[commandIndex];
            Console.Clear();
            Console.WriteLine("Running " + command.FriendlyName + ":");
            command.Execute();
            Console.WriteLine("Press any key to exit.");
            while (Console.KeyAvailable) { Console.ReadKey(true); }//Clear buffered keys
            Console.ReadKey(true);
        }

        private static int GetCommandNumberFromUser(IList<int> allowedInputs)
        {
            while (true)
            {
                string input = Console.ReadKey(true).KeyChar.ToString();
                int inputInt;
                if (int.TryParse(input, out inputInt) && allowedInputs.Contains(inputInt))
                {
                    return inputInt;
                }
            }
        }
    }
}
