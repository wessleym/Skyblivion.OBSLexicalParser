using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    class ProgressWriter
    {
        private readonly string label;
        private int total;
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

        public void Write(string? progress)
        {
            Console.Write("\r" + label + (progress != null ? ":  " + progress : "..."));
        }

        public void ClearByPreviousProgress(string progress)
        {
            Write(string.Join("", progress.Select(s => " ")));
        }

        private void WritePercent()
        {
            int newPercent = (int)Math.Floor((float)current / total * 100);
            if (newPercent != percent)
            {
                percent = newPercent;
                Write(newPercent.ToString() + "%");
            }
        }

        public void IncrementAndWrite(int incrementCount)
        {
            current += incrementCount;
            WritePercent();
        }
        public void IncrementAndWrite()
        {
            IncrementAndWrite(1);
        }

        public void ModifyTotalAndWrite(int totalAddend)
        {
            total += totalAddend;
            WritePercent();
        }

        public void WriteLast()
        {
            stopwatch.Stop();
            Write("Complete (" + stopwatch.ElapsedMilliseconds + " ms)");
            Console.WriteLine();
        }

        public static void Within(string label, int total, Action<ProgressWriter> action)
        {
            ProgressWriter progressWriter = new ProgressWriter(label, total);
            action(progressWriter);
            progressWriter.WriteLast();
        }

        public static void Loop<T>(string label, IReadOnlyCollection<T> items, Action<T> action)
        {
            Within(label, items.Count, pw =>
            {
                foreach (var item in items)
                {
                    action(item);
                    pw.IncrementAndWrite();
                }
            });
        }
    }
}
