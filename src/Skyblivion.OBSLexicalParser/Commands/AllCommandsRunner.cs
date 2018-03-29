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
            new TestStageMap()
        };

        public static void Run()
        {
            Console.WriteLine("Ensure you've placed all relevant data files in a Data folder in the same directory as this executable:");
            Console.WriteLine("    Data" + Path.DirectorySeparatorChar + "Build" + Path.DirectorySeparatorChar);
            Console.WriteLine("    Data" + Path.DirectorySeparatorChar + "BuildTargets" + Path.DirectorySeparatorChar);
            Console.WriteLine("    Data" + Path.DirectorySeparatorChar + "Compiler" + Path.DirectorySeparatorChar);
            Console.WriteLine("    Data" + Path.DirectorySeparatorChar + "Oblivion.esm");
            Console.WriteLine("Type a number to run a command:");
            for (int i = 0; i < commands.Length; i++)
            {
                Console.WriteLine((i + 1).ToString() + ".  " + commands[i].FriendlyName);
            }
            int commandNumber = GetCommandNumberFromUser(commands.Select((c, i) => i + 1).ToArray());
            int commandIndex = commandNumber - 1;
            LPCommand command = commands[commandIndex];
            Console.Clear();
            Console.WriteLine("Running " + command.FriendlyName + ":");
            command.execute();
            Console.WriteLine("Press any key to exit.");
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
