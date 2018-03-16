using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildSourceFilesCollection : IEnumerable<KeyValuePair<string, string[]>>
    {
        private Dictionary<string, string[]> sourceFiles = new Dictionary<string, string[]>();
        public void add(BuildTarget buildTarget, string[] sourceFiles)
        {
            if (!this.sourceFiles.ContainsKey(buildTarget.getTargetName()))
            {
                this.sourceFiles[buildTarget.getTargetName()] = new string[] { };
            }

            this.sourceFiles[buildTarget.getTargetName()] = this.sourceFiles[buildTarget.getTargetName()].Concat(sourceFiles).Distinct().ToArray();
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