using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.Types
{
    /*
     * Class TES4Type
     * @package Ormin\OBSLexicalParser\TES4\Types
     * @method static TES4Type T_REF
     * @method static TES4Type T_SHORT
     * @method static TES4Type T_LONG
     * @method static TES4Type T_FLOAT
     * @method static TES4Type T_INT
     * @method static TES4Type T_STRING
     */
    class TES4Type
    {
        public string Name { get; private set; }
        private TES4Type(string name)
        {
            this.Name = name;
        }

        public static readonly TES4Type
            T_REF = new TES4Type("ref"),
            T_SHORT = new TES4Type("short"),
            T_LONG = new TES4Type("long"),
            T_FLOAT = new TES4Type("float"),
            T_INT = new TES4Type("int"),
            T_STRING = new TES4Type("string");

        public static readonly TES4Type[] all = new TES4Type[]
        {
            T_REF,
            T_SHORT,
            T_LONG,
            T_FLOAT,
            T_INT,
            T_STRING
        };

        public static TES4Type GetFirst(string name)
        {
            return all.Where(t => t.Name == name).First();
        }
    }
}