using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Input
{
    static class FragmentsReferencesBuilder
    {
        public static TES4VariableDeclarationList buildVariableDeclarationList(string path)
        {
            TES4VariableDeclarationList list = new TES4VariableDeclarationList();
            if (!File.Exists(path))
            {
                return list;
            }

            string[] references = File.ReadAllLines(path);
            foreach (var reference in references)
            {
                string trimmedReference = reference.Trim();
                if (trimmedReference=="")
                {
                    continue;
                }

                list.add(new TES4VariableDeclaration(trimmedReference, TES4Type.T_REF));
            }

            return list;
        }
    }
}