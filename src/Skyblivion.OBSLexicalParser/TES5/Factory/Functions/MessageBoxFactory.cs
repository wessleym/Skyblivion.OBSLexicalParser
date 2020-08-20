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
        public const string MessageBoxPrefix = TES5TypeFactory.TES4Prefix + "MessageBox";
        public MessageBoxFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //todo Refactor - add floating point vars .
            if (functionArguments.Count == 1)
            {
                TES5StaticReference calledOnRef = TES5StaticReferenceFactory.Debug;
                return this.objectCallFactory.CreateObjectCall(calledOnRef, "MessageBox", this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                string[] stringArguments = functionArguments.Select(v => v.StringValue).ToArray();
                string edid = GetEDID(stringArguments);
                IEnumerable<string> messageArguments = (new string[] { edid }).Concat(functionArguments.Select(a => a.StringValue));
                this.metadataLogService.WriteLine("ADD_MESSAGE", messageArguments);
                ITES5Referencer messageBoxResult = this.referenceFactory.CreateReadReference(TES5ReferenceFactory.MESSAGEBOX_VARIABLE_CONST, globalScope, multipleScriptsScope, localScope);
                ITES5Referencer reference = this.referenceFactory.CreateReadReference(edid, globalScope, multipleScriptsScope, localScope);
                return TES5VariableAssignationFactory.CreateAssignation(messageBoxResult, this.objectCallFactory.CreateObjectCall(reference, "show"));
            }
        }

        //WTM:  Change:  These conversions are needed since the original script converter used the below PHP to generate the EDID:
        //"TES4MessageBox" . md5(serialize($functionArguments->getValues()))
        //Attempting the serialize call in C# would produce different results, so they are instead copied into this project.
        //These values were copied into Skyblivion.esm some time ago.
        private static readonly Dictionary<string[], string> messageMD5s = new Dictionary<string[], string>()
        {
            { new string[] { "%0.f added back", "Actor" }, "561a6a32a19e057fd589cfe10f71bc7d" },
            { new string[] { "%0.f taken out", "Actor" }, "d9459a2c3a4e932a7a2e7e4512092ad3" },
            { new string[] { "A mysterious voice poses a question: What is the color of night?", "<Say nothing, and walk away.>" }, "bf1f196bcba619c5d25019b0c4e8ad43" },
            { new string[] { "A mysterious voice poses a question: What is the color of night?", "<Say nothing, and walk away.>", "Sanguine, my Brother." }, "0eb9589df05920099476fa6d9b16260e" },
            { new string[] { "Are you prepared to travel to Oblivion?", "Yes", "No" }, "213457ce5277520c9834f1a78c7a22d0" },
            { new string[] { "Are you ready to enter the Tournament of Ten Bloods?", "I am ready.", "I'm not ready yet." }, "739b599c20c784ca942d42714a7bab38" },
            { new string[] { "Before exiting the sewers, you may revise your character.", "Edit Race", "Edit Birthsign", "Edit Class", "Finished - Exit Sewers" }, "c24cbbdee07ebe98cf02c53e87b25a17" },
            { new string[] { "Do you return the Umbra Sword to Clavicus Vile?", "Yes", "No" }, "3466d79e6e7d9979566968e4c80b3f7c" },
            { new string[] { "Do you want to place a Heart of Order into the obelisk?", "Yes", "No" }, "a6aa811c1afe610c2f09f6032d15c101" },
            { new string[] { "Do you want to put the doll in the fire?", "Yes", "No" }, "c3f8c8b7c76d968a07f623e460bfb2c5" },
            { new string[] { "Do you want to release the prisoner?", "Yes", "No" }, "fb20cb4e657e612e016eea67bb60b10a" },
            { new string[] { "Do you want to use a Heart of Order to open this chest?", "OK", "Cancel" }, "dfd1dad2cec50e11439780543b12e572" },
            { new string[] { "Do you want to use a Shard of Order to open the container?", "Yes", "No" }, "ba981c907611282e2a995f84ea9d7b54" },
            { new string[] { "Do you wish to leave your offerings at the altar of Sheogorath?", "Yes", "No" }, "2d5e24a3eef43379bc50b1a3f60bbf8a" },
            { new string[] { "Do you wish to make an offering to the altar of Hircine?", "Offer Bear Pelt", "Do Nothing" }, "3898e79486d6b36bb83fbc7d8b1e48d1" },
            { new string[] { "Do you wish to make an offering to the altar of Hircine?", "Offer Bear Pelt", "Offer Wolf Pelt", "Do Nothing" }, "506498a3dc669080d5d2c644fd7e8037" },
            { new string[] { "Do you wish to make an offering to the altar of Hircine?", "Offer Wolf Pelt", "Do Nothing" }, "836ec93ed6d69b221541775a28f58bac" },
            { new string[] { "Do you wish to make an offering to the altar of Meridia?", "Offer Bonemeal", "Do Nothing" }, "ae1d6dab4be7d533042d2772ffa9d535" },
            { new string[] { "Do you wish to make an offering to the altar of Meridia?", "Offer Bonemeal", "Offer Ectoplasm", "Do Nothing" }, "ba2893c3db3df68fc0014bc69b3a5f4f" },
            { new string[] { "Do you wish to make an offering to the altar of Meridia?", "Offer Bonemeal", "Offer Ectoplasm", "Offer Mort Flesh", "Do Nothing" }, "8d21c68d9c0d3a2325161a01349e5ad7" },
            { new string[] { "Do you wish to make an offering to the altar of Meridia?", "Offer Bonemeal", "Offer Mort Flesh", "Do Nothing" }, "cf392911ad6dc3fa57255b2f4c51fce1" },
            { new string[] { "Do you wish to make an offering to the altar of Meridia?", "Offer Ectoplasm", "Do Nothing" }, "79e517a1da14b424c5c0f6a8133ab9dc" },
            { new string[] { "Do you wish to make an offering to the altar of Meridia?", "Offer Ectoplasm", "Offer Mort Flesh", "Do Nothing" }, "00a7f2ed4ba836874ede599f47fb3881" },
            { new string[] { "Do you wish to make an offering to the altar of Meridia?", "Offer Mort Flesh", "Do Nothing" }, "5e08bfc79a354349a40ddbe60e4f57b7" },
            { new string[] { "Do you wish to offer 500 gold to the altar of Clavicus?", "Yes", "No" }, "92b59354e9d271373c085ee46a5a1e92" },
            { new string[] { "Do you wish to offer a black soul gem to the altar of Vaermina?", "Yes", "No" }, "928ced66a36ee5b5a3ee599dd4bb3595" },
            { new string[] { "Do you wish to offer a Lion Hide to the altar of Molag Bal?", "Yes", "No" }, "af9e355dd826084cba28c8b9e65e5c11" },
            { new string[] { "Do you wish to offer Cyrodiilic Brandy to the altar of Sanguine?", "Yes", "No" }, "a9a79409c42dc8a5325e69919b7fe283" },
            { new string[] { "Do you wish to offer Daedra Heart to the altar of Boethia?", "Yes", "No" }, "ab97557af62381e0a7fe86b03c6b0dee" },
            { new string[] { "Do you wish to offer Glow Dust to the altar of Azura?", "Yes", "No" }, "f5910c37d8d79077df7fdffa4e958cf1" },
            { new string[] { "Do you wish to offer Nightshade to the altar of Mephala?", "Yes", "No" }, "200b32a5e5e9b080804585acb3c6db64" },
            { new string[] { "Do you wish to offer Troll Fat to the altar of Malacath?", "Yes", "No" }, "2052a34e5c0bf68838f0c90de090d947" },
            { new string[] { "Do you wish to read the Oghma Infinium?", "Read the Path of Steel", "Read the Path of Shadow", "Read the Path of Spirit", "Do Not Read Anything" }, "93b61917369dac854459d3f7f59b10b0" },
            { new string[] { "Here lies Count Cesrien Vitharn, Noble conqueror and just leader of our people.", "Pry Coffin Open", "Leave it Alone" }, "6b19291aa4691552d1da24331bdc7f06" },
            { new string[] { "Here lies Count Csaran Vitharn, to whom the Love of family meant so much.", "Pry Coffin Open", "Leave it Alone" }, "c4a466b1651107e18ff0599c94190cae" },
            { new string[] { "Here lies Countess Jideen, Lady of color from whom we all had so much to learn.", "Pry Coffin Open", "Leave it Alone" }, "20bab3e5c8ae17f3c80ed9cb34ba437a" },
            { new string[] { "Here lies Countess Sheen-In-Glade, Matron Scholar of Vitharn, and Ambassador of Black Marsh.", "Pry Coffin Open", "Leave it Alone" }, "9cfe7f3ea85bf481461dd72e1213cf9c" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Apprentice?", "No", "Yes" }, "587023dead3d9b26c812b9e2bfa7a90f" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Atronach?", "No", "Yes" }, "642feaf9b7230ed75bc0aafac95358a7" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Lady?", "No", "Yes" }, "1b5de5e808fb07acc01aea7995fbc9cb" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Lord?", "No", "Yes" }, "6a8c86ad8f10ed4773dd2738906024c2" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Lover?", "No", "Yes" }, "50174d98c4f0c4727dd391a30b786602" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Mage?", "No", "Yes" }, "217fac8f1988040b60a8992304d0edc3" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Ritual?", "No", "Yes" }, "432a6bc977c1b08c0f54982f1b337dfc" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Serpent?", "No", "Yes" }, "1cd2fe7a51f1de87f38d894cbb0b377b" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Shadow?", "No", "Yes" }, "69275e2e238a8e1aa6743a12d7824ca6" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Steed?", "No", "Yes" }, "23c10d1971ffeca66b2a40dba589e378" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Thief?", "No", "Yes" }, "9bac589b37c8154dd5e66a3d64f09e22" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Tower?", "No", "Yes" }, "738718d8fc5a0932a865ecfa328da33f" },
            { new string[] { "Marked by a special fate, you rule your destiny. Do you choose to steer by the stars of the Warrior?", "No", "Yes" }, "070989afc47fe8a3fdad08cdae8118f1" },
            { new string[] { "Persuasion help screen", "Done", "Never show again" }, "329ff4595de23dc86274ce99b30261af" },
            { new string[] { "Place the Olroy Cheese in the cooking pot?", "Yes", "No" }, "653675f78c370cb76fca06f464fd2c65" },
            { new string[] { "Place the Rat Poison in the Feeding Trough?", "Yes", "No" }, "94786b8b45d0fb4b1998e22c6e6aa06f" },
            { new string[] { "The bands emit a red glow and the door opens.", "Done" }, "be2da56194059655b53084cc7272d2e9" },
            { new string[] { "The Savior of Bruma. Singlehandedly fought off the hordes of Oblivion, entered their Great Gate and cast down the dread Siege Machine in ruin. Erected by the Grateful Citizens of Bruma, 3E %.0f.", "statueYear" }, "406c98a7b65a2d604ace8b66ae1a48a3" },
            { new string[] { "The stone stirs under your hand. Do you seek its conjured weapon and armor?", "No", "Yes" }, "4f6694b28ef5a3fad4bc8a94254e8253" },
            { new string[] { "The stone stirs under your hand. Do you seek its conjured weapon and armor?", "No", "Yes" }, "4f6694b28ef5a3fad4bc8a94254e8253" },
            { new string[] { "The stone stirs under your hand. Do you seek its conjured weapon and armor?", "No", "Yes" }, "4f6694b28ef5a3fad4bc8a94254e8253" },
            { new string[] { "These appear to be the fastenings of the mounted minotaur head. As expected, they have been loosened over time. You could easily remove the fastenings, causing the mounted head to crash to the floor below. Do you wish to remove the fastenings?", "Yes, remove the fastenings.", "No, do not remove the fastenings." }, "dae49dc67358345966fec05b8b40e766" },
            { new string[] { "Will you return to the world of men and claim your prize?", "Yes.", "First I must claim the spoils from the fallen." }, "f3613a6db8689fafb2b31240e2802b45" },
            { new string[] { "You believe you could effectively hide in this crate and then be transported to the pirate ship Marie Elena. Do you wish to hide in the crate, or try to find your own way onboard the ship?", "Hide in the crate.", "Do not hide in the crate." }, "c100b7c600dec176d09bbe95f030cc36" }
        };

        private static string GetEDID(string[] stringArguments)
        {
            string md5 = messageMD5s.Where(kvp => kvp.Key.SequenceEqual(stringArguments)).Select(kvp => kvp.Value).FirstOrDefault();//WTM:  Added:  See notes on messageEDIDs.
            if (md5 != null)
            {
                return TES5TypeFactory.TES4Prefix + "MessageBox" + md5;
            }
            else
            {//WTM:  Note:  If not found, generate a more predictable name;
                return NameTransformer.GetEscapedName(string.Join("", stringArguments), MessageBoxPrefix + "_", true);//WTM:  Change:  PHPFunction.MD5(PHPFunction.Serialize(functionArguments.getValues()))
            }
        }
    }
}