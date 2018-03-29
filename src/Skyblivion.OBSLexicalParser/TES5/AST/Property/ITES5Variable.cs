using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    /*
     * Interface TES5Variable
     * Implementers declare they are a variable to be put in a scope.
     */
    interface ITES5Variable : ITES5Outputtable
    {
        string getPropertyName();
        ITES5Type getPropertyType();
        void setPropertyType(ITES5Type type);
        /*
        * Get the reference EDID
        */
        string getReferenceEdid();
        /*
        * Marks this variable to track a remote script - to be able to exchange inferencing information between multiple
         * scripts
        */
        void trackRemoteScript(TES5ScriptHeader scriptHeader);
    }
}