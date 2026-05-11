using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    class TES5GlobalScope
    {
        public TES5ScriptHeader ScriptHeader { get; }
        public List<TES5Property> Properties { get; } = new List<TES5Property>();
        private readonly List<ITES5CodeChunk> propertiesAndComments = new List<ITES5CodeChunk>();
        private readonly List<TES5FunctionCodeBlock> functions = new List<TES5FunctionCodeBlock>();
        public bool QuestHasOnUpdateRegisterForSingleUpdate { get; set; }
        /*
        * TES5GlobalScope constructor.
        */
        public TES5GlobalScope(TES5ScriptHeader scriptHeader)
        {
            this.ScriptHeader = scriptHeader;
            QuestHasOnUpdateRegisterForSingleUpdate = false;
        }

        public bool AnyConditionalProperties => Properties.Any(p => p.Conditional);

        private void AddPropertyOrComment(ITES5CodeChunk propertyOrComment)
        {
            propertiesAndComments.Add(propertyOrComment);
        }

        public bool AddPropertyIfNotExists(TES5Property property)
        {
            if (!Properties.Where(p => p.OriginalName == property.OriginalName).Any())
            {
                this.Properties.Add(property);
                AddPropertyOrComment(property);
                return true;
            }
            return false;
        }

        public void AddProperty(TES5Property property)
        {
            if (!AddPropertyIfNotExists(property))
            {
                throw new ConversionException("Property " + property.Name + " was not unique");
            }
        }

        public void AddComment(TES5Comment comment)
        {
            AddPropertyOrComment(comment);
        }

        public void AddFunction(TES5FunctionCodeBlock functionCodeBlock)
        {
            functions.Add(functionCodeBlock);
        }

        public void AddFunctionIfNotExists(string name, Func<TES5FunctionCodeBlock> functionCodeBlockFactory)
        {
            if (!functions.Where(f => f.BlockName == name).Any())
            {
                AddFunction(functionCodeBlockFactory());
            }
        }

        private static readonly string[] preventFormattedPropertiesAndCommentsScriptNames = ["TES4CharGenQuest", "TES4E3CaptainScript", "TES4GoblinHeadScript", "TES4MG00Script", "TES4MQ04Script", "TES4MQ05Script", "TES4MQ15TortureCage01SCRIPT", "TES4MQ16EndgameBattleScript", "TES4MQ16EndgameTransformationScript", "TES4MS02VelwynScript", "TES4MS38Script", "TES4MS45DarMaScript", "TES4MS94ImperialCaptainScript", "TES4OBMineScript01", "TES4SE02GatekeeperScript", "TES4SE02QuestScript", "TES4SE06SCRIPT", "TES4SE08XedQuestScript", "TES4SE09QuestScript", "TES4SE09VictimScript", "TES4SE11QuestScript", "TES4SE14Script", "TES4SE14VoiceofSheogorathSpell", "TES4SE32AlthelScript", "TES4SE32Script", "TES4SE37Script", "TES4SE38SCRIPT", "TES4SE40QuestScript", "TES4SE41PoolTriggerScript", "TES4SE45Script", "TES4SE46Script", "TES4SEConversationsQuestScript", "TES4SEHerdirScript", "TES4SEHirrusClutumnusSCRIPT", "TES4SEJayredIceVeinsScript", "TES4SEObeliskEnergyBoxSCRIPT", "TES4SEObeliskNewSCRIPT", "TES4SERelmynaVerenimScript", "TES4SESheldenScript", "TES4SEXidPuzButton1", "TES4TandilweScript"];
        private IEnumerable<string> GetFormatttedPropertiesAndComments(bool allowConditionalProperties)
        {
            IEnumerable<string> ungroupedOutput = propertiesAndComments.SelectMany(p => p.Output);
            var properties = propertiesAndComments.OfType<TES5Property>().ToArray();
            if (properties.Length == 0) { return ungroupedOutput; }
            IEnumerable<string> groupedOutput;
            if (preventFormattedPropertiesAndCommentsScriptNames.Contains(this.ScriptHeader.PrefixedScriptName))
            {
                groupedOutput = propertiesAndComments.SelectMany(p => p.Output);
            }
            else
            {
                var commentsOutput = propertiesAndComments.OfType<TES5Comment>().SelectMany(c => c.Output);
                var groupedProperties = properties
                    .GroupBy(p => p.TES5Type.OutputSingle)
                    .OrderBy(g => g.Key)
                    .ToArray();
                var propertiesOutput = groupedProperties
                    .SelectMany((g, i) =>
                    {
                        int propertyNameMaxLength = g.Select(p => p.Name.Length).Max();
                        IEnumerable<string> groupOutputs = g.Select(p => p.GetOutput(propertyNameMaxLength, allowConditionalProperties)).OrderBy(o => o);
                        if (i < groupedProperties.Length - 1) { groupOutputs = groupOutputs.Concat(new string[] { "" }); }
                        return groupOutputs;
                    });
                groupedOutput = commentsOutput.Concat(propertiesOutput);
            }
            return groupedOutput;
        }

        public IEnumerable<string> GetOutput(bool allowConditionalProperties)
        {
            return (propertiesAndComments.Count > 0 || functions.Count > 0 ? new string[] { "" } : []).Concat(GetFormatttedPropertiesAndComments(allowConditionalProperties)).Concat(functions.SelectMany(o => o.Output));
        }

        private TES5Property? TryGetPropertyByName(string propertyName, bool throwException)
        {
            if (this.Properties.Count > 0)
            {
                string propertyNameWithSuffix = TES5Property.AddPropertyNameSuffix(propertyName, false);
                foreach (var property in this.Properties)
                {
                    string currentPropertyName = property.Name;
                    if (propertyName.Equals(currentPropertyName, StringComparison.OrdinalIgnoreCase) || propertyNameWithSuffix.Equals(currentPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        //Token found.
                        return property;
                    }
                }
            }
            if (throwException) { throw new InvalidOperationException(nameof(propertyName) + " " + propertyName + " not found."); }
            return null;
        }
        public TES5Property? TryGetPropertyByName(string propertyName)
        {
            return TryGetPropertyByName(propertyName, false);
        }
        public TES5Property GetPropertyByName(string propertyName)
        {
            return TryGetPropertyByName(propertyName, true)!;
        }

        private TES5Property? TryGetPropertyByOriginalName(string originalName, bool throwException)
        {
            TES5Property? property = this.Properties.Where(p => p.OriginalName.Equals(originalName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (property == null && throwException) { throw new InvalidOperationException(nameof(originalName) + " " + originalName + " not found."); }
            return property;
        }
        public TES5Property? TryGetPropertyByOriginalName(string originalName)
        {
            return TryGetPropertyByOriginalName(originalName, false);
        }

        public void AddPlayerRefPropertyIfNotExists()
        {
            const string name = TES5PlayerReference.PlayerRefName;
            TES5BasicType playerType = TES5PlayerReference.TES5TypeStatic;
            TES5Property property = TES5PropertyFactory.ConstructWithTES4FormID(name, playerType, name, TES5PlayerReference.FormID);
            AddPropertyIfNotExists(property);
        }

        public Nullable<float> GetTimerIntervalIfNoQuestDelayTime(out TES5Property? questDelayTimeProperty)
        {
            questDelayTimeProperty = null;
            if (ScriptHeader.ScriptType.NativeType == TES5BasicType.T_QUEST)
            {
                questDelayTimeProperty = Properties.Where(p => p.OriginalName.Equals("fquestdelaytime", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (questDelayTimeProperty != null)
                {
                    return null;
                }
                return 5;
            }
            return 0.4f;
        }
    }
}