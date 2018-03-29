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
        protected List<Rule> rules = new List<Rule> { null };//Leave the first rule open for the eventual call to start(string name).
        protected Dictionary<string, List<Rule>> groupedRules = new Dictionary<string, List<Rule>>();
        protected int nextRuleNumber = 1;
        protected int conflictsMode = 9; // SHIFT | OPERATORS
        protected string currentNonterminal;
        protected Rule currentRule;
        protected Dictionary<string, Dictionary<string, int>> operators = new Dictionary<string, Dictionary<string, int>>();
        protected string[] currentOperators;
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
        public Grammar _is(params string[] args)
        {
            this.currentOperators = null;
            if (this.currentNonterminal == null)
            {
                throw new InvalidOperationException("You must specify a name of the rule first.");
            }

            int num = this.nextRuleNumber++;
            Rule rule = new Rule(num, this.currentNonterminal, args);
            this.rules.Add(rule);
            this.currentRule = rule;
            this.groupedRules.AddNewListIfNotContainsKeyAndAddValueToList(this.currentNonterminal, rule);
            return this;
        }

        /*
            * Sets the callback for the current rule.
            */
        public Grammar call(Func<object[], object> callback)
        {
            if (this.currentRule == null)
            {
                throw new InvalidOperationException("You must specify a rule first.");
            }
            this.currentRule.setCallback(callback);
            return this;
        }

        public Grammar call<TIn1, TOut>(Func<TIn1, TOut> callback)
        {
            return call((args) => callback((TIn1)args[0]));
        }

        public Grammar call<TIn1, TIn2, TOut>(Func<TIn1, TIn2, TOut> callback)
        {
            return call((args) => callback((TIn1)args[0], (TIn2)args[1]));
        }

        public Grammar call<TIn1, TIn2, TIn3, TOut>(Func<TIn1, TIn2, TIn3, TOut> callback)
        {
            return call((args) => callback((TIn1)args[0], (TIn2)args[1], (TIn3)args[2]));
        }

        public Grammar call<TIn1, TIn2, TIn3, TIn4, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TOut> callback)
        {
            return call((args) => callback((TIn1)args[0], (TIn2)args[1], (TIn3)args[2], (TIn4)args[3]));
        }

        public Grammar call<TIn1, TIn2, TIn3, TIn4, TIn5, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> callback)
        {
            return call((args) => callback((TIn1)args[0], (TIn2)args[1], (TIn3)args[2], (TIn4)args[3], (TIn5)args[4]));
        }

        /*
        * Returns the set of rules of this grammar.
        */
        public List<Rule> getRules()
        {
            return this.rules;
        }

        public Rule getRule(int number)
        {
            return this.rules[number];
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
        * Returns rules grouped by nonterminal name.
        */
        public Dictionary<string, List<Rule>> getGroupedRules()
        {
            return this.groupedRules;
        }

        /*
            * Sets a start rule for this grammar.
            */
        public void start(string name)
        {
            this.rules[0] = new Rule(0, START_RULE_NAME, new string[] { name });
        }

        /*
            * Returns the augmented start rule. For internal use only.
            */
        public Dissect.Parser.Rule getStartRule()
        {
            Rule firstRule = rules.FirstOrDefault();
            if (firstRule == null)//previously isset
            {
                throw new InvalidOperationException("No start rule specified.");
            }

            return firstRule;
        }

        /*
            * Sets the mode of conflict resolution.
            */
        public void resolve(int mode)
        {
            this.conflictsMode = mode;
        }

        /*
            * Returns the conflict resolution mode for this grammar.
            */
        public int getConflictsMode()
        {
            return this.conflictsMode;
        }

        /*
            * Does a nonterminal name exist in the grammar?
            */
        public bool hasNonterminal(string name)
        {
            return groupedRules.ContainsKey(name);
        }

        //DissectChange
        /*
        * Defines a group of operators.
        */
        public Grammar getOperators(params string[] args)
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
        public Dissect.Parser.Grammar left()
        {
            return this.assoc(LEFT);
        }

        /*
            * Marks the current group of operators as right-associative.
            */
        public Dissect.Parser.Grammar right()
        {
            return this.assoc(RIGHT);
        }

        /*
            * Marks the current group of operators as nonassociative.
            */
        public Dissect.Parser.Grammar nonassoc()
        {
            return this.assoc(NONASSOC);
        }

        /*
            * Explicitly sets the associatity of the current group of operators.
            */
        public Grammar assoc(int a)
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
        public Grammar prec(int i)
        {
            if (this.currentOperators == null)
            {
                if (this.currentRule == null)
                {
                    throw new InvalidOperationException("Define a group of operators or a rule first.");
                }
                else
                {
                    this.currentRule.setPrecedence(i);
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
        public bool hasOperator(string token)
        {
            return operators.ContainsKey(token);
        }

        public Dictionary<string, int> getOperatorInfo(string token)
        {
            return this.operators[token];
        }
    }
}