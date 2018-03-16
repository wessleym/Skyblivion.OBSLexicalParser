using Dissect.Parser.LALR1.Analysis;
using System;

namespace Dissect.Parser.LALR1.Dumper
{
    /*
     * Dumps the handle-finding FSA in the
     * format used by Graphviz.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class AutomatonDumper
    {
        protected Automaton automaton;
        /*
        * Constructor.
        */
        public AutomatonDumper(Automaton automaton)
        {
            this.automaton = automaton;
        }

        /*
        * Dumps the entire automaton.
         *
         *  The automaton encoded in DOT.
        */
        public string dump()
        {
            StringWriter writer = new StringWriter();
            this.writeHeader(writer);
            writer.writeLine();
            foreach (var state in this.automaton.getStates())
            {
                this.writeState(writer, state.Value);
            }

            writer.writeLine();
            foreach (var kvp in this.automaton.getTransitionTable())
            {
                var num = kvp.Key;
                var map = kvp.Value;
                foreach (var kvp2 in map )
                {
                    var trigger = kvp.Key;
                    var destination = kvp.Value;
                    writer.writeLine(string.Format(@"%d . %d [label=""%s""];", num, destination, trigger));
                }
            }

            writer.outdent();
            this.writeFooter(writer);
            return writer.get();
        }

        /*
        * Dumps only the specified state + any relevant
         * transitions.
         *
         *  The number of the state.
         *
         *  The output in DOT format.
        */
        public string dumpState(int n)
        {
            StringWriter writer = new StringWriter();
            this.writeHeader(writer, n);
            writer.writeLine();
            this.writeState(writer, this.automaton.getState(n));
            table = this.automaton.getTransitionTable();
            row = isset(table[n]) ? table[n] : array();
            foreach (var dest in row)
            {
                if (dest != n)
                {
                    this.writeState(writer, this.automaton.getState(dest), false);
                }
            }

            writer.writeLine();
            foreach (var trigger  => 
            dest  in 
            row )
            {
                writer.writeLine(sprintf("%d . %d [label=" % s "];", n, dest, trigger));
            }

            writer.outdent();
            this.writeFooter(writer);
            return writer.get();
        }

        protected void writeHeader(StringWriter writer, Nullable<int> stateNumber = null)
        {
            writer.writeLine(string.Format("digraph %s {" + (stateNumber != null ? "State" + stateNumber.Value.ToString() : "Automaton")));
            writer.indent();
            writer.writeLine(@"rankdir=""LR"";");
        }

        protected void writeState(StringWriter writer, State state, bool full = true)
        {
            int n = state.getNumber();
            string = sprintf("%d [label=" State % d ", n, n);
            if (full)
            {
                string. = "\n\n";
                items = array();
                foreach (state.getItems() as item)
                {
                    items[] = this.formatItem(item);
                }

                string. = implode("\n", items);
            }

            string. = "" ]
            ;
            "; writer.writeLine(string);
        }

        protected void formatItem(Item item)
        {
            rule = item.getRule();
            components = rule.getComponents();
            // the dot
            array_splice(components, item.getDotIndex(), 0, array("&bull;"));
            if (rule.getNumber() == 0)
            {
                string = "";
            }
            else
            {
                string = sprintf("%s &rarr; ", rule.getName());
            }

            string. = implode(" ", components);
            if (item.isReduceItem())
            {
                string. = sprintf(" [%s]", implode(" ", item.getLookahead()));
            }

            return string;
        }

        protected void writeFooter(StringWriter writer)
        {
            writer.writeLine("}");
        }
    }
}