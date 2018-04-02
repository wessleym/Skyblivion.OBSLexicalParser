using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    class TES4ToTES5ASTQFFragmentConverter
    {
        /*
             * Oblivion binary data analyzer.
        */
        private ESMAnalyzer esmAnalyzer;
        private TES5FragmentFactory fragmentFactory;
        private TES5ValueFactory valueFactory;
        private TES5ReferenceFactory referenceFactory;
        public TES4ToTES5ASTQFFragmentConverter(ESMAnalyzer esmAnalyzer, TES5FragmentFactory fragmentFactory, TES5ValueFactory valueFactory, TES5ReferenceFactory referenceFactory)
        {
            this.esmAnalyzer = esmAnalyzer;
            this.fragmentFactory = fragmentFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
        }

        /*
             * @throws ConversionException
        */
        public TES5Target convert(TES4FragmentTarget fragmentTarget, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5FunctionCodeBlock fragment = this.fragmentFactory.createFragment(TES5FragmentType.T_QF, "Fragment_0", globalScope, multipleScriptsScope, fragmentTarget.getCodeChunks());
            TES5BlockList blockList = new TES5BlockList();
            blockList.add(fragment);
            TES5Script script = new TES5Script(globalScope, blockList);
            TES5Target target = new TES5Target(script, fragmentTarget.getOutputPath());
            return target;
        }
    }
}