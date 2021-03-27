using System;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    /**
     * TODO - move this class out of there
     * They are meant to be a mere representing of global variables and
     * do not use scoping system at all, hence should not be here
     */
    class TES5GlobalVariable
    {
        public string Name { get; }

        public TES5GlobalVariable(string name)
        {
            Name = name;
        }
    }
}