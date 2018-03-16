using Dissect.Parser;

namespace Dissect.Parser.LALR1.Analysis
{
    class Conflict
    {
        private int state;
        private string lookahead;
        private Rule rule;
        private Rule[] rules;
        private int resolution;
        public Conflict(int state, string lookahead, int resolution)
        {
            this.state = state;
            this.lookahead = lookahead;
            this.resolution = resolution;
        }
        public Conflict(int state, string lookahead, Rule rule, int resolution)
            : this(state, lookahead, resolution)
        {
            this.rule = rule;
        }
        public Conflict(int state, string lookahead, Rule[] rules, int resolution)
            : this(state, lookahead, resolution)
        {
            this.rules = rules;
        }
    }
}
