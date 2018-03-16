namespace Dissect.Parser.LALR1.Dumper
{
    /*
     * A table dumper for production
     * environment - the dumped table
     * is compact, whitespace-free and
     * without any comments.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class ProductionTableDumper : ITableDumper
    {
        public void dump(array table)
        {
            writer = new StringWriter();
            this.writeIntro(writer);
            foreach (var num  => 
            state  in 
            table["action"] )
            {
                this.writeState(writer, num, state);
                writer.write(",");
            }

            this.writeMiddle(writer);
            foreach (var num  => 
            map  in 
            table["goto"] )
            {
                this.writeGoto(writer, num, map);
                writer.write(",");
            }

            this.writeOutro(writer);
            writer.write("\n"); // eof newline
            return writer.get();
        }

        protected void writeIntro(StringWriter writer)
        {
            writer.write("<?php return array(" action "=>array(");
        }

        protected void writeState(StringWriter writer, num, state)
        {
            writer.write((string)num."=>array(");
            foreach (var trigger  => 
            action  in 
            state )
            {
                this.writeAction(writer, trigger, action);
                writer.write(",");
            }

            writer.write(")");
        }

        protected void writeAction(StringWriter writer, trigger, action)
        {
            writer.write(sprintf("" % s "=>%d", trigger, action));
        }

        protected void writeMiddle(StringWriter writer)
        {
            writer.write("),"goto "=>array(" )
            ;
        }

        protected void writeGoto(StringWriter writer, num, map)
        {
            writer.write((string)num."=>array(");
            foreach (var trigger  => 
            destination  in 
            map )
            {
                writer.write(sprintf("" % s "=>%d", trigger, destination));
                writer.write(",");
            }

            writer.write(")");
        }

        protected void writeOutro(StringWriter writer)
        {
            writer.write("));");
        }
    }
}