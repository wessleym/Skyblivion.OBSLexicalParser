using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    /*
     * Interface TES5Variable
     * Implementers declare they are a variable to be put in a scope.
     */
    interface ITES5VariableOrProperty : ITES5Value
    {
        string Name { get; }
        new ITES5Type TES5Type { get; set; }
        /*
        * Get the reference EDID
        */
        string ReferenceEDID { get; }
        /*
        * Marks this variable to track a remote script - to be able to exchange inferencing information between multiple
        * scripts
        */
        void TrackRemoteScript(TES5ScriptHeader scriptHeader);
    }
}