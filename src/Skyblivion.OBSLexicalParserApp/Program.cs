using Skyblivion.OBSLexicalParser.Commands;

/*
For now, this project targets .NET Framework.
When targetting .NET Core, it consumes almost twice the memory when processing Oblivion.esm.
Otherwise, performance seems unchanged.
*/
namespace Skyblivion.OBSLexicalParserApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AllCommandsRunner.Run();
        }
    }
}
