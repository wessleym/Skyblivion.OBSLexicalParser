using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildSourceFilesCollection<T> : IEnumerable<KeyValuePair<T, string[]>> where T : IBuildTarget
    {
        private readonly Dictionary<T, string[]> sourceFiles = new Dictionary<T, string[]>();
        public void Add(T buildTarget, string[] sourceFiles)
        {
            this.sourceFiles.Add(buildTarget, sourceFiles);
        }

        public IEnumerator<KeyValuePair<T, string[]>> GetEnumerator()
        {
            return sourceFiles.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}