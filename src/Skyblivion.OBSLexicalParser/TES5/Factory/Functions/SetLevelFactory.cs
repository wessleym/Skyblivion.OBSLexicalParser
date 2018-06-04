using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetLevelFactory : IFunctionFactory
    {
        private TES5ObjectCallFactory objectCallFactory;
        public SetLevelFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  @Inconsistence:
            //SKSE has Game.SetPlayerLevel(int level), but Oblivion calls this function on non-player actors.
            //Since we can't call SetLevel directly, we are instead modifying actor values.
            TES4FunctionArguments tes4Arguments = function.Arguments;
            int newLevel = (int)tes4Arguments[0].Data;
            bool levelToPC = tes4Arguments.Count > 1 ? (int)tes4Arguments[1].Data == 1 : false;//When false, newLevel should be evaluated absolutely.  When true, newLevel should be evaluated relative to the current level.
            string newFunctionName = !levelToPC ? "SetActorValue" : "ModActorValue";
            int attributeLevel = newLevel, skillLevel = newLevel;
            const int minAttributeValue = 25, minSkillLevel = 10;
            if (levelToPC)
            {//If levelToPC is true, increase or decrease attributes 10 times greater than the level change and skills 2 times greater than the level change.
                attributeLevel *= 10;
                skillLevel *= 2;
            }
            else
            {//If levelToPC is false, ensure values are not set to values less than minimums.
                if (attributeLevel < minAttributeValue) { attributeLevel = minAttributeValue; }
                if (skillLevel < minSkillLevel) { skillLevel = minSkillLevel; }
            }
            TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection();
            string[] attributes = new string[] { "Health", "Magicka", "Stamina" };
            foreach (string attribute in attributes)
            {
                codeChunks.Add(objectCallFactory.CreateObjectCall(calledOn, newFunctionName, multipleScriptsScope, new TES5ObjectCallArguments() { new TES5String(attribute), new TES5Float(attributeLevel) }));
            }
            string[] skills = new string[] { "OneHanded", "TwoHanded", "Marksman", "Block", "Smithing", "HeavyArmor", "LightArmor", "Pickpocket", "Lockpicking", "Sneak", "Alchemy", "Speechcraft", "Alteration", "Conjuration", "Destruction", "Illusion", "Restoration", "Enchanting" };
            foreach (string skill in skills)
            {
                codeChunks.Add(objectCallFactory.CreateObjectCall(calledOn, newFunctionName, multipleScriptsScope, new TES5ObjectCallArguments() { new TES5String(skill), new TES5Float(skillLevel) }));
            }
            return codeChunks;
        }
    }
}
