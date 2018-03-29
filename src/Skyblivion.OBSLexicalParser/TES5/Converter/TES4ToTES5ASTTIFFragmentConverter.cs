using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    class TES4ToTES5ASTTIFFragmentConverter
    {
        /*
             * Oblivion binary data analyzer.
        */
        private ESMAnalyzer esmAnalyzer;
        private TES5FragmentFactory fragmentFactory;
        private TES5ValueFactory valueFactory;
        private TES5ReferenceFactory referenceFactory;
        private TES5PropertiesFactory propertiesFactory;
        private TES5NameTransformer nameTransformer;
        public TES4ToTES5ASTTIFFragmentConverter(ESMAnalyzer esmAnalyzer, TES5FragmentFactory fragmentFactory, TES5ValueFactory valueFactory, TES5ReferenceFactory referenceFactory, TES5PropertiesFactory propertiesFactory, TES5NameTransformer nameTransformer)
        {
            this.esmAnalyzer = esmAnalyzer;
            this.fragmentFactory = fragmentFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.propertiesFactory = propertiesFactory;
            this.nameTransformer = nameTransformer;
        }

        /*
             * @throws ConversionException
        */
        public TES5Target convert(TES4FragmentTarget fragmentTarget, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5FunctionCodeBlock fragment = this.fragmentFactory.createFragment(TES5FragmentType.T_TIF, "Fragment_0", globalScope, multipleScriptsScope, fragmentTarget.getCodeChunks());
            TES5BlockList blockList = new TES5BlockList();
            blockList.add(fragment);
            TES5Script script = new TES5Script(globalScope, blockList);
            TES5Target target = new TES5Target(script, fragmentTarget.getOutputPath());
            return target;
        }
    }
}