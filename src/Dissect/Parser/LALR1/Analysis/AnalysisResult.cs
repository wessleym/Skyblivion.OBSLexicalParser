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
        /*
        * Returns the handle-finding FSA.
        */
        public Automaton Automaton { get; protected set; }
        /*
        * Returns the resulting parse table.
         *
         *  The parse table.
        */
        public ActionAndGoTo ParseTable { get; protected set; }
        /*
        * Returns an array of resolved parse table conflicts.
         *
         *  The conflicts.
        */
        public List<Conflict> ResolvedConflicts { get; protected set; }
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
            this.ParseTable = parseTable;
            this.Automaton = automaton;
            this.ResolvedConflicts = conflicts;
        }
    }
}