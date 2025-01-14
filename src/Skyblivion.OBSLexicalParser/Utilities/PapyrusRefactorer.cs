using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    internal class PapyrusRefactorer
    {
        private readonly string directory;
        internal PapyrusRefactorer(string directory)
        {
            this.directory = directory;
        }

        private IEnumerable<string> GetFilePaths(IReadOnlyCollection<string> excludeFileNamesNoExt)
        {
            return Directory.EnumerateFiles(directory).Where(p => !excludeFileNamesNoExt.Any(exclude => Path.GetFileNameWithoutExtension(p).Equals(exclude, StringComparison.Ordinal)));
        }
        private void ReplaceAll(IReadOnlyCollection<string> excludeFileNamesNoExt, Func<string, string> replace)
        {
            foreach (string path in GetFilePaths(excludeFileNamesNoExt))
            {
                string oldContents = File.ReadAllText(path);
                string newContents = replace(oldContents);
                if (oldContents != newContents)
                {
                    File.WriteAllText(path, newContents);
                }
            }
        }

        private void ConvertInstanceMethodToStaticMethod(string oldMethod, string newStaticClass, string newMethod)
        {
            Regex oldRE = new Regex($"""\b(([A-Z0-9_\.\(\)]+?)\.)?{oldMethod}\(([^\)]*)\)""", RegexOptions.IgnoreCase);
            ReplaceAll([newStaticClass], oldContents =>
            {
                return oldRE.Replace(oldContents, m =>
                {
                    string oldCalledOn = m.Groups[2].Value;
                    if (oldCalledOn == newStaticClass) { return m.Value; }
                    string oldArguments = m.Groups[3].Value;
                    string newArguments = (oldCalledOn != "" ? oldCalledOn : "self") + (!string.IsNullOrWhiteSpace(oldArguments) ? ", " + oldArguments : "");
                    return newStaticClass + "." + newMethod + "(" + newArguments + ")";
                });
            });
        }
        private void ConvertInstanceMethodToStaticMethod(string newStaticClass, string method)
        {
            ConvertInstanceMethodToStaticMethod(method, newStaticClass, method);
        }

        private void ConvertStaticMethodToStaticMethod(string oldStaticClass, string oldMethod, string newStaticClass, string newMethod)
        {
            Regex oldRE = new Regex($"""\b{oldStaticClass}\.{oldMethod}\(""", RegexOptions.IgnoreCase);
            ReplaceAll([oldStaticClass, newStaticClass], oldContents =>
            {
                return oldRE.Replace(oldContents, m =>
                {
                    return newStaticClass + "." + newMethod + "(";
                });
            });
        }
        private void ConvertStaticMethodToStaticMethod(string oldStaticClass, string newStaticClass, string method)
        {
            ConvertStaticMethodToStaticMethod(oldStaticClass, method, newStaticClass, method);
        }

        internal void Convert()
        {
            ConvertStaticMethodToStaticMethod("Game", "SKYBGameUtility", "LegacyGetAmountSoldStolen");
            ConvertStaticMethodToStaticMethod("Game", "SKYBGameUtility", "LegacyModAmountSoldStolen");
            ConvertStaticMethodToStaticMethod("Game", "SKYBGameUtility", "LegacyIsPCAMurderer");
            ConvertInstanceMethodToStaticMethod("SKYBObjectReferenceUtility", "ContainsItem");
            ConvertInstanceMethodToStaticMethod("SKYBObjectReferenceUtility", "IsAnimPlaying");
            ConvertInstanceMethodToStaticMethod("SKYBObjectReferenceUtility", "LegacyGetDestroyed");
            ConvertInstanceMethodToStaticMethod("SKYBObjectReferenceUtility", "LegacySay");
            ConvertInstanceMethodToStaticMethod("SKYBObjectReferenceUtility", "LegacySayTo");
            ConvertInstanceMethodToStaticMethod("SKYBQuestUtility", "PrepareForReinitializing");
            ConvertInstanceMethodToStaticMethod("SKYBTimerHelper", "GetPassedGameDays");
            ConvertInstanceMethodToStaticMethod("SKYBTimerHelper", "GetDayOfWeek");
        }
    }
}
