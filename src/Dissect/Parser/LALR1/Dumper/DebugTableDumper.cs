using Dissect.Parser;

namespace Dissect.Parser.LALR1.Dumper
{
    /*
     * Dumps a parse table using the debug format,
     * with comments explaining the actions of the
     * parser.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class DebugTableDumper : ITableDumper
    {
        protected \ Dissect \Parser \Grammar grammar;
        protected \ Dissect \Parser \LALR1 \Dumper \StringWriter writer;
        protected boolean written = false;
        /*
        * Constructor.
         *
         *  The grammar of this parse table.
        */
        public DebugTableDumper(Grammar grammar)
        {
            this.grammar = grammar;
            this.writer = new StringWriter();
        }

        public void dump(array table)
        {
            // for readability
            ksort(table["action"]);
            ksort(table["goto"]);
            // the grammar dictates the parse table,
            // therefore the result is always the same
            if (!this.written)
            {
                this.writeHeader();
                this.writer.indent();
                foreach (var n  => 
                state  in 
                table["action"] )
                {
                    this.writeState(n, state);
                    this.writer.writeLine();
                }

                this.writer.outdent();
                this.writeMiddle();
                this.writer.indent();
                foreach (var n  => 
                map  in 
                table["goto"] )
                {
                    this.writeGoto(n, map);
                    this.writer.writeLine();
                }

                this.writer.outdent();
                this.writeFooter();
                this.written = true;
            }

            return this.writer.get();
        }

        protected void writeHeader()
        {
            this.writer.writeLine("<?php");
            this.writer.writeLine();
            this.writer.writeLine("return array(");
            this.writer.indent();
            this.writer.writeLine("" action " => array(");
        }

        protected void writeState(n, array state)
        {
            this.writer.writeLine((string)n." => array(");
            this.writer.indent();
            foreach (var trigger  => 
            action  in 
            state )
            {
                this.writeAction(trigger, action);
                this.writer.writeLine();
            }

            this.writer.outdent();
            this.writer.writeLine("),");
        }

        protected void writeAction(trigger, action)
        {
            if (action > 0)
            {
                this.writer.writeLine(sprintf("// on %s shift and go to state %d", trigger, action));
            }

            elseif(action < 0)
            {
                rule = this.grammar.getRule(-action);
                components = rule.getComponents();
                if (empty(components))
                {
                    rhs = "/* empty */";
                }
                else
                {
                    rhs = implode(" ", components);
                }

                this.writer.writeLine(sprintf("// on %s reduce by rule %s . %s", trigger, rule.getName(), rhs));
            } else 
            {
                this.writer.writeLine(sprintf("// on %s accept the input", trigger));
            }

            this.writer.writeLine(sprintf("" % s " => %d,", trigger, action));
        }

        protected void writeMiddle()
        {
            this.writer.writeLine("),");
            this.writer.writeLine();
            this.writer.writeLine(""goto " => array(" )
            ;
        }

        protected void writeGoto(n, array map)
        {
            this.writer.writeLine((string)n." => array(");
            this.writer.indent();
            foreach (var sym  => 
            dest  in 
            map )
            {
                this.writer.writeLine(sprintf("// on %s go to state %d", sym, dest));
                this.writer.writeLine(sprintf("" % s " => %d,", sym, dest));
                this.writer.writeLine();
            }

            this.writer.outdent();
            this.writer.writeLine("),");
        }

        protected void writeFooter()
        {
            this.writer.writeLine("),");
            this.writer.outdent();
            this.writer.writeLine(");");
        }
    }
}