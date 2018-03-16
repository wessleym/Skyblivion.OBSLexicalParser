using System.Collections.Generic;

namespace Dissect.Parser.LALR1.Analysis
{
    /*
     * The result of a grammar analysis.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class AnalysisResult
    {
        protected Automaton automaton;
        protected ActionAndGoTo parseTable;
        protected List<Conflict> resolvedConflicts;
        /*
        * Constructor.
         *
         *  The parse table.
         * 
         *  An array of conflicts resolved during parse table
         * construction.
        */
        public AnalysisResult(ActionAndGoTo parseTable, Automaton automaton, List<Conflict> conflicts)
        {
            this.parseTable = parseTable;
            this.automaton = automaton;
            this.resolvedConflicts = conflicts;
        }

        /*
        * Returns the handle-finding FSA.
        */
        public Automaton getAutomaton()
        {
            return this.automaton;
        }

        /*
        * Returns the resulting parse table.
         *
         *  The parse table.
        */
        public ActionAndGoTo getParseTable()
        {
            return this.parseTable;
        }

        /*
        * Returns an array of resolved parse table conflicts.
         *
         *  The conflicts.
        */
        public List<Conflict> getResolvedConflicts()
        {
            return this.resolvedConflicts;
        }
    }
}