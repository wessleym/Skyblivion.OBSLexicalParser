using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.TES5.AST;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class WriteCommandBase
    {
        private static void Write(string path, IList<string> output)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < output.Count; ++i)
                {
                    string o = output[i];
                    writer.Write(o);
                    if(i < output.Count - 1)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        protected static void Write(IEnumerable<TES5Target> targets, ProgressWriter progressWriter)
        {
            foreach (TES5Target target in targets)
            {
                string[] outputString = target.getScript().output().ToArray();
                Write(target.getOutputPath(), outputString);
                progressWriter.IncrementAndWrite();
            }
        }
    }
}
