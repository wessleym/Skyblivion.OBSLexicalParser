using System;
using System.Diagnostics;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class ProgressWriter
    {
        private readonly string label;
        private readonly int total;
        private int current;
        private int percent;
        private readonly Stopwatch stopwatch;
        public ProgressWriter(string label, int total)
        {
            this.label = label;
            this.total = total;
            current = 0;
            percent = 0;
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
            int newPercent = (int)Math.Floor(((float)current / total) * 100);
            if (newPercent != percent)
            {
                percent = newPercent;
                Write(newPercent.ToString() + "%");
            }
        }

        public void WriteLast()
        {
            stopwatch.Stop();
            Write("Complete (" + stopwatch.ElapsedMilliseconds + " ms)");
            Console.WriteLine();
        }
    }
}
