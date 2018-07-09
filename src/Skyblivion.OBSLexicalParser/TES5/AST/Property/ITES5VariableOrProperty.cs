using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    /*
     * Interface TES5Variable
     * Implementers declare they are a variable to be put in a scope.
     */
    interface ITES5VariableOrProperty
    {
        string Name { get; }

        /**
         * Inferrable type
         * This is the type that we infer as we go through the scripts
         */
        ITES5Type TES5Type { get; set; }

        /**
         * Declaring type
         * This is the type that we intend to declare
         * For most cases, this will be exactly the same as inferring type
         * as we have all the liberty in declaring whatever we want
         * A notable exception is native function/event handlers' parameters,
         * where the types are set in stone by the engine and hence we need
         * to declare them in a particular way and downward cast
         */
        ITES5Type TES5DeclaredType { get; }

        /*
        * Reference EDID
        */
        string ReferenceEDID { get; }
        /*
        * Marks this variable to track a remote script - to be able to exchange inferencing information between multiple
        * scripts
        */
        void TrackRemoteScript(TES5ScriptHeader scriptHeader);
    }
}