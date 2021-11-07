using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.TES5.AST;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class WriteCommandBase
    {
        //This functions the same as File.WriteAllLines except that it doesn't leave a trailing \r\n.
        private static void Write(string path, IReadOnlyList<string> output)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < output.Count; ++i)
                {
                    string o = output[i];
                    writer.Write(o);
                    if (i < output.Count - 1)
                    {
                        writer.Write("\r\n");
                    }
                }
            }
        }

        protected static void Write(IEnumerable<TES5Target> targets, ProgressWriter progressWriter)
        {
            foreach (TES5Target target in targets)
            {
                string[] outputString = target.Script.Output.ToArray();
                Write(target.OutputPath, outputString);
                progressWriter.IncrementAndWrite();
            }
        }
    }
}
