using System;
using System.Diagnostics;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class ProgressWriter
    {
        private string label;
        private int total;
        private int current;
        private Stopwatch stopwatch;
        public ProgressWriter(string label, int total)
        {
            this.label = label;
            this.total = total;
            current = 0;
            Write(null);
            stopwatch = Stopwatch.StartNew();
        }

        public void Write(string progress)
        {
            Console.Write("\r" + label + (progress != null ? ":  " + progress : "..."));
        }

        public void IncrementAndWrite()
        {
            current++;
            int percent = (int)Math.Floor(((float)current / total) * 100);
            Write(percent.ToString() + "%");
        }

        public void WriteLast()
        {
            stopwatch.Stop();
            Write("Complete (" + stopwatch.ElapsedMilliseconds + " ms)");
            Console.WriteLine();
        }
    }
}
