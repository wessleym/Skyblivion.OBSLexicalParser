namespace Dissect.Parser.LALR1.Dumper
{
    /*
     * A common contract for parse table dumpers.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    interface ITableDumper
    {
        /*
        * Dumps the parse table.
         *
         *  The parse table.
         *
         *  The resulting string representation of the table.
        */
        string dump(array table);
    }
}