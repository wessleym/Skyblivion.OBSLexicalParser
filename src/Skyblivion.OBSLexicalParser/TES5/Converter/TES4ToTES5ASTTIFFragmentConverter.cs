using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    class TES4ToTES5ASTTIFFragmentConverter
    {
        /*
             * Oblivion binary data analyzer.
        */
        private readonly TES5FragmentFactory fragmentFactory;
        public TES4ToTES5ASTTIFFragmentConverter(TES5FragmentFactory fragmentFactory)
        {
            this.fragmentFactory = fragmentFactory;
        }

        /*
             * @throws ConversionException
        */
        public TES5Target Convert(TES4FragmentTarget fragmentTarget, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return fragmentFactory.Convert(TES5FragmentType.T_TIF, fragmentTarget, globalScope, multipleScriptsScope);
        }
    }
}