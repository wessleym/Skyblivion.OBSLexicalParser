using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Types
{
    /*
     * Class TES5CustomType
     * @package Ormin\OBSLexicalParser\TES5\Types
     * @method static ITES5Type T_TES4CONVERTERHOOK()
     * @method static ITES5Type T_TES4CONTAINER()
     * @method static ITES5Type T_TES4TIMERHELPER()
     */
    class TES5CustomType : ITES5Type
    {
        const string T_TES4CONTAINER = "TES4Container";
        const string T_TES4TIMERHELPER = "TES4TimerHelper";
        private string typeName;
        private ITES5Type nativeType;
        /*
        * Original type name
         * Needed only for compilation graph build.. will be scrapped once this is cleaned up properly.
        */
        private string originalName;
        private string prefix;
        public TES5CustomType(string typeName, string prefix, string originalName, ITES5Type nativeType)
        {
            this.typeName = typeName;
            this.prefix = prefix;
            this.originalName = originalName;
            //qt = new ReflectionClass(get_class(this));WTM:  Change:  Unused
            //this.constants = qt.getConstants();
            this.nativeType = nativeType;
        }

        public string value()
        {
            return this.typeName;
        }

        public List<string> output()
        {
            if (this.typeName != "TES4TimerHelper" && this.typeName != "TES4Container")
            {
                return new List<string>() { this.prefix + this.value() };
            } //no time to refactor now, later.

            return new List<string>() { this.value() };
        }

        public bool isPrimitive()
        {
            return this.nativeType.isPrimitive();
        }

        public bool isNativePapyrusType()
        {
            return false;
        }

        public ITES5Type getNativeType()
        {
            return this.nativeType;
        }

        public void setNativeType(ITES5Type basicType)
        {
            this.nativeType = basicType;
        }

        public string getOriginalName()
        {
            return this.originalName;
        }
    }
}