using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IBuildScopeCommand
    {
        void initialize();
        /*
        * Build a global scope for a given source path.
         *
         * Command is expected to return a valid global scope for the given script path, assuming the build target
         * it is in ( as in - global scopes are built in a different way for Standalone, different for TIF, etc. )
         *
         * Global variables are passed so that when parsing variable declarations list ( i.e. ref XXX from Obscript ),
         * we"re able to tell from the start if property is a GlobalVariable or not
         *
         * 
         *  Defined global variables used within the scope
        */
        TES5GlobalScope buildScope(string sourcePath, TES5GlobalVariables globalVariables);
    }
}