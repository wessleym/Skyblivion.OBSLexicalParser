using Skyblivion.ESReader.Extensions;
using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES4.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory
{
    static class ReferenceAliasBuilder
    {
        private static IEnumerable<string> GetReferenceAliases(IBuildTarget target, string resultingFragmentName)
        {
            /*
             * Add ReferenceAliases
             * At some point, we might port the conversion so it doesn't use the directly injected property,
             * but instead has a map to aliases and we'll map accordingly and have references point to aliases instead
             */
            string sourcePath = target.GetSourceFromPath(resultingFragmentName);
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            string? sourceDirectory = Path.GetDirectoryName(sourcePath);
            if (sourceDirectory == null) { throw new NullableException(nameof(sourceDirectory)); }
            string aliasesFile = Path.Combine(sourceDirectory, scriptName + ".aliases");
            string[] aliasesLines = File.ReadAllLines(aliasesFile);
            return aliasesLines.Select(a => a.Trim()).Where(a => a != "").Select(a =>
            {
                //WTM:  Change:  This used to build an alias like this:
                //Alias_[FormID]_p
                //I changed it to generate this:
                //Alias_[EditorID]_p
                /*const string Alias_ = "Alias_";
                if (!trimmedAlias.StartsWith(Alias_)) { throw new ConversionException(nameof(trimmedAlias) + " did not start with " + Alias_ + ":  " + trimmedAlias); }
                string formIDString = trimmedAlias.Substring(Alias_.Length);
                int formID = Convert.ToInt32(formIDString, 16);
                string? edid = esmAnalyzer.GetEDIDByFormIDNullable(formID);
                string innerName = edid != null ? edid : formIDString;
                string propertyName = Alias_ + innerName;*/
                //WTM:  Note:  The above didn't work.  GECKFrontend couldn't seem to associate the properties correctly.
                return a;
            }).Distinct();
        }

        private static IEnumerable<string> GetReferenceAliases(ESMAnalyzer esmAnalyzer, int tes4FormID)
        {
            TES4Record qust = esmAnalyzer.GetRecordByFormID(tes4FormID);
            return ESMAnalyzer.GetReferenceAliases(qust);
        }

        public static IEnumerable<string> Build(IBuildTarget target, string resultingFragmentName, ESMAnalyzer esmAnalyzer, int tes4FormID)
        {
            return

#if USEFILESINSTEADOFESM
                GetReferenceAliases(target, resultingFragmentName)
#else
                GetReferenceAliases(esmAnalyzer, tes4FormID)
#endif
                ;
                
        }
    }
}
