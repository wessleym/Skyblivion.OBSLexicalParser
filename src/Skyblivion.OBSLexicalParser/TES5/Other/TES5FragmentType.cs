namespace Skyblivion.OBSLexicalParser.TES5.Other
{
    class TES5FragmentType
    {
        private readonly string name;
        private TES5FragmentType(string name)
        {
            this.name = name;
        }

        public static readonly TES5FragmentType
            T_TIF = new TES5FragmentType("TIF"),
            T_QF = new TES5FragmentType("QF"),
            T_PF = new TES5FragmentType("PF");
    }
}