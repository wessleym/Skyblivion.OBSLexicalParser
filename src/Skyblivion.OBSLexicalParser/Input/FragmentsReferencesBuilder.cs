using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Input
{
    class FragmentsReferencesBuilder
    {
        private readonly ESMAnalyzer esmAnalyzer;
        public FragmentsReferencesBuilder(ESMAnalyzer esmAnalyzer)
        {
            this.esmAnalyzer = esmAnalyzer;
        }
        
        public TES4VariableDeclarationList BuildVariableDeclarationList(string path, TES5FragmentType fragmentType)
        {
            TES4VariableDeclarationList list = new TES4VariableDeclarationList();
            if (!File.Exists(path))
            {
                return list;
            }
            string fileNameNoExt = Path.GetFileNameWithoutExtension(path);
            var scroRecords = TES5ReferenceFactory.GetTypesFromSCRO(esmAnalyzer, fileNameNoExt, fragmentType);
            string[] references = File.ReadAllLines(path);
            foreach (var reference in references)
            {
                string trimmedReference = reference.Trim();
                if (trimmedReference=="")
                {
                    continue;
                }
                var scroReference = scroRecords[trimmedReference];
                list.Add(new TES4VariableDeclaration(trimmedReference, TES4Type.T_REF, formIDs: scroReference.Key, tes5Type: scroReference.Value));
            }
            return list;
        }
    }
}