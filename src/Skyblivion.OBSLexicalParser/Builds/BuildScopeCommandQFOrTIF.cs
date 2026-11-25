using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildScopeCommandQFOrTIF : IBuildScopeCommand
    {
        private readonly TES5PropertyFactory propertyFactory;
        private readonly FragmentsReferencesBuilder fragmentsReferencesBuilder;
        private readonly TES5ScriptHeaderFactory scriptHeaderFactory;
        private readonly TES5BasicType scriptType;
        private readonly string scriptNamePrefix;
        private readonly TES5FragmentType fragmentType;
        public BuildScopeCommandQFOrTIF(TES5PropertyFactory propertyFactory, FragmentsReferencesBuilder fragmentsReferencesBuilder, TES5ScriptHeaderFactory scriptHeaderFactory, TES5BasicType scriptType, string scriptNamePrefix, TES5FragmentType fragmentType)
        {
            this.propertyFactory = propertyFactory;
            this.fragmentsReferencesBuilder = fragmentsReferencesBuilder;
            this.scriptHeaderFactory = scriptHeaderFactory;
            this.scriptType = scriptType;
            this.scriptNamePrefix = scriptNamePrefix;
            this.fragmentType = fragmentType;
        }

        private static readonly Regex questAbbreviation = new Regex("^([A-Za-z]{1,4})([0-9]{2})([A-Za-z]*)$", RegexOptions.Compiled);
        private static readonly Regex questAbbreviation2 = new Regex("^(da(rk)?|arena)([0-9]{2})?([A-Za-z]+[0-9]?)$", RegexOptions.Compiled);
        public static string ImproveScriptNameCase(string scriptName)
        {
            string[] scriptNameSegments = scriptName.Split('_');
            static string UpperSubSegment(Group subSegment)
            {
                return subSegment.Length <= 2 || subSegment.Value == "fgc" ? subSegment.Value.ToUpper() : subSegment.Value.UpperFirst();
            }
            return string.Join("_", scriptNameSegments.Select((s, i) =>
            {
                if (i == 0 || i == 2) { return s.ToUpper(); }
                if (i == 1 && s != "" && !s.Any(char.IsUpper))
                {
                    var questAbbreviationMatch = questAbbreviation.Match(s);
                    if (questAbbreviationMatch.Success)
                    {
                        return
                            UpperSubSegment(questAbbreviationMatch.Groups[1]) +
                            questAbbreviationMatch.Groups[2].Value +
                            UpperSubSegment(questAbbreviationMatch.Groups[3]);
                    }
                    else
                    {
                        var questAbbreviation2Match = questAbbreviation2.Match(s);
                        if (questAbbreviation2Match.Success)
                        {
                            return
                                UpperSubSegment(questAbbreviation2Match.Groups[1]) +
                                questAbbreviation2Match.Groups[3].Value +
                                UpperSubSegment(questAbbreviation2Match.Groups[4]);
                        }
                        else if (s.StartsWith("house") && s.All(char.IsLetter)) { return s.Substring(0, 5).UpperFirst() + s.Substring(5).UpperFirst(); }
                        else if (s == "e3") { return "E3"; }
                        else if (s.StartsWith("se")) { return s.Substring(0, 3).ToUpper() + s.Substring(3); }
                        else { return s.UpperFirst(); }
                    }
                }
                return s;
            }));
        }

        public TES5GlobalScope Build(string sourcePath, TES5GlobalVariableCollection globalVariables)
        {
            string scriptName = ImproveScriptNameCase(Path.GetFileNameWithoutExtension(sourcePath));
            TES4VariableDeclaration[] variableDeclarations = fragmentsReferencesBuilder.BuildVariableDeclarationList(sourcePath, scriptName, fragmentType).ToArray();
            //Create the header.
            TES5ScriptHeader scriptHeader = scriptHeaderFactory.GetFromCacheOrConstructByBasicType(scriptName, scriptType, scriptNamePrefix, true);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            propertyFactory.CreateAndAddProperties(variableDeclarations, globalScope, globalVariables);
            return globalScope;
        }
    }
}
