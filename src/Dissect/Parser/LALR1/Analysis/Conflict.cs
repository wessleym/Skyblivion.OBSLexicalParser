using Dissect.Parser.LALR1.Analysis.KernelSet;

namespace Dissect.Parser.LALR1.Analysis
{
    class Conflict
    {
        private readonly Node state;
        private readonly string lookahead;
        private readonly Rule? rule;
        private readonly Rule[]? rules;
        private readonly int resolution;
        public Conflict(Node state, string lookahead, int resolution)
        {
            this.state = state;
            this.lookahead = lookahead;
            this.resolution = resolution;
        }
        public Conflict(Node state, string lookahead, Rule rule, int resolution)
            : this(state, lookahead, resolution)
        {
            this.rule = rule;
        }
        public Conflict(Node state, string lookahead, Rule[] rules, int resolution)
            : this(state, lookahead, resolution)
        {
            this.rules = rules;
        }
    }
}
