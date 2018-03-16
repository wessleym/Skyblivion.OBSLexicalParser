using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class LPCommand
    {
        protected string Name, Description;
        protected LPCommandInput Input = new LPCommandInput();
        public void set_time_limit(int minutes) { }
    }
}
