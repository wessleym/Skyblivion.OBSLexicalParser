using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class MessageBoxFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public MessageBoxFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        //WTM:  Change:  These conversions are needed since I don't know what method was used to generate EDIDs when Skyblivion.esm was generated.
        //They were copied from Skyblivion.esm.  This is an imperfect solution since the questions are not unique.
        private static readonly Dictionary<string, string> messageEDIDs = new Dictionary<string, string>()
        {
            { "%0.f added back", "TES4MessageBox9b2c3791ba4cb0b95a4757aa90138f1b" },
            { "%0.f taken out", "TES4MessageBox9d765edc8f7b20d3ed7c238e965c841c" },
            { "Are you prepared to travel to Oblivion?", "TES4MessageBox213457ce5277520c9834f1a78c7a22d0" },
            { "Are you ready to enter the Tournament of Ten Bloods?", "TES4MessageBox739b599c20c784ca942d42714a7bab38" },
            { "Before exiting the sewers, you may revise your character.", "TES4MessageBoxc24cbbdee07ebe98cf02c53e87b25a17" },
            { "Do you return the Umbra Sword to Clavicus Vile?", "TES4MessageBox3466d79e6e7d9979566968e4c80b3f7c" },
            { "Do you want to place a Heart of Order into the obelisk?", "TES4MessageBoxa6aa811c1afe610c2f09f6032d15c101" },
            { "Do you want to put the doll in the fire?", "TES4MessageBoxc3f8c8b7c76d968a07f623e460bfb2c5" },
            { "Do you want to release the prisoner?", "TES4MessageBoxfb20cb4e657e612e016eea67bb60b10a" },
            { "Do you want to use a Heart of Order to open this chest?", "TES4MessageBoxdfd1dad2cec50e11439780543b12e572" },
            { "Do you want to use a Shard of Order to open the container?", "TES4MessageBoxba981c907611282e2a995f84ea9d7b54" },
            { "Do you wish to leave your offerings at the altar of Sheogorath?", "TES4MessageBox2d5e24a3eef43379bc50b1a3f60bbf8a" },
            /*{ "Do you wish to make an offering to the altar of Hircine?", "TES4MessageBox3898e79486d6b36bb83fbc7d8b1e48d1" },
            { "Do you wish to make an offering to the altar of Hircine?", "TES4MessageBox836ec93ed6d69b221541775a28f58bac" },
            { "Do you wish to make an offering to the altar of Hircine?", "TES4MessageBox506498a3dc669080d5d2c644fd7e8037" },
            { "Do you wish to make an offering to the altar of Meridia?", "TES4MessageBoxba2893c3db3df68fc0014bc69b3a5f4f" },
            { "Do you wish to make an offering to the altar of Meridia?", "TES4MessageBox5e08bfc79a354349a40ddbe60e4f57b7" },
            { "Do you wish to make an offering to the altar of Meridia?", "TES4MessageBoxcf392911ad6dc3fa57255b2f4c51fce1" },
            { "Do you wish to make an offering to the altar of Meridia?", "TES4MessageBox00a7f2ed4ba836874ede599f47fb3881" },
            { "Do you wish to make an offering to the altar of Meridia?", "TES4MessageBoxae1d6dab4be7d533042d2772ffa9d535" },
            { "Do you wish to make an offering to the altar of Meridia?", "TES4MessageBox79e517a1da14b424c5c0f6a8133ab9dc" },
            { "Do you wish to make an offering to the altar of Meridia?", "TES4MessageBox8d21c68d9c0d3a2325161a01349e5ad7" },*/
            { "Do you wish to offer 500 gold to the altar of Clavicus?", "TES4MessageBox92b59354e9d271373c085ee46a5a1e92" },
            { "Do you wish to offer a black soul gem to the altar of Vaermina?", "TES4MessageBox928ced66a36ee5b5a3ee599dd4bb3595" },
            { "Do you wish to offer a Lion Hide to the altar of Molag Bal?", "TES4MessageBoxaf9e355dd826084cba28c8b9e65e5c11" },
            { "Do you wish to offer Cyrodiilic Brandy to the altar of Sanguine?", "TES4MessageBoxa9a79409c42dc8a5325e69919b7fe283" },
            { "Do you wish to offer Daedra Heart to the altar of Boethia?", "TES4MessageBoxab97557af62381e0a7fe86b03c6b0dee" },
            { "Do you wish to offer Glow Dust to the altar of Azura?", "TES4MessageBoxf5910c37d8d79077df7fdffa4e958cf1" },
            { "Do you wish to offer Nightshade to the altar of Mephala?", "TES4MessageBox200b32a5e5e9b080804585acb3c6db64" },
            { "Do you wish to offer Troll Fat to the altar of Malacath?", "TES4MessageBox2052a34e5c0bf68838f0c90de090d947" },
            { "Do you wish to read the Oghma Infinium?", "TES4MessageBox93b61917369dac854459d3f7f59b10b0" },
            { "Here lies Count Cesrien Vitharn, Noble conqueror and just leader of our people.", "TES4MessageBox6b19291aa4691552d1da24331bdc7f06" },
            { "Here lies Count Csaran Vitharn, to whom the Love of family meant so much.", "TES4MessageBoxc4a466b1651107e18ff0599c94190cae" },
            { "Here lies Countess Jideen, Lady of color from whom we all had so much to learn.", "TES4MessageBox20bab3e5c8ae17f3c80ed9cb34ba437a" },
            { "Here lies Countess Sheen-In-Glade, Matron Scholar of Vitharn, and Ambassador of Black Marsh.", "TES4MessageBox9cfe7f3ea85bf481461dd72e1213cf9c" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Apprentice?", "TES4MessageBox587023dead3d9b26c812b9e2bfa7a90f" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Atronach?", "TES4MessageBox642feaf9b7230ed75bc0aafac95358a7" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Lady?", "TES4MessageBox1b5de5e808fb07acc01aea7995fbc9cb" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Lord?", "TES4MessageBox6a8c86ad8f10ed4773dd2738906024c2" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Lover?", "TES4MessageBox50174d98c4f0c4727dd391a30b786602" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Mage?", "TES4MessageBox217fac8f1988040b60a8992304d0edc3" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Ritual?", "TES4MessageBox432a6bc977c1b08c0f54982f1b337dfc" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Serpent?", "TES4MessageBox1cd2fe7a51f1de87f38d894cbb0b377b" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Shadow?", "TES4MessageBox69275e2e238a8e1aa6743a12d7824ca6" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Steed?", "TES4MessageBox23c10d1971ffeca66b2a40dba589e378" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Thief?", "TES4MessageBox9bac589b37c8154dd5e66a3d64f09e22" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Tower?", "TES4MessageBox738718d8fc5a0932a865ecfa328da33f" },
            { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Warrior?", "TES4MessageBox070989afc47fe8a3fdad08cdae8118f1" },
            { "Place the Olroy Cheese in the cooking pot?", "TES4MessageBox653675f78c370cb76fca06f464fd2c65" },
            { "Place the Rat Poison in the Feeding Trough?", "TES4MessageBox94786b8b45d0fb4b1998e22c6e6aa06f" },
            { "The bands emit a red glow and the door opens.", "TES4MessageBoxbe2da56194059655b53084cc7272d2e9" },
            { "The Savior of Bruma. Singlehandedly fought off the hordes of Oblivion, entered their Great Gate and cast down the dread Siege Machine in ruin. Erected by the Grateful Citizens of Bruma, 3E %.0f.", "TES4MessageBox74dd774db32579775d900161a824dedc" },
            { "The stone stirs under your hand. Do you seek its conjured weapon and armor?", "TES4MessageBox4f6694b28ef5a3fad4bc8a94254e8253" },
            { "These appear to be the fastenings of the mounted minotaur head. As expected, they have been loosened over time. You could easily remove the fastenings, causing the mounted head to crash to the floor below. Do you wish to remove the fastenings?", "TES4MessageBoxdae49dc67358345966fec05b8b40e766" },
            { "Will you return to the world of men and claim your prize?", "TES4MessageBoxf3613a6db8689fafb2b31240e2802b45" },
            { "You believe you could effectively hide in this crate and then be transported to the pirate ship Marie Elena. Do you wish to hide in the crate, or try to find your own way onboard the ship?", "TES4MessageBoxc100b7c600dec176d09bbe95f030cc36" },
        };

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //todo Refactor - add floating point vars .
            if (functionArguments.Count== 1)
            {
                TES5StaticReference calledOnRef = TES5StaticReferenceFactory.Debug;
                return this.objectCallFactory.CreateObjectCall(calledOnRef, "MessageBox", this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                string edid;
                if (!messageEDIDs.TryGetValue(functionArguments[0].StringValue, out edid))//WTM:  Added:  Change
                {
                    edid = NameTransformer.GetEscapedName(string.Join("", functionArguments.Select(v => v.StringValue)), TES5TypeFactory.TES4Prefix + "MessageBox_", true);//WTM:  Change:  PHPFunction.MD5(PHPFunction.Serialize(functionArguments.getValues()))
                }
                IEnumerable<string> messageArguments = (new string[] { edid }).Concat(functionArguments.Select(a => a.StringValue));
                this.metadataLogService.WriteLine("ADD_MESSAGE", messageArguments);
                ITES5Referencer messageBoxResult = this.referenceFactory.CreateReadReference(TES5ReferenceFactory.MESSAGEBOX_VARIABLE_CONST, globalScope, multipleScriptsScope, localScope);
                ITES5Referencer reference = this.referenceFactory.CreateReadReference(edid, globalScope, multipleScriptsScope, localScope);
                return TES5VariableAssignationFactory.CreateAssignation(messageBoxResult, this.objectCallFactory.CreateObjectCall(reference, "show"));
            }
        }
    }
}