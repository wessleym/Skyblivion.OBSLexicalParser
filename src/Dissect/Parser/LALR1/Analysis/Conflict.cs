namespace Dissect.Parser.LALR1.Analysis
{
    class Conflict
    {
        private readonly int state;
        private readonly string lookahead;
        private readonly Rule? rule;
        private readonly Rule[]? rules;
        private readonly int resolution;
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
