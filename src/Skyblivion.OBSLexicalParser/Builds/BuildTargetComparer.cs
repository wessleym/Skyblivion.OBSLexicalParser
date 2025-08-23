using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.Builds
{
    internal class BuildTargetComparer
    {
        private readonly BuildTargetExecutor buildTargetExecutor;
        private readonly IReadOnlyList<BuildTarget> buildTargets;
        private readonly string /*currentScriptDirectory, */currentSourceScriptDirectory, comparisonPathOld, comparisonPathNew, comparisonPathNewArtifacts, comparisonPathCompile;
        private readonly DateTime start;
        public BuildTargetComparer(BuildTargetExecutor buildTargetExecutor, string dataDirectory)
        {
            this.buildTargetExecutor = buildTargetExecutor;
            buildTargets = buildTargetExecutor.BuildTargets;
            //currentScriptDirectory = dataDirectory + "Scripts" + Path.DirectorySeparatorChar;
            currentSourceScriptDirectory = dataDirectory + "Source" + Path.DirectorySeparatorChar + "Scripts" + Path.DirectorySeparatorChar;
            comparisonPathOld = DataDirectory.ComparisonDirectoryPath + "Old" + Path.DirectorySeparatorChar;
            comparisonPathNew = DataDirectory.ComparisonDirectoryPath + "New" + Path.DirectorySeparatorChar;
            comparisonPathNewArtifacts = DataDirectory.ComparisonDirectoryPath + "NewArtifacts" + Path.DirectorySeparatorChar;
            comparisonPathCompile = DataDirectory.ComparisonDirectoryPath + "Compile" + Path.DirectorySeparatorChar;
            start = DateTime.UtcNow;
        }

        private void Transpile(bool newOutput)
        {
            Console.WriteLine("Transpiling " + (newOutput ? "New" : "Old") + "...");
            BuildTargetExecutor.AllowNewOutput = newOutput;
            buildTargetExecutor.Transpile();
            bool copy = newOutput;///Copy so <see cref="CompileAndMoveArtifacts"/> and <see cref="ConsiderFinalComparison"/> comparison can work.
            MoveTranspiledFiles(buildTargets, newOutput ? comparisonPathNew : comparisonPathOld, copy);
        }

        //https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560
        private static int CalculateLevenshteinDistance(string source1, string source2) //O(n*m)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }

        private static string ReplaceRepeatedly(string haystack, string needle, string replacement)
        {
            while (haystack.Contains(needle))
            {
                haystack = haystack.Replace(needle, replacement);
            }
            return haystack;
        }

        private static readonly Regex singleLineCommentRegex = new Regex(@"(?<=[^/]|^);([^\r\n/]*|$)", RegexOptions.Compiled);
        //private static readonly Regex multiLineCommentStartRegex = new Regex("((\r\n)+|^);/(\r\n)+", RegexOptions.Compiled);
        //private static readonly Regex multiLineCommentEndRegex = new Regex("(\r\n)+/;(( *\r\n)+|$)", RegexOptions.Compiled);
        private static readonly Regex additionAssignmentRegex = new Regex("""\b([A-Za-z0-9_.]+) (\+|-)= ([0-9]+)(?=(\r\n|$))""", RegexOptions.Compiled);
        private static readonly Regex debugTraceRegex = new Regex("""Debug.Trace\(".+?"\)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex registerForSingleUpdateRegex = new Regex("""(self\.)?RegisterForSingleUpdate\([^)]+\)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static string GetTextForComparison(string text, bool isCurrentScript)
        {
            string revised = text;
            if (isCurrentScript)
            {
                //revised = revised.Replace(";/", "").Replace("/;", "");
                revised = TES5PropertySorter.ReplaceWithinHeaderOrFooter(revised);
                revised = revised.Replace("float timerInterval = 0.4", "")
                    .Replace("timer_p = (timer_p - timerInterval) as Int", "").Replace("timer_p = (timer_p - timerInterval)", "")
                    .Replace("timer_p -= timerInterval", "").Replace("timer_p += timerInterval", "")
                    .Replace(" - timerInterval)", " - 1000)").Replace(" + timerInterval)", " + 1000)")
                    .Replace(".GetValueInt()", ".GetValue()");
                revised = additionAssignmentRegex.Replace(revised, m => m.Groups[1].Value + " = " + m.Groups[1] + " " + m.Groups[2].Value + " " + m.Groups[3].Value);
            }
            revised = revised.Replace("timer_p = (timer_p - 1000)", "").Replace("timer_p = (timer_p + 1000)", "").Replace(" as Int", "").Replace(" as Float", "").Replace("Float timerInterval", "").Replace("timerInterval = 5", "");
            if (revised.Contains("IsAnimPlaying"))
            {
                revised = revised.Replace(" == 1", "");
                revised = revised.Replace(" == True", "");
                revised = revised.Replace(" == 0", "");
                revised = revised.Replace(" == False", "");
            }
            revised = debugTraceRegex.Replace(revised, "");
            Regex debugTraceStack = new Regex("""Debug.TraceStack\(".+?"\)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            revised = debugTraceStack.Replace(revised, "");
            revised = registerForSingleUpdateRegex.Replace(revised, "");
            revised = revised.Replace(" ", "").Replace("(", "").Replace(")", "");
            revised = singleLineCommentRegex.Replace(revised, "");
            revised = ReplaceRepeatedly(revised, "\r\n\r\n", "\r\n").Trim();
            return revised;
        }

        private static bool DoesOldEqualCurrentWithComments(string oldText, string currentText)
        {
            string[] oldLines = oldText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] currentLines = currentText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(l => l != ";/" && l != "/;").ToArray();
            return oldLines.SequenceEqual(currentLines, StringComparer.OrdinalIgnoreCase);
        }

        private static readonly string[] ignoreFileNames2 = [
            "TES4BeggarScript.psc", "TES4CGGoblinLogRun01SCRIPT.psc", "TES4CGMythicDawnAssassinGenericScript.psc", "TES4CGSewerDoor01Script.psc", "TES4CGSewerExteriorDoorScript.psc", "TES4CGTrigZoneEmperorBirthsignSCRIPT.psc", "TES4ClaudeMaricScript.psc", "TES4CTrigTripwire01SCRIPT.psc", "TES4DAAzuraVampScript.psc", "TES4DAMeridiaNecroScript.psc", "TES4DAMeridiaScript.psc", "TES4Dark01KnifeScript.psc", "TES4DarkAndreasGraveScript.psc", "TES4DarkCaeliaGraveScript.psc", "TES4DarkMatthiasGraveScript.psc", "TES4DarkSibyllaGraveScript.psc", "TES4EmfridScript.psc", "TES4FGC03ThiefScript.psc", "TES4FGC06GoblinScript.psc", "TES4FGD08GoblinScript.psc", "TES4GarrusDarelliunSCRIPT.psc", "TES4GoblinTribeASCRIPT.psc", "TES4HenantierDreamScript.psc", "TES4HorsePCPriorMaborelsHorseScript.psc", "TES4JearlScript.psc", "TES4JulienneFanisSCRIPT.psc", "TES4KurtTestMagicEffect2.psc", "TES4MG03Script.psc", "TES4MG05Script.psc", "TES4MG08ZahrashaScript.psc", "TES4MG13VampScript.psc", "TES4MG14CreatureScript.psc", "TES4MQ06ShrineExteriorDoorSCRIPT.psc", "TES4MQ10BrumaGuardSCRIPT.psc", "TES4MQ15GrottoExitScript.psc", "TES4MS02LorgrenDiaryScrapScript.psc", "TES4MS02LorgrenScript.psc", "TES4MS04TsarrinaScript.psc", "TES4MS05DreamworldBedScript.psc", "TES4MS06ManaclesKeyScript.psc", "TES4MS08FGgateSwitchBeforeSCRIPT.psc", "TES4MS08HuntersRunInnerDoorScript.psc", "TES4MS08OrcHunterScript.psc", "TES4MS08UrsanneLocheScript.psc", "TES4MS14BrushScript.psc", "TES4MS14PortalCanvasScript.psc", "TES4MS16BScript.psc", "TES4MS16Script.psc", "TES4MS18TemplateScriptWave01.psc", "TES4MS18TemplateScriptWave02.psc", "TES4MS18TemplateScriptWave03.psc", "TES4MS22ThugScript.psc", "TES4MS23RolandScript.psc", "TES4MS26AudensAvidius.psc", "TES4MS29AgarmirScript.psc", "TES4MS31LoadDoorScript.psc", "TES4MS31LowerDoor.psc", "TES4MS37GladeDoorScript.psc", "TES4RoliandHanusScript.psc", "TES4SapPumpREF01Script.psc", "TES4SaveriFaramScript.psc", "TES4SdrassaSCRIPT.psc", "TES4SEAichan01Boss01SCRIPT.psc", "TES4SEMarkTestSpell.psc", "tes4spellmakingstationscript.psc", "TES4SQ02RingScript.psc", "TES4TG01BestThiefScript.psc", "TES4TG10JaksDiaryScript.psc",
            "TES475C58870776EDD4AD198E41C3D0D7A59.psc", "TES4ARTallWallCarvingAnim01SCRIPT.psc", "TES4DAAzuraScript.psc", "TES4DarkUngolimScript.psc", "TES4DASanguineChestScript.psc", "TES4ElvenStatueScript.psc", "TES4FXPlasmaCloud01SCRIPT.psc", "TES4GoblinWarsScript.psc", "TES4HighFaneBookScript.psc", "TES475C58870776EDD4AD198E41C3D0D7A59.psc", "TES4ARTallWallCarvingAnim01SCRIPT.psc", "TES4DAAzuraScript.psc", "TES4DarkUngolimScript.psc", "TES4DASanguineChestScript.psc", "TES4ElvenStatueScript.psc", "TES4FXPlasmaCloud01SCRIPT.psc", "TES4GoblinWarsScript.psc", "TES4HighFaneBookScript.psc", "TES4MS23SeridurScript.psc", "TES4MS39NirnrootScript.psc", "TES4MS94OblivionGateScript.psc", "TES4ontusvaninschestscript.psc", "TES4TG04AhdarjisRingScript.psc", "TES4TG11cHiddenStairsScript.psc", "TES4TrigZonePlayerDoonce01BrumaSCRIPT.psc",
            "TES4_TIF__01002DAC.psc", "TES4_TIF__01002DE1.psc", "TES4_TIF__01003A7B.psc", "TES4_TIF__0100A0FE.psc", "TES4_TIF__0100A1B5.psc", "TES4_TIF__0100Bed7.psc", "TES4_tif__0100c220.psc", "TES4_tif__01014739.psc", "TES4_TIF__010151F6.psc", "TES4_TIF__01015206.psc", "TES4_TIF__01015207.psc", "TES4_TIF__0101544A.psc", "TES4_TIF__0101544E.psc", "TES4_TIF__0101544F.psc", "TES4_TIF__01015450.psc", "TES4_TIF__01015452.psc", "TES4_TIF__01015454.psc", "TES4_TIF__01015457.psc", "TES4_TIF__01016482.psc", "TES4_TIF__01018233.psc", "TES4_TIF__0101BB35.psc", "TES4_TIF__0101CB75.psc", "TES4_TIF__0101CB76.psc", "TES4_TIF__0101CB77.psc", "TES4_TIF__0101CB78.psc", "TES4_TIF__0101CB79.psc", "TES4_TIF__0101CB7A.psc", "TES4_TIF__0101CB7B.psc", "TES4_TIF__0101DC2E.psc", "TES4_TIF__0101E8E9.psc", "TES4_TIF__0101E90E.psc", "TES4_TIF__0101Ebb2.psc", "TES4_TIF__0101Ebb7.psc", "TES4_TIF__0101Ebb8.psc", "TES4_TIF__0101EbbD.psc", "TES4_TIF__0101EBBe.psc", "TES4_TIF__0101EBC2.psc", "TES4_TIF__0101EBC8.psc", "TES4_TIF__0101EBCC.psc", "TES4_TIF__0101EBcf.psc", "TES4_TIF__0101EBD5.psc", "TES4_TIF__0101EE3D.psc", "TES4_TIF__0101EE3E.psc", "TES4_TIF__0101EF75.psc", "TES4_TIF__0101EFE2.psc", "TES4_TIF__0101F875.psc", "TES4_TIF__0101F876.psc", "TES4_TIF__0101F877.psc", "TES4_TIF__0101F878.psc", "TES4_TIF__0101F879.psc", "TES4_TIF__0101F87A.psc", "TES4_TIF__01020073.psc", "TES4_TIF__010224AC.psc", "TES4_TIF__010224B4.psc", "TES4_TIF__01022CF6.psc", "TES4_TIF__01022DA6.psc", "TES4_TIF__01022DA7.psc", "TES4_TIF__01022DA8.psc", "TES4_TIF__01022DA9.psc", "TES4_TIF__01022DAA.psc", "TES4_TIF__01022Dab.psc", "TES4_TIF__01022EF2.psc", "TES4_TIF__0102366A.psc", "TES4_TIF__0102366B.psc", "TES4_TIF__01024851.psc", "TES4_TIF__01024EA2.psc", "TES4_TIF__01026904.psc", "TES4_TIF__01026967.psc", "TES4_TIF__01026A9B.psc", "TES4_TIF__01026A9E.psc", "TES4_TIF__01026AA2.psc", "TES4_TIF__01026AA3.psc", "TES4_TIF__01026AA4.psc", "TES4_TIF__01026AA8.psc", "TES4_TIF__01026AA9.psc", "TES4_TIF__01026DDF.psc", "TES4_TIF__0102809C.psc", "TES4_TIF__0102820C.psc", "TES4_tif__01028273.psc", "TES4_TIF__01028865.psc", "TES4_tif__0102a999.psc", "TES4_tif__0102a99b.psc", "TES4_tif__0102a99c.psc", "TES4_tif__0102ada0.psc", "TES4_TIF__0102B1C9.psc", "TES4_TIF__0102B1CA.psc", "TES4_TIF__0102B45E.psc", "TES4_tif__0102c4ea.psc", "TES4_TIF__0102C51A.psc", "TES4_TIF__0102D918.psc", "TES4_TIF__0102D91D.psc", "TES4_TIF__0102EE2E.psc", "TES4_TIF__0102FBB1.psc", "TES4_TIF__01031521.psc", "TES4_TIF__01032472.psc", "TES4_TIF__01032473.psc", "TES4_TIF__01032474.psc", "TES4_TIF__01032475.psc", "TES4_TIF__01032476.psc", "TES4_TIF__01032477.psc", "TES4_TIF__01032478.psc", "TES4_TIF__01032479.psc", "TES4_TIF__0103247A.psc", "TES4_TIF__0103247B.psc", "TES4_TIF__0103247C.psc", "TES4_TIF__0103247D.psc", "TES4_TIF__0103247E.psc", "TES4_TIF__0103267A.psc", "TES4_TIF__01032B0E.psc", "TES4_TIF__0103378F.psc", "TES4_TIF__01034877.psc", "TES4_TIF__01034E96.psc", "TES4_TIF__01035548.psc", "TES4_TIF__0103565F.psc", "TES4_TIF__01035672.psc", "TES4_TIF__01035676.psc", "TES4_TIF__01035A14.psc", "TES4_TIF__01035DD9.psc", "TES4_tif__01037ff7.psc", "TES4_tif__01038b35.psc", "TES4_tif__01038ed3.psc", "TES4_tif__01038ed7.psc", "TES4_tif__0103a8fd.psc", "TES4_tif__0103a8fe.psc", "TES4_tif__0103a915.psc", "TES4_tif__0103a916.psc", "TES4_tif__0103a917.psc", "TES4_TIF__0103AB35.psc", "TES4_tif__0103b1d8.psc", "TES4_TIF__0103C2A6.psc", "TES4_TIF__0103C2A7.psc", "TES4_TIF__0103C2C4.psc", "TES4_TIF__0103C339.psc", "TES4_TIF__0103C34A.psc", "TES4_TIF__0103C371.psc", "TES4_TIF__0103C379.psc", "TES4_TIF__0103CCAC.psc", "TES4_tif__0103d62b.psc", "TES4_TIF__0103E950.psc", "TES4_TIF__0103ED37.psc", "TES4_TIF__01046303.psc", "TES4_TIF__01046304.psc", "TES4_TIF__010463AA.psc", "TES4_TIF__010463AB.psc", "TES4_TIF__010463BB.psc", "TES4_TIF__0104F220.psc", "TES4_TIF__01053735.psc", "TES4_TIF__01053736.psc", "TES4_TIF__01053737.psc", "TES4_TIF__0105377A.psc", "TES4_tif__0106245b.psc", "TES4_TIF__0106E2DB.psc", "TES4_TIF__0106E2DC.psc", "TES4_TIF__0106E2E3.psc", "TES4_TIF__0106E2E5.psc", "TES4_TIF__0106E2EB.psc", "TES4_TIF__01079441.psc", "TES4_TIF__01079442.psc", "TES4_TIF__01084AA0.psc", "TES4_TIF__0108511C.psc", "TES4_TIF__0108511D.psc", "TES4_TIF__0108585E.psc", "TES4_TIF__0108587C.psc", "TES4_TIF__01089938.psc", "TES4_TIF__0108Adc4.psc", "TES4_TIF__0108C220.psc", "TES4_TIF__0108C227.psc", "TES4_TIF__0108C22A.psc", "TES4_TIF__0108C231.psc", "TES4_TIF__0108C242.psc", "TES4_TIF__0108C248.psc", "TES4_TIF__01091AAB.psc", "TES4_TIF__01091C5B.psc", "TES4_TIF__01095898.psc", "TES4_TIF__01095A67.psc", "TES4_TIF__010977D7.psc", "TES4_TIF__01098120.psc", "TES4_TIF__0109813C.psc", "TES4_TIF__0109813D.psc", "TES4_TIF__0109813E.psc", "TES4_TIF__0109813F.psc", "TES4_TIF__010981F1.psc", "TES4_TIF__0109E641.psc", "TES4_TIF__0109E64F.psc", "TES4_TIF__0109E65B.psc", "TES4_TIF__010A1F62.psc", "TES4_tif__010aed44.psc", "TES4_tif__010aed45.psc", "TES4_tif__010aed46.psc", "TES4_tif__010aed4e.psc", "TES4_TIF__010B984F.psc", "TES4_TIF__0112DD32.psc", "TES4_TIF__0118AFE0.psc",
            "HSQT_TIF__0104ED8D.psc", "HSQT_TIF__0104F213.psc", "HSQT_TIF__0104F215.psc", "HSQT_TIF__0104F219.psc", "HSQT_TIF__010531BC.psc", "HSQT_TIF__010B97D9.psc", "HSQT_TIF__010B97E7.psc", "SKYBMethredhelsChestScript.psc", "SKYB_QF_TrainingBlade_0118BA2F.psc",
            "TES4ARGateDelay01SCRIPT.psc", "TES4DASheogorathRatScript.psc", "TES4GrayFoxScript.psc", "TES4MS51CarpetScript.psc", "TES4MS51PaintingScript.psc", "TES4RallusOdiilSCRIPT.psc", "TES4SapStorageTank04Script.psc", "TES4TG10JakbenFamilyHistoryScript.psc", "TES4_QF_TG10BloodPrice_010BF025.psc", "TES4_TIF__010982D9.psc",
            "TES4BarthelGernandScript.psc", "TES4BernadettePenelesScript.psc", "TES4CGMythicDawnAmbushB1Script.psc", "TES4CGMythicDawnAmbushB2Script.psc", "TES4CheydinhalMGBasementDoorScript.psc", "TES4DAMalacathOgreScript.psc", "TES4DavideSurilieScript.psc", "TES4HannibalTravenScript.psc", "TES4MQ15MythicDawnPrisonerSCRIPT.psc", "TES4MS29ThoronirScript.psc", "TES4PublicanDrunkenDragonAndreas.psc", "TES4ToutiusSextiusScript.psc", "TES4TutorialScript.psc", "TES4_TIF__01095899.psc",
            "TES4_TIF__01002DB9.psc", "TES4_TIF__01002DBB.psc", "TES4_TIF__01002DCF.psc", "TES4_TIF__01002DD2.psc", "TES4_TIF__01002DD5.psc", "TES4_TIF__01002DD6.psc", "TES4_TIF__01002DD8.psc", "TES4_TIF__010034AA.psc", "TES4_TIF__01003999.psc", "TES4_TIF__01003A6A.psc", "TES4_TIF__01003A78.psc", "TES4_TIF__01009279.psc", "TES4_TIF__01009294.psc", "TES4_TIF__0100A09F.psc", "TES4_TIF__0100A0DA.psc", "TES4_TIF__0100A0DB.psc", "TES4_TIF__0100Bcd1.psc", "TES4_TIF__0100Bcd2.psc", "TES4_TIF__0100C1EB.psc", "TES4_TIF__0100C1EE.psc", "TES4_TIF__0100C1EF.psc", "TES4_TIF__0100C20A.psc", "TES4_TIF__010146C8.psc", "TES4_TIF__010146ED.psc", "TES4_TIF__01014709.psc", "TES4_TIF__010151E6.psc", "TES4_TIF__010151E8.psc", "TES4_TIF__010151EA.psc", "TES4_TIF__01015258.psc", "TES4_TIF__0101525A.psc", "TES4_TIF__0101525C.psc", "TES4_TIF__0101525D.psc", "TES4_TIF__0101525F.psc", "TES4_TIF__01015260.psc", "TES4_TIF__01015263.psc", "TES4_TIF__01015265.psc", "TES4_TIF__01015DF8.psc", "TES4_TIF__01015DF9.psc", "TES4_TIF__01015DFA.psc", "TES4_TIF__01015DFB.psc", "TES4_TIF__01015DFC.psc", "TES4_TIF__01015DFD.psc", "TES4_TIF__01015DFE.psc", "TES4_TIF__01015DFF.psc", "TES4_TIF__0101B948.psc", "TES4_TIF__0101BB21.psc", "TES4_TIF__0101BB31.psc", "TES4_TIF__0101C497.psc", "TES4_TIF__0101CF37.psc", "TES4_TIF__0101D037.psc", "TES4_TIF__0101D23F.psc", "TES4_TIF__0101DA53.psc", "TES4_TIF__0101E852.psc", "TES4_TIF__0101E859.psc", "TES4_TIF__0101E8DA.psc", "TES4_TIF__0101E8DB.psc", "TES4_TIF__0101E8DC.psc", "TES4_TIF__0101EE2E.psc", "TES4_TIF__0101FB6F.psc", "TES4_TIF__0101FDBe.psc", "TES4_TIF__0101Fee5.psc", "TES4_TIF__01020093.psc", "TES4_TIF__01022B3C.psc", "TES4_TIF__0102366F.psc", "TES4_TIF__01023683.psc", "TES4_TIF__01023E48.psc", "TES4_TIF__01023E4B.psc", "TES4_TIF__01023F33.psc", "TES4_TIF__01023F35.psc", "TES4_TIF__01023F36.psc", "TES4_TIF__01023F37.psc", "TES4_TIF__01024853.psc", "TES4_TIF__01025274.psc", "TES4_TIF__01025275.psc", "TES4_TIF__010253B6.psc", "TES4_TIF__01026EF0.psc", "TES4_TIF__01026EF1.psc", "TES4_TIF__01026EF2.psc", "TES4_TIF__01026EF3.psc", "TES4_TIF__010275C6.psc", "TES4_TIF__010275C7.psc", "TES4_TIF__010275C8.psc", "TES4_TIF__010275CA.psc", "TES4_TIF__0102798F.psc", "TES4_TIF__01027996.psc", "TES4_TIF__01027F34.psc", "TES4_TIF__01027F88.psc", "TES4_TIF__01027F89.psc", "TES4_TIF__01027F8A.psc", "TES4_TIF__01027FC8.psc", "TES4_TIF__01027FC9.psc", "TES4_TIF__010280C1.psc", "TES4_TIF__0102819D.psc", "TES4_TIF__010281B9.psc", "TES4_TIF__01028264.psc", "TES4_TIF__01028695.psc", "TES4_TIF__01028696.psc", "TES4_TIF__0102869C.psc", "TES4_TIF__01028702.psc", "TES4_TIF__01028710.psc", "TES4_TIF__0102879C.psc", "TES4_TIF__01028BeB.psc", "TES4_TIF__01028CEA.psc", "TES4_TIF__01028D34.psc", "TES4_TIF__01028F4C.psc", "TES4_TIF__0102AD1B.psc", "TES4_TIF__0102AD1C.psc", "TES4_TIF__0102B09C.psc", "TES4_TIF__0102B524.psc", "TES4_TIF__0102B525.psc", "TES4_TIF__0102C527.psc", "TES4_TIF__0102C595.psc", "TES4_TIF__0102C5Be.psc", "TES4_TIF__0102C5C0.psc", "TES4_TIF__0102C622.psc", "TES4_TIF__0102C6D0.psc", "TES4_TIF__0102C6DB.psc", "TES4_TIF__0102D3A3.psc", "TES4_TIF__0102D6E7.psc", "TES4_TIF__0102D923.psc", "TES4_TIF__0102E5CE.psc", "TES4_TIF__0102E91D.psc", "TES4_TIF__0102EE2A.psc", "TES4_TIF__0102EE2C.psc", "TES4_TIF__0102EE2D.psc", "TES4_TIF__0102EE6B.psc", "TES4_TIF__0102EE6F.psc", "TES4_TIF__0102FBAF.psc", "TES4_TIF__0102FF0B.psc", "TES4_TIF__0102FF10.psc", "TES4_TIF__0102FF13.psc", "TES4_TIF__01030206.psc", "TES4_TIF__01030207.psc", "TES4_TIF__010314FA.psc", "TES4_TIF__01031506.psc", "TES4_TIF__0103150F.psc", "TES4_TIF__01031520.psc", "TES4_TIF__0103246C.psc", "TES4_TIF__01033D11.psc", "TES4_TIF__01033Dcb.psc", "TES4_TIF__01033DCE.psc", "TES4_TIF__01033DCF.psc", "TES4_TIF__01033DD0.psc", "TES4_TIF__01033DD1.psc", "TES4_TIF__01033DD2.psc", "TES4_TIF__01033DD3.psc", "TES4_TIF__01033DD4.psc", "TES4_TIF__01033DD5.psc", "TES4_TIF__01033DD6.psc", "TES4_TIF__010352A5.psc", "TES4_TIF__01035592.psc", "TES4_TIF__0103569F.psc", "TES4_TIF__010356E0.psc", "TES4_TIF__01035A15.psc", "TES4_TIF__01035C9A.psc", "TES4_TIF__01035C9D.psc", "TES4_TIF__01035CA5.psc", "TES4_TIF__01035CA7.psc", "TES4_TIF__01035D8D.psc", "TES4_TIF__01035D8F.psc", "TES4_TIF__01035E06.psc", "TES4_TIF__01035E07.psc", "TES4_TIF__010363Be.psc", "TES4_TIF__010363C4.psc", "TES4_TIF__010363C6.psc", "TES4_TIF__010363D3.psc", "TES4_TIF__01037B4F.psc", "TES4_TIF__01037B51.psc", "TES4_TIF__01037C38.psc", "TES4_TIF__01037C39.psc", "TES4_TIF__01037C3A.psc", "TES4_TIF__01037C3B.psc", "TES4_TIF__01037C3C.psc", "TES4_TIF__01037CFB.psc", "TES4_TIF__01037CFD.psc", "TES4_TIF__0103842B.psc", "TES4_TIF__0103858C.psc", "TES4_TIF__01038B06.psc", "TES4_TIF__0103AB59.psc", "TES4_TIF__0103ABF9.psc", "TES4_TIF__0103B348.psc", "TES4_TIF__0103B34B.psc", "TES4_TIF__0103B34D.psc", "TES4_TIF__0103C2A8.psc", "TES4_TIF__0103C2B5.psc", "TES4_TIF__0103C2CB.psc", "TES4_TIF__0103C350.psc", "TES4_TIF__0103C375.psc", "TES4_TIF__0103D30D.psc", "TES4_TIF__0103E8F7.psc", "TES4_TIF__01047373.psc", "TES4_TIF__0104CE45.psc", "TES4_TIF__0104CE49.psc", "TES4_TIF__0104CE4A.psc", "TES4_TIF__0104CE4B.psc", "TES4_TIF__0104CF8A.psc", "TES4_TIF__0104CF8B.psc", "TES4_TIF__0104E919.psc", "TES4_TIF__0104E91C.psc", "TES4_TIF__0104E920.psc", "TES4_TIF__0104E92C.psc", "TES4_TIF__0104F12B.psc", "TES4_TIF__0104F235.psc", "TES4_TIF__0104F78F.psc", "TES4_TIF__0104F8C5.psc", "TES4_TIF__0105179B.psc", "TES4_TIF__0105179C.psc", "TES4_TIF__0105179D.psc", "TES4_TIF__0105373D.psc", "TES4_TIF__0105373E.psc", "TES4_TIF__0105373F.psc", "TES4_TIF__01053740.psc", "TES4_TIF__01053774.psc", "TES4_TIF__0105377D.psc", "TES4_TIF__0105377E.psc", "TES4_TIF__0105402E.psc", "TES4_TIF__0105402F.psc", "TES4_TIF__01054030.psc", "TES4_TIF__01054038.psc", "TES4_TIF__01062445.psc", "TES4_TIF__01067DD1.psc", "TES4_TIF__0106B1B6.psc", "TES4_TIF__0106B1B7.psc", "TES4_TIF__0106B1B8.psc", "TES4_TIF__0106E2DD.psc", "TES4_TIF__0106E2E4.psc", "TES4_TIF__01071E76.psc", "TES4_TIF__010836D0.psc", "TES4_TIF__01084B4A.psc", "TES4_TIF__0108585F.psc", "TES4_TIF__01085860.psc", "TES4_TIF__01085861.psc", "TES4_TIF__01085863.psc", "TES4_TIF__01085865.psc", "TES4_TIF__01085973.psc", "TES4_TIF__01085A65.psc", "TES4_TIF__01085A69.psc", "TES4_TIF__01085A6B.psc", "TES4_TIF__0108B600.psc", "TES4_TIF__0108B601.psc", "TES4_TIF__0108B618.psc", "TES4_TIF__0108C21D.psc", "TES4_TIF__0108DC3E.psc", "TES4_TIF__01090AB5.psc", "TES4_TIF__01090B73.psc", "TES4_TIF__01090B74.psc", "TES4_TIF__01090B75.psc", "TES4_TIF__01091C4F.psc", "TES4_TIF__0109354C.psc", "TES4_TIF__0109354D.psc", "TES4_TIF__0109354E.psc", "TES4_TIF__0109354F.psc", "TES4_TIF__010977D5.psc", "TES4_TIF__010977D8.psc", "TES4_TIF__010980FC.psc", "TES4_TIF__01098139.psc", "TES4_TIF__01098145.psc", "TES4_TIF__01098199.psc", "TES4_TIF__0109819E.psc", "TES4_TIF__010981A0.psc", "TES4_TIF__010981A1.psc", "TES4_TIF__01098397.psc", "TES4_TIF__0109EA94.psc", "TES4_TIF__010A3DF2.psc", "TES4_TIF__010A3DF4.psc", "TES4_TIF__010A6588.psc", "TES4_TIF__010A6589.psc", "TES4_TIF__010B1244.psc", "TES4_TIF__010B124A.psc", "TES4_TIF__010B124B.psc", "TES4_TIF__010B1255.psc", "TES4_TIF__010B1297.psc", "TES4_TIF__010B12A0.psc", "TES4_TIF__010B6C29.psc", "TES4_TIF__010B9841.psc", "TES4_TIF__010B9842.psc", "TES4_TIF__010B9844.psc", "TES4_TIF__010B9845.psc", "TES4_TIF__010B9849.psc", "TES4_TIF__010B984C.psc", "TES4_TIF__010BB9B6.psc", "TES4_TIF__010BB9B8.psc", "TES4_TIF__010BB9BA.psc", "TES4_TIF__010BB9C2.psc", "TES4_TIF__010BB9C5.psc", "TES4_TIF__010BF16B.psc", "TES4_TIF__010CBB73.psc", "TES4_TIF__01188F3E.psc", "TES4_TIF__01188F3F.psc", "TES4_TIF__01188F40.psc", "TES4_TIF__01188F41.psc", "TES4_TIF__0118AE86.psc", "TES4_TIF__0118Afd5.psc",
            "TES4MS23Script.psc", "TES4MS48InsideOblivionGateScript.psc", "TES4SECrimeScript.psc", "TES4SERakheranScript.psc", "TES4_TIF__01002DE2.psc", "TES4_TIF__01009295.psc", "TES4_TIF__010178C5.psc", "TES4_TIF__01018239.psc", "TES4_TIF__0101B92E.psc", "TES4_TIF__0101DC31.psc", "TES4_TIF__0102366E.psc", "TES4_TIF__01023FF2.psc", "TES4_TIF__01023FF5.psc", "TES4_TIF__01024829.psc", "TES4_TIF__010275CC.psc", "TES4_TIF__01027F86.psc", "TES4_TIF__0102809B.psc", "TES4_TIF__010288AE.psc", "TES4_TIF__010288B2.psc", "TES4_TIF__01028C03.psc", "TES4_TIF__0102B463.psc", "TES4_TIF__0102B51F.psc", "TES4_TIF__0102B522.psc", "TES4_TIF__0102B527.psc", "TES4_TIF__0102B5E9.psc", "TES4_TIF__0102D282.psc", "TES4_TIF__0102D283.psc", "TES4_TIF__0102D284.psc", "TES4_TIF__0102D286.psc", "TES4_TIF__0102D287.psc", "TES4_TIF__0102D288.psc", "TES4_TIF__0102D289.psc", "TES4_TIF__0102E5E5.psc", "TES4_TIF__0102F9E9.psc", "TES4_TIF__010300A4.psc", "TES4_TIF__01034E12.psc", "TES4_TIF__01035648.psc", "TES4_TIF__01035707.psc", "TES4_TIF__01035F50.psc", "TES4_TIF__01037B50.psc", "TES4_TIF__0103C29C.psc", "TES4_TIF__0103C33A.psc", "TES4_TIF__0103D4AC.psc", "TES4_TIF__0104120F.psc", "TES4_TIF__0104E918.psc", "TES4_TIF__0104E91B.psc", "TES4_TIF__0104E91F.psc", "TES4_TIF__0104E929.psc", "TES4_TIF__01056E6A.psc", "TES4_TIF__01056E6C.psc", "TES4_TIF__01070487.psc", "TES4_TIF__010732B3.psc", "TES4_TIF__010844C3.psc", "TES4_TIF__0108B619.psc", "TES4_TIF__01091C5A.psc", "TES4_TIF__0109532D.psc", "TES4_TIF__0109811F.psc", "TES4_TIF__01098154.psc", "TES4_TIF__0118D1FE.psc", "TES4_TIF__0118D1FF.psc", "TES4_TIF__0118D200.psc", "TES4_TIF__0118D201.psc", "TES4_TIF__0118D202.psc", "TES4_TIF__0118D203.psc",
            "TES4MQ03OblivionSigilKeeperSCRIPT.psc", "TES4_TIF__0101E8AD.psc", "TES4_TIF__0102482A.psc", "TES4_TIF__0102482B.psc", "TES4_TIF__01024836.psc", "TES4_TIF__01024839.psc", "TES4_TIF__0102483A.psc", "TES4_TIF__0102483C.psc", "TES4_TIF__0102483F.psc", "TES4_TIF__0102485B.psc", "TES4_TIF__01098143.psc"
            ];
        private static readonly List<string> ignoreFileNames = [];
        private static readonly string[] disallowNearMatch = ["TES4_TIF__0101C1CC.psc", "TES4_TIF__0101C1CD.psc", "TES4_TIF__0101C1CE.psc", "TES4_TIF__0101C1CF.psc"];//These files have potentially useful Debug.Trace calls.
        private (List<string> FileNamesWithMatchingOldContent, List<string> FileNamesWithNonMatchingNewContent) Compare(bool openTextComparer)
        {
            string[] currentScriptPaths = Directory.GetFiles(currentSourceScriptDirectory);
            List<string> fileNamesWithMatchingOldContent = [], fileNamesWithNonMatchingNewContent = [];
            //Regex extendsTypeConditional = new Regex("( Extends [A-Za-z]+) Conditional", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //Regex autoConditionalTES4FormID = new Regex(" Auto Conditional;TES4FormID:[0-9]+;\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            ProgressWriter.Loop("Comparing Files", currentScriptPaths, currentScriptPath =>
            {
                string fileName = Path.GetFileName(currentScriptPath);
                string oldFilePath = comparisonPathOld + fileName;
                string? oldText = FileSystemEntryUtility.ReadAllTextIfExists(oldFilePath);
                bool usingOldName = false;
                if (oldText == null)
                {
                    string oldFilePathTemp = comparisonPathOld + "TES4" + fileName.Substring(4);
                    oldText = FileSystemEntryUtility.ReadAllTextIfExists(oldFilePathTemp);
                    if (oldText != null)
                    {
                        usingOldName = true;
                        oldFilePath = oldFilePathTemp;
                    }
                }
                if (oldText != null)
                {
                    string currentText = File.ReadAllText(currentScriptPath);
                    string currentTextForComparison = GetTextForComparison(currentText, true);
                    string oldTextForComparison = GetTextForComparison(oldText, false);
                    bool isExactMatch = oldText == currentText;
                    bool isNearMatch = oldTextForComparison.Equals(currentTextForComparison, StringComparison.OrdinalIgnoreCase) &&
                        Math.Abs(oldText.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries).Length - currentText.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries).Length) <= 1;
                    if (isExactMatch || (isNearMatch && !disallowNearMatch.Contains(fileName)))
                    {
                        if (usingOldName)
                        {
                            throw new InvalidOperationException("Will any SKYB file be a close match?");
                        }
                        if (openTextComparer && !isExactMatch && isNearMatch)
                        {
                            //OpenTextComparer(oldFilePath, currentScriptPath);
                        }
                        fileNamesWithMatchingOldContent.Add(fileName);
                        string newText = File.ReadAllText(comparisonPathNew + fileName);
                        if (newText != currentText)
                        {
                            fileNamesWithNonMatchingNewContent.Add(fileName);
                        }
                    }
                    else if (openTextComparer)
                    {
                        if (DoesOldEqualCurrentWithComments(oldText, currentText)) { }//Commented file
                        else
                        {
                            if (
                                Math.Abs(oldTextForComparison.Length - currentTextForComparison.Length) < 200 &&
                                !(ignoreFileNames2.Contains(fileName) || disallowNearMatch.Contains(fileName))
                                )
                            {
                                int ld = CalculateLevenshteinDistance(oldTextForComparison, currentTextForComparison);
                                if (ld < 200 || currentScriptPath.Contains("SE"))
                                {
                                    OpenTextComparer(oldFilePath, currentScriptPath);
                                    string? properties = TES5PropertySorter.ExtractAndSortProperties(currentText);
                                    //Consider copying above to clipboard and prompting with three options: (1) consider files equivalent, (2) add to ignore list, (3) skip file
                                    ignoreFileNames.Add(fileName);
                                }
                            }
                        }
                    }
                }
            });
            string[] pscBakFiles = Directory.GetFiles(currentSourceScriptDirectory, "*.psc.bak");
            if (pscBakFiles.Length > 0) { ProgressWriter.Loop("Deleting *.psc.bak Files (from WinMerge)", pscBakFiles, File.Delete); }
            ///2025-08-13: fileNamesWithMatchingOldContent.Count = 6596
            Console.WriteLine(fileNamesWithMatchingOldContent.Count + " old files matched current files.");
            Console.WriteLine("Of those, " + fileNamesWithNonMatchingNewContent.Count + " new files differ from current files.");
            return (fileNamesWithMatchingOldContent, fileNamesWithNonMatchingNewContent);
        }

        private void CompileAndMoveArtifacts(IReadOnlyCollection<string> fileNamesWithNonMatchingNewContent)
        {
            buildTargetExecutor.Compile(afterPrepareWorkspace: () =>
            {
                Console.WriteLine("Deleting Unneeded Transpiled Files...");
                foreach (var buildTarget in buildTargets)
                {
                    foreach (var transpiledFilePath in Directory.GetFiles(buildTarget.GetTranspiledPath()))
                    {
                        string fileName = Path.GetFileName(transpiledFilePath);
                        if (!fileNamesWithNonMatchingNewContent.Where(f => f.Equals(fileName, StringComparison.OrdinalIgnoreCase)).Any())
                        {
                            File.Delete(transpiledFilePath);
                        }
                    }
                }
                Console.WriteLine("Compiling...");
            });
            MoveArtifactsFiles(buildTargets, comparisonPathNewArtifacts, false);
        }

        private void MoveToCurrent(List<string> fileNamesWithMatchingOldContent, List<string> fileNamesWithNonMatchingNewContent)
        {
            if (fileNamesWithNonMatchingNewContent.Count > 0)
            {
                Console.Write("These " + fileNamesWithNonMatchingNewContent.Count + " new files will overwrite the current files. Press any key to continue.");
                Console.ReadKey(true);
                Console.WriteLine();
#if INCLUDEPEX//Robert said he doesn't want these.
                CompileAndMoveArtifacts(fileNamesWithNonMatchingNewContent);
#endif
                ProgressWriter.Loop("Moving Files to Current Directory", fileNamesWithNonMatchingNewContent, matchingFileName =>
                {
                    string pscNewPath = comparisonPathNew + matchingFileName;
                    string pscCurrentPath = currentSourceScriptDirectory + matchingFileName;
                    FileSystemEntryUtility.MoveOverwrite(pscNewPath, pscCurrentPath);
#if INCLUDEPEX
                    string pexFileName = Path.GetFileNameWithoutExtension(matchingFileName) + ".pex";
                    string pexNewPath = comparisonPathNewArtifacts + pexFileName;
                    string pexCurrentPath = currentScriptDirectory + pexFileName;
                    FileSystemEntryUtility.MoveOverwrite(pexNewPath, pexCurrentPath);
#endif
                });
                Console.WriteLine("New files have been copied to the current directory.");
            }
        }

        private bool PerformInitialComparison()
        {
            Compare(false);
            Console.Write("Would you like to continue? ");
            bool yes = Console.ReadKey(true).Key == ConsoleKey.Y;
            Console.WriteLine();
            return yes;
        }

        private void CompareAndMoveToCurrent()
        {
            var (fileNamesWithMatchingOldContent, fileNamesWithNonMatchingNewContent) = Compare(true);
            MoveToCurrent(fileNamesWithMatchingOldContent, fileNamesWithNonMatchingNewContent);
        }

        private void ConsiderFinalComparison()
        {
            while (true)
            {
                Console.WriteLine("Would you like to run a final comparison using the new output?");
                Console.WriteLine("This is useful if any manual changes were made.");
                if (Console.ReadKey(true).Key != ConsoleKey.Y) { break; }
                Directory.Delete(comparisonPathOld, true);
                Directory.Move(comparisonPathNew, comparisonPathOld);
                Transpile(true);
                CompareAndMoveToCurrent();
            }
        }

        private void ConsiderFinalCompilation()
        {
            string[] newFilePaths = Directory.EnumerateFiles(currentSourceScriptDirectory).Where(f => new FileInfo(f).LastWriteTimeUtc >= start).ToArray();
            if (newFilePaths.Length > 0)
            {
                Console.WriteLine("Would you like to compile the " + newFilePaths.Length + " files in the current directory that were modified at or after " + start.ToString("HH:mm:ss") + "?");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    FileSystemEntryUtility.CreateDirectoryEmpty(comparisonPathCompile);
                    Build build = new Build(Build.DEFAULT_BUILD_PATH);
                    PrepareWorkspaceJob prepareWorkspaceJob = new PrepareWorkspaceJob(build, buildTargets);
                    prepareWorkspaceJob.Run(new ProgressWriter("Preparing Workspace (Initial)", 10));
                    //This doesn't quite work yet. I need to copy Source/Scripts into Workspace.
                    ProgressWriter.Loop("Copying Files to Compilation Folder", newFilePaths, filePath =>
                    {
                        File.Copy(filePath, comparisonPathCompile + Path.DirectorySeparatorChar + Path.GetFileName(filePath), true);
                    });
                    buildTargetExecutor.Compile([new BuildTargetForCompiler(comparisonPathCompile, comparisonPathNewArtifacts)]);
                }
            }
        }

        public void Run()
        {
            Console.WriteLine("Deleting Previous Comparison Directories...");
            FileSystemEntryUtility.DeleteDirectoryIfExists(DataDirectory.ComparisonDirectoryPath);
            IReadOnlyList<BuildTarget> buildTargets = buildTargetExecutor.BuildTargets;
            Transpile(false);
            Console.WriteLine("Deleting Build Files...");
            buildTargets.DeleteBuildFiles();
            Transpile(true);
            if (PerformInitialComparison())
            {
                CompareAndMoveToCurrent();
                ConsiderFinalComparison();
                ConsiderFinalCompilation();
            }
            Console.WriteLine("Deleting Comparison Directories...");
            Directory.Delete(DataDirectory.ComparisonDirectoryPath, true);
            Console.WriteLine("Deleting Build Files...");
            buildTargets.DeleteBuildFiles();
        }

        private static void MoveFiles(IReadOnlyCollection<BuildTarget> buildTargets, Func<BuildTarget, string> getPath, string destinationDirectory, bool copy)
        {
            FileSystemEntryUtility.CreateDirectoryEmpty(destinationDirectory);
            ProgressWriter.Loop("Moving Files", buildTargets, buildTarget =>
            {
                FileSystemEntryUtility.MoveOrCopyFiles(getPath(buildTarget), destinationDirectory, copy);
            });
        }
        private static void MoveTranspiledFiles(IReadOnlyCollection<BuildTarget> buildTargets, string destinationDirectory, bool copy)
        {
            MoveFiles(buildTargets, bt => bt.GetTranspiledPath(), destinationDirectory, copy);
        }
        private static void MoveArtifactsFiles(IReadOnlyCollection<BuildTarget> buildTargets, string destinationDirectory, bool copy)
        {
            MoveFiles(buildTargets, bt => bt.GetArtifactsPath(), destinationDirectory, copy);
        }

        private static void OpenTextComparer(string path1, string path2)
        {
            using (Process.Start(@"C:\Program Files (x86)\WinMerge\WinMergeU.exe", "\"" + path1 + "\" \"" + path2 + "\"")) { }
        }
    }
}
