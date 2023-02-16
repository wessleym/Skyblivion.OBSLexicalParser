using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Input
{
    class FragmentsReferencesBuilder
    {
        private readonly ESMAnalyzer esmAnalyzer;
        public FragmentsReferencesBuilder(ESMAnalyzer esmAnalyzer)
        {
            this.esmAnalyzer = esmAnalyzer;
        }

        public IEnumerable<TES4VariableDeclaration> BuildVariableDeclarationList(string sourcePath, string scriptName, TES5FragmentType fragmentType)
        {
            var scroRecords = TES5ReferenceFactory.GetTypesFromSCRO(esmAnalyzer, scriptName, fragmentType);
            string[] references =
#if USEFILESINSTEADOFESM
                GetReferences(sourcePath, scriptName)
#else
                scroRecords
#if !NEWBT
                .Where(r => r.Value.Key != TES5PlayerReference.FormID)
#endif
                .Select(r => r.Key)
#endif
                .ToArray();
            foreach (var reference in references)
            {
                var scroReference = scroRecords[reference];
                yield return new TES4VariableDeclaration(reference, TES4Type.T_REF, formID: scroReference.Key, tes5Type: scroReference.Value);
            }
        }

        private static IEnumerable<string> GetReferences(string sourcePath, string scriptName)
        {
            string? sourceDirectory = Path.GetDirectoryName(sourcePath);
            if (sourceDirectory == null) { throw new NullableException(nameof(sourceDirectory)); }
            string referencesPath = Path.Combine(sourceDirectory, scriptName + ".references");
            if (!File.Exists(referencesPath))
            {
                return Array.Empty<string>();
            }
            return File.ReadAllLines(referencesPath).Select(l => l.Trim()).Where(l => l != "");
        }
    }
}