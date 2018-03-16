# Skyblivion.OBSLexicalParser

This project is a C# conversion of https://github.com/TESSkyblivion/skyblivion-ScriptConverter, which is written in PHP.

The original PHP project depends on https://github.com/jakubledl/dissect and https://github.com/TESSkyblivion/esreader.  So, to convert this project, I had to convert its references as well.  This project includes my C# conversion of Dissect (https://github.com/jakubledl/dissect).  https://github.com/wessleym/Skyblivion.ESReader is a separate project.

This project does not yet build, mostly because Skyblivion.OBSLexicalParser.TES5.Factory.Functions.IFunctionFactory requires its convertFunction method to return an ITES5CodeChunk.  But many classes that implement IFunctionFactory do not return ITES5CodeChunk without casting their result.  For example, see Skyblivion.OBSLexicalParser.TES5.Factory.Functions.GetArmorRatingFactory.convertFunction.

To set up the project on your computer, be sure to download this project and https://github.com/wessleym/Skyblivion.ESReader.
