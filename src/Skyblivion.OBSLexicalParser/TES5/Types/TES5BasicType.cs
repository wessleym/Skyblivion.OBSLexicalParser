using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    /*
     * Class TES5BasicType
     * @package Ormin\OBSLexicalParser\TES5\Types
     * @method static TES5BasicType T_ACTIVEMAGICEFFECT
     * @method static TES5BasicType T_INT
     * @method static TES5BasicType T_FLOAT
     * @method static TES5BasicType T_ALIAS
     * @method static TES5BasicType T_REFERENCEALIAS
     * @method static TES5BasicType T_LOCATIONALIAS
     * @method static TES5BasicType T_UTILITY
     * @method static TES5BasicType T_DEBUG
     * @method static TES5BasicType T_GAME
     * @method static TES5BasicType T_MAIN
     * @method static TES5BasicType T_MATH
     * @method static TES5BasicType T_FORM
     * @method static TES5BasicType T_ACTION
     * @method static TES5BasicType T_MAGICEFFECT
     * @method static TES5BasicType T_ACTIVATOR
     * @method static TES5BasicType T_FURNITURE
     * @method static TES5BasicType T_FLORA
     * @method static TES5BasicType T_TALKINGACTIVATOR
     * @method static TES5BasicType T_MESSAGE
     * @method static TES5BasicType T_ACTORBASE
     * @method static TES5BasicType T_MISCOBJECT
     * @method static TES5BasicType T_APPARATUS
     * @method static TES5BasicType T_CONSTRUCTIBLEOBJECT
     * @method static TES5BasicType T_KEY
     * @method static TES5BasicType T_SOULGEM
     * @method static TES5BasicType T_AMMO
     * @method static TES5BasicType T_ARMOR
     * @method static TES5BasicType T_ARMORADDON
     * @method static TES5BasicType T_ASSOCIATIONTYPE
     * @method static TES5BasicType T_MUSICTYPE
     * @method static TES5BasicType T_BOOK
     * @method static TES5BasicType T_BOOL
     * @method static TES5BasicType T_OBJECTREFERENCE
     * @method static TES5BasicType T_ACTOR
     * @method static TES5BasicType T_CELL
     * @method static TES5BasicType T_CLASS
     * @method static TES5BasicType T_OUTFIT
     * @method static TES5BasicType T_COLORFORM
     * @method static TES5BasicType T_PACKAGE
     * @method static TES5BasicType T_COMBATSTYLE
     * @method static TES5BasicType T_CONTAINER
     * @method static TES5BasicType T_PERK
     * @method static TES5BasicType T_DOOR
     * @method static TES5BasicType T_POTION
     * @method static TES5BasicType T_EFFECTSHADER
     * @method static TES5BasicType T_PROJECTILE
     * @method static TES5BasicType T_ENCHANTMENT
     * @method static TES5BasicType T_QUEST
     * @method static TES5BasicType T_ENCOUNTERZONE
     * @method static TES5BasicType T_RACE
     * @method static TES5BasicType T_COLORCOMPONENT
     * @method static TES5BasicType T_EQUIPSLOT
     * @method static TES5BasicType T_SCENE
     * @method static TES5BasicType T_EXPLOSION
     * @method static TES5BasicType T_FACTION
     * @method static TES5BasicType T_FORMLIST
     * @method static TES5BasicType T_SCROLL
     * @method static TES5BasicType T_GLOBALVARIABLE
     * @method static TES5BasicType T_SHOUT
     * @method static TES5BasicType T_HAZARD
     * @method static TES5BasicType T_SOUND
     * @method static TES5BasicType T_HEADPART
     * @method static TES5BasicType T_SOUNDCATEGORY
     * @method static TES5BasicType T_IDLE
     * @method static TES5BasicType T_SPELL
     * @method static TES5BasicType T_IMAGESPACEMODIFIER
     * @method static TES5BasicType T_STATIC
     * @method static TES5BasicType T_IMPACTDATASET
     * @method static TES5BasicType T_TEXTURESET
     * @method static TES5BasicType T_INGREDIENT
     * @method static TES5BasicType T_TOPIC
     * @method static TES5BasicType T_KEYWORD
     * @method static TES5BasicType T_LOCATIONREFTYPE
     * @method static TES5BasicType T_TOPICINFO
     * @method static TES5BasicType T_LEVELEDACTOR
     * @method static TES5BasicType T_VISUALEFFECT
     * @method static TES5BasicType T_LEVELEDITEM
     * @method static TES5BasicType T_VOICETYPE
     * @method static TES5BasicType T_LEVELEDSPELL
     * @method static TES5BasicType T_WEAPON
     * @method static TES5BasicType T_LIGHT
     * @method static TES5BasicType T_WEATHER
     * @method static TES5BasicType T_LOCATION
     * @method static TES5BasicType T_WORDOFPOWER
     * @method static TES5BasicType T_WORLDSPACE
     * @method static TES5BasicType T_INPUT
     * @method static TES5BasicType T_SKSE
     * @method static TES5BasicType T_STRING
     * @method static TES5BasicType T_STRINGUTIL
     * @method static TES5BasicType T_UI
     * @method static TES5BasicType T_TES4TIMERHELPER()
     */
    class TES5BasicType : ITES5Type
    {
        public string Name { get; private set; }
        private TES5BasicType(string name)
        {
            Name = name;
        }

        public static readonly TES5BasicType
            T_ACTIVEMAGICEFFECT = new TES5BasicType("ActiveMagicEffect"),
            T_ALIAS = new TES5BasicType("Alias"),
            T_REFERENCEALIAS = new TES5BasicType("ReferenceAlias"),
            T_LOCATIONALIAS = new TES5BasicType("LocationAlias"),
            T_UTILITY = new TES5BasicType("Utility"),
            T_DEBUG = new TES5BasicType("Debug"),
            T_GAME = new TES5BasicType("Game"),
            T_MAIN = new TES5BasicType("Main"),
            T_MATH = new TES5BasicType("Math"),
            T_FORM = new TES5BasicType("Form"),
            T_ACTION = new TES5BasicType("Action"),
            T_MAGICEFFECT = new TES5BasicType("MagicEffect"),
            T_ACTIVATOR = new TES5BasicType("Activator"),
            T_FURNITURE = new TES5BasicType("Furniture"),
            T_FLORA = new TES5BasicType("Flora"),
            T_TALKINGACTIVATOR = new TES5BasicType("TalkingActivator"),
            T_MESSAGE = new TES5BasicType("Message"),
            T_COLORCOMPONENT = new TES5BasicType("ColorComponent"),
            T_ACTORBASE = new TES5BasicType("ActorBase"),
            T_MISCOBJECT = new TES5BasicType("MiscObject"),
            T_APPARATUS = new TES5BasicType("Apparatus"),
            T_CONSTRUCTIBLEOBJECT = new TES5BasicType("ConstructibleObject"),
            T_KEY = new TES5BasicType("Key"),
            T_SOULGEM = new TES5BasicType("SoulGem"),
            T_AMMO = new TES5BasicType("Ammo"),
            T_ARMOR = new TES5BasicType("Armor"),
            T_ARMORADDON = new TES5BasicType("ArmorAddon"),
            T_ASSOCIATIONTYPE = new TES5BasicType("AssociationType"),
            T_MUSICTYPE = new TES5BasicType("MusicType"),
            T_BOOK = new TES5BasicType("Book"),
            T_BOOL = new TES5BasicType("Bool"),
            T_OBJECTREFERENCE = new TES5BasicType("ObjectReference"),
            T_ACTOR = new TES5BasicType("Actor"),
            T_CELL = new TES5BasicType("Cell"),
            T_CLASS = new TES5BasicType("Class"),
            T_OUTFIT = new TES5BasicType("Outfit"),
            T_COLORFORM = new TES5BasicType("ColorForm"),
            T_PACKAGE = new TES5BasicType("Package"),
            T_COMBATSTYLE = new TES5BasicType("CombatStyle"),
            T_CONTAINER = new TES5BasicType("Container"),
            T_PERK = new TES5BasicType("Perk"),
            T_DOOR = new TES5BasicType("Door"),
            T_POTION = new TES5BasicType("Potion"),
            T_EFFECTSHADER = new TES5BasicType("EffectShader"),
            T_PROJECTILE = new TES5BasicType("Projectile"),
            T_ENCHANTMENT = new TES5BasicType("Enchantment"),
            T_QUEST = new TES5BasicType("Quest"),
            T_ENCOUNTERZONE = new TES5BasicType("EncounterZone"),
            T_RACE = new TES5BasicType("Race"),
            T_EQUIPSLOT = new TES5BasicType("EquipSlot"),
            T_SCENE = new TES5BasicType("Scene"),
            T_EXPLOSION = new TES5BasicType("Explosion"),
            T_FACTION = new TES5BasicType("Faction"),
            T_FORMLIST = new TES5BasicType("FormList"),
            T_FLOAT = new TES5BasicType("Float"),
            T_STRING = new TES5BasicType("String"),
            T_SCROLL = new TES5BasicType("Scroll"),
            T_GLOBALVARIABLE = new TES5BasicType("GlobalVariable"),
            T_SHOUT = new TES5BasicType("Shout"),
            T_HAZARD = new TES5BasicType("Hazard"),
            T_SOUND = new TES5BasicType("Sound"),
            T_HEADPART = new TES5BasicType("HeadPart"),
            T_SOUNDCATEGORY = new TES5BasicType("SoundCategory"),
            T_IDLE = new TES5BasicType("Idle"),
            T_INT = new TES5BasicType("Int"),
            T_SPELL = new TES5BasicType("Spell"),
            T_IMAGESPACEMODIFIER = new TES5BasicType("ImageSpaceModifier"),
            T_STATIC = new TES5BasicType("Static"),
            T_IMPACTDATASET = new TES5BasicType("ImpactDataSet"),
            T_TEXTURESET = new TES5BasicType("TextureSet"),
            T_INGREDIENT = new TES5BasicType("Ingredient"),
            T_TOPIC = new TES5BasicType("Topic"),
            T_KEYWORD = new TES5BasicType("Keyword"),
            T_LOCATIONREFTYPE = new TES5BasicType("LocationRefType"),
            T_TOPICINFO = new TES5BasicType("TopicInfo"),
            T_LEVELEDACTOR = new TES5BasicType("LeveledActor"),
            T_VISUALEFFECT = new TES5BasicType("VisualEffect"),
            T_LEVELEDITEM = new TES5BasicType("LeveledItem"),
            T_VOICETYPE = new TES5BasicType("VoiceType"),
            T_LEVELEDSPELL = new TES5BasicType("LeveledSpell"),
            T_WEAPON = new TES5BasicType("Weapon"),
            T_LIGHT = new TES5BasicType("Light"),
            T_WEATHER = new TES5BasicType("Weather"),
            T_LOCATION = new TES5BasicType("Location"),
            T_WORDOFPOWER = new TES5BasicType("WordOfPower"),
            T_WORLDSPACE = new TES5BasicType("WorldSpace"),
            T_INPUT = new TES5BasicType("Input"),
            T_SKSE = new TES5BasicType("SKSE"),
            T_STRINGUTIL = new TES5BasicType("StringUtil"),
            T_UI = new TES5BasicType("UI"),
            T_TES4TIMERHELPER = new TES5BasicType("TES4TimerHelper");

            public static readonly TES5BasicType[] all = new TES5BasicType[]
            {
                T_ACTIVEMAGICEFFECT,
                T_ALIAS,
                T_REFERENCEALIAS,
                T_LOCATIONALIAS,
                T_UTILITY,
                T_DEBUG,
                T_GAME,
                T_MAIN,
                T_MATH,
                T_FORM,
                T_ACTION,
                T_MAGICEFFECT,
                T_ACTIVATOR,
                T_FURNITURE,
                T_FLORA,
                T_TALKINGACTIVATOR,
                T_MESSAGE,
                T_COLORCOMPONENT,
                T_ACTORBASE,
                T_MISCOBJECT,
                T_APPARATUS,
                T_CONSTRUCTIBLEOBJECT,
                T_KEY,
                T_SOULGEM,
                T_AMMO,
                T_ARMOR,
                T_ARMORADDON,
                T_ASSOCIATIONTYPE,
                T_MUSICTYPE,
                T_BOOK,
                T_BOOL,
                T_OBJECTREFERENCE,
                T_ACTOR,
                T_CELL,
                T_CLASS,
                T_OUTFIT,
                T_COLORFORM,
                T_PACKAGE,
                T_COMBATSTYLE,
                T_CONTAINER,
                T_PERK,
                T_DOOR,
                T_POTION,
                T_EFFECTSHADER,
                T_PROJECTILE,
                T_ENCHANTMENT,
                T_QUEST,
                T_ENCOUNTERZONE,
                T_RACE,
                T_EQUIPSLOT,
                T_SCENE,
                T_EXPLOSION,
                T_FACTION,
                T_FORMLIST,
                T_FLOAT,
                T_STRING,
                T_SCROLL,
                T_GLOBALVARIABLE,
                T_SHOUT,
                T_HAZARD,
                T_SOUND,
                T_HEADPART,
                T_SOUNDCATEGORY,
                T_IDLE,
                T_INT,
                T_SPELL,
                T_IMAGESPACEMODIFIER,
                T_STATIC,
                T_IMPACTDATASET,
                T_TEXTURESET,
                T_INGREDIENT,
                T_TOPIC,
                T_KEYWORD,
                T_LOCATIONREFTYPE,
                T_TOPICINFO,
                T_LEVELEDACTOR,
                T_VISUALEFFECT,
                T_LEVELEDITEM,
                T_VOICETYPE,
                T_LEVELEDSPELL,
                T_WEAPON,
                T_LIGHT,
                T_WEATHER,
                T_LOCATION,
                T_WORDOFPOWER,
                T_WORLDSPACE,
                T_INPUT,
                T_SKSE,
                T_STRINGUTIL,
                T_UI,
                T_TES4TIMERHELPER
        };

        public static TES5BasicType GetFirst(string name)
        {
            return all.Where(t => t.Name == name).First();
        }

        public bool isPrimitive()
        {
            return this == T_BOOL || this == T_INT || this == T_STRING || this == T_FLOAT;
        }

        public bool isNativePapyrusType()
        {
            return true;
        }

        public string value()
        {
            return Name;
        }

        public List<string> output()
        {
            return new List<string>() { Name };
        }

        public void setNativeType(ITES5Type basicType)
        {
            throw new ConversionException("Cannot set native type on basic type - wrong logic.");
        }

        public ITES5Type getNativeType()
        {
            return this;
        }
    }
}