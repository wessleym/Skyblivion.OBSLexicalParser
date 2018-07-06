using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.TES5.AST;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    public class FormattingStreamWriter : StreamWriter
    {
        private readonly IFormatProvider formatProvider;

        public FormattingStreamWriter(string path, IFormatProvider formatProvider)
            : base(path)
        {
            this.formatProvider = formatProvider;
        }
        public override IFormatProvider FormatProvider
        {
            get
            {
                return this.formatProvider;
            }
        }
    }

    class WriteCommandBase
    {
        private static void Write(string path, IList<string> output)
        {
            using (StreamWriter writer = new FormattingStreamWriter(path, CultureInfo.InvariantCulture))
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
