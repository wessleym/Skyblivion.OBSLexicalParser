using Dissect.Extensions.IDictionaryExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dissect.Parser
{
    /*
     * Represents a context-free grammar.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class Grammar
    {
        /*
        * The name given to the rule the grammar is augmented with
         * when start() is called.
        */
        const string START_RULE_NAME = "start";
        /*
        * The epsilon symbol signifies an empty production.
        */
        public const string EPSILON = "epsilon";
        /*
        * Returns the set of rules of this grammar.
        */
        public List<Rule?> Rules { get; protected set; } = new List<Rule?> { null };//Leave the first rule open for the eventual call to start(string name).
        /*
        * Returns rules grouped by nonterminal name.
        */
        public Dictionary<string, List<Rule>> GroupedRules { get; protected set; } = new Dictionary<string, List<Rule>>();
        protected int nextRuleNumber = 1;
        /*
        * Returns the conflict resolution mode for this grammar.
        */
        public int ConflictsMode { get; protected set; } = 9; // SHIFT | OPERATORS
        protected string? currentNonterminal;
        protected Rule? currentRule;
        protected Dictionary<string, Dictionary<string, int>> operators = new Dictionary<string, Dictionary<string, int>>();
        protected string[]? currentOperators;
        /*
        * Signifies that the parser should not resolve any
         * grammar conflicts.
        */
        const int NONE = 0;
        /*
        * Signifies that the parser should resolve
         * shift/reduce conflicts by always shifting.
        */
        public const int SHIFT = 1;
        /*
        * Signifies that the parser should resolve
         * reduce/reduce conflicts by reducing with
         * the longer rule.
        */
        public const int LONGER_REDUCE = 2;
        /*
        * Signifies that the parser should resolve
         * reduce/reduce conflicts by reducing
         * with the rule that was given earlier in
         * the grammar.
        */
        public const int EARLIER_REDUCE = 4;
        /*
        * Signifies that the conflicts should be
         * resolved by taking operator precendence
         * into account.
        */
        public const int OPERATORS = 8;
        /*
        * Signifies that the parser should automatically
         * resolve all grammar conflicts.
        */
        const int ALL = 15;
        /*
        * Left operator associativity.
        */
        public const int LEFT = 0;
        /*
        * Right operator associativity.
        */
        public const int RIGHT = 1;
        /*
        * The operator is nonassociative.
        */
        public const int NONASSOC = 2;
        public Grammar __invoke(string nonterminal)
        {
            this.currentNonterminal = nonterminal;
            return this;
        }

        /*
        * Defines an alternative for a grammar rule.
        */
        public Grammar Is(params string[] args)
        {
            this.currentOperators = null;
            if (this.currentNonterminal == null)
            {
                throw new InvalidOperationException("You must specify a name of the rule first.");
            }

            int num = this.nextRuleNumber++;
            Rule rule = new Rule(num, this.currentNonterminal, args);
            this.Rules.Add(rule);
            this.currentRule = rule;
            this.GroupedRules.AddNewListIfNotContainsKeyAndAddValueToList(this.currentNonterminal, rule);
            return this;
        }

        /*
        * Sets the callback for the current rule.
        */
        public Grammar Call(Func<object[], object> callback)
        {
            if (this.currentRule == null)
            {
                throw new InvalidOperationException("You must specify a rule first.");
            }
            this.currentRule.Callback = callback;
            return this;
        }

        public Grammar Call<TIn1, TOut>(Func<TIn1, TOut> callback) where TOut : notnull
        {
            return Call((args) => callback((TIn1)args[0]));
        }

        public Grammar Call<TIn1, TIn2, TOut>(Func<TIn1, TIn2, TOut> callback) where TOut : notnull
        {
            return Call((args) => callback((TIn1)args[0], (TIn2)args[1]));
        }

        public Grammar Call<TIn1, TIn2, TIn3, TOut>(Func<TIn1, TIn2, TIn3, TOut> callback) where TOut : notnull
        {
            return Call((args) => callback((TIn1)args[0], (TIn2)args[1], (TIn3)args[2]));
        }

        public Grammar Call<TIn1, TIn2, TIn3, TIn4, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TOut> callback) where TOut : notnull
        {
            return Call((args) => callback((TIn1)args[0], (TIn2)args[1], (TIn3)args[2], (TIn4)args[3]));
        }

        public Grammar Call<TIn1, TIn2, TIn3, TIn4, TIn5, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> callback) where TOut : notnull
        {
            return Call((args) => callback((TIn1)args[0], (TIn2)args[1], (TIn3)args[2], (TIn4)args[3], (TIn5)args[4]));
        }

        public Rule? GetRuleNullable(int number)
        {
            return this.Rules[number];
        }
        public Rule GetRule(int number)
        {
            Rule? rule = GetRuleNullable(number);
            if (rule == null) { throw new InvalidOperationException("Rule at " + nameof(number) + " " + number + " was null."); }
            return rule;
        }

        //DissectChange:
        /*
        * Returns the nonterminal symbols of this grammar.
        */
        /*public string[] getNonterminals()
        {
            return this.nonterminals;
        }*/

        /*
            * Sets a start rule for this grammar.
            */
        public void Start(string name)
        {
            this.Rules[0] = new Rule(0, START_RULE_NAME, new string[] { name });
        }

        /*
            * Returns the augmented start rule. For internal use only.
            */
        public Rule StartRule
        {
            get
            {
                Rule? firstRule = Rules.FirstOrDefault();
                if (firstRule == null)
                {
                    throw new InvalidOperationException("No start rule specified.");
                }
                return firstRule;
            }
        }

        /*
        * Sets the mode of conflict resolution.
        */
        public void Resolve(int mode)
        {
            this.ConflictsMode = mode;
        }

        /*
        * Does a nonterminal name exist in the grammar?
        */
        public bool HasNonterminal(string name)
        {
            return GroupedRules.ContainsKey(name);
        }

        //DissectChange
        /*
        * Defines a group of operators.
        */
        public Grammar GetOperators(params string[] args)
        {
            this.currentRule = null;
            this.currentOperators = args;
            foreach (var op in args)
            {
                this.operators.Add(op, new Dictionary<string, int>() { { "prec", 1 }, { "assoc", LEFT } });
            }

            return this;
        }

        /*
        * Marks the current group of operators as left-associative.
        */
        public Grammar Left()
        {
            return this.Assoc(LEFT);
        }

        /*
        * Marks the current group of operators as right-associative.
        */
        public Grammar Right()
        {
            return this.Assoc(RIGHT);
        }

        /*
        * Marks the current group of operators as nonassociative.
        */
        public Grammar Nonassoc()
        {
            return this.Assoc(NONASSOC);
        }

        /*
        * Explicitly sets the associatity of the current group of operators.
        */
        public Grammar Assoc(int a)
        {
            if (this.currentOperators == null)
            {
                throw new InvalidOperationException("Define a group of operators first.");
            }

            foreach (var op in this.currentOperators)
            {
                this.operators[op].Add("assoc", a);
            }

            return this;
        }

        /*
            * Sets the precedence (as an integer) of the current group of operators.
             * If no group of operators is being specified, sets the precedence
             * of the currently described rule.
            */
        public Grammar Prec(int i)
        {
            if (this.currentOperators == null)
            {
                if (this.currentRule == null)
                {
                    throw new InvalidOperationException("Define a group of operators or a rule first.");
                }
                else
                {
                    this.currentRule.Precedence = i;
                }
            }
            else
            {
                foreach (var op in this.currentOperators)
                {
                    this.operators[op].Add("prec", i);
                }
            }

            return this;
        }

        /*
        * Is the passed token an operator?
        */
        public bool HasOperator(string token)
        {
            return operators.ContainsKey(token);
        }

        public Dictionary<string, int> GetOperatorInfo(string token)
        {
            return this.operators[token];
        }
    }
}