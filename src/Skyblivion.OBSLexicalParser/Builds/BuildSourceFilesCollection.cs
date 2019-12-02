using Skyblivion.ESReader.Extensions.IDictionaryExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildSourceFilesCollection : IEnumerable<KeyValuePair<string, string[]>>
    {
        private readonly Dictionary<string, string[]> sourceFiles = new Dictionary<string, string[]>();
        public void Add(BuildTarget buildTarget, string[] sourceFiles)
        {
            string targetName = buildTarget.GetTargetName();
            this.sourceFiles[targetName] = this.sourceFiles.GetWithFallback(targetName, () => new string[] { }).Concat(sourceFiles).Distinct().ToArray();
        }

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            return sourceFiles.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}