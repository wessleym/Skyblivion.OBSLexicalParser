using Dissect.Parser.LALR1.Analysis.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dissect.Parser.LALR1.Analysis
{
    /*
     * Performs a grammar analysis and returns
     * the result.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class Analyzer
    {
        /*
        * Performs a grammar analysis.
         *
         *  The grammar to analyse.
         *
         *  The result ofthe analysis.
        */
        public AnalysisResult analyze(Grammar grammar)
        {
            Automaton automaton = this.buildAutomaton(grammar);
            var builtTable = this.buildParseTable(automaton, grammar);
            var parseTable = builtTable.Item1;
            var conflicts = builtTable.Item2;
            return new AnalysisResult(parseTable, automaton, conflicts);
        }

        /*
        * Builds the handle-finding FSA from the grammar.
         *
         *  The grammar.
         *
         *  The resulting automaton.
        */
        protected Automaton buildAutomaton(Grammar grammar)
        {
            // the eventual automaton
            Automaton automaton = new Automaton();
            // the queue of states that need processing
            Queue<State> queue = new Queue<State>();
            // the BST for state kernels
            Analysis.KernelSet.KernelSet kernelSet = new Analysis.KernelSet.KernelSet();
            // rules grouped by their name
            var groupedRules = grammar.getGroupedRules();
            // FIRST sets of nonterminals
            Dictionary<string, List<string>> firstSets = this.calculateFirstSets(groupedRules);
            // keeps a list of tokens that need to be pumped
            // through the automaton
            Dictionary<Item, List<string>> pumpings = new Dictionary<Item, List<string>>();
            // the item from which the whole automaton
            // is derived
            Item initialItem = new Item(grammar.getStartRule(), 0);
            //const ruct the initial state
            State state = new State(kernelSet.insert(new decimal[][] { new decimal[] { initialItem.getRule().getNumber(), initialItem.getDotIndex() } }), new Item[] { initialItem });
            // the initial item automatically has EOF
            // as its lookahead
            pumpings.Add(initialItem, new List<string>() { Dissect.Parser.Parser.EOF_TOKEN_TYPE });
            queue.Enqueue(state);
            automaton.addState(state);
            while (queue.Any())
            {
                State queueState = queue.Dequeue();
                // items of this state are grouped by
                // the active component to calculate
                // transitions easily
                Dictionary<string, List<Item>> groupedItems = new Dictionary<string, List<Item>>();
                // calculate closure
                List<string> added = new List<string>();
                List<Item> currentItems = queueState.getItems();
                for (int x = 0; x < currentItems.Count; x++)
                {
                    Item item = currentItems[x];
                    if (!item.isReduceItem())
                    {
                        string component = item.getActiveComponent();
                        if (!groupedItems.ContainsKey(component))
                        {
                            groupedItems.Add(component, new List<Item>());
                        }
                        groupedItems[component].Add(item);
                        // if nonterminal
                        if (grammar.hasNonterminal(component))
                        {
                            // calculate lookahead
                            string[] cs = item.getUnrecognizedComponents().ToArray();
                            List<string> lookahead = new List<string>();
                            for (int i=0;i<cs.Length;i++)

                            {
                                var c = cs[i];
                                if (!grammar.hasNonterminal(c))
                                {
                                    // if terminal, add it and break the loop
                                    lookahead = Util.Util.union(lookahead, new string[] { c }).ToList();
                                    break;
                                }
                                else
                                {
                                    // if nonterminal
                                    List<string> newSet = firstSets[c];
                                    if (!newSet.Contains(Grammar.EPSILON))
                                    {
                                        // if the component doesn"t derive
                                        // epsilon, merge FIRST sets and break
                                        lookahead = Util.Util.union(lookahead, newSet).ToList();
                                        break;
                                    }
                                    else
                                    {
                                        // if it does
                                        if (i < (cs.Length - 1))
                                        {
                                            // if more components ahead, remove epsilon
                                            newSet.RemoveAt(newSet.IndexOf(Grammar.EPSILON));
                                        }

                                        // and continue the loop
                                        lookahead = Util.Util.union(lookahead, newSet).ToList();
                                    }
                                }
                            }

                            // two items are connected if the unrecognized
                            // part of rule 1 derives epsilon
                            bool connect = false;
                            // only store the pumped tokens if there
                            // actually is an unrecognized part
                            bool pump = true;
                            if (!lookahead.Any())
                            {
                                connect = true;
                                pump = false;
                            }
                            else
                            {
                                if (lookahead.Contains(Grammar.EPSILON))
                                {
                                    lookahead.RemoveAt(lookahead.IndexOf(Grammar.EPSILON));
                                    connect = true;
                                }
                            }

                            foreach (var rule in groupedRules[component])
                            {
                                Item newItem;
                                if (!added.Contains(component))
                                {
                                    // if component hasn"t yet been expaned,
                                    // create new items for it
                                    newItem = new Item(rule, 0);
                                    currentItems.Add(newItem);
                                    queueState.add(newItem);
                                }
                                else
                                {
                                    // if it was expanded, each original
                                    // rule might bring new lookahead tokens,
                                    // so get the rule from the current state
                                    newItem = queueState.get(rule.getNumber(), 0);
                                }

                                if (connect)
                                {
                                    item.connect(newItem);
                                }

                                if (pump)
                                {
                                    pumpings.Add(newItem, lookahead);
                                }
                            }
                        }

                        // mark the component as processed
                        added.Add(component);
                    }
                } // calculate transitions
                foreach

                (var kvp in groupedItems)
                {
                    var thisComponent = kvp.Key;
                    var theseItems = kvp.Value;
                    List<decimal[]> newKernel = new List<decimal[]>();
                    foreach
    (var thisItem in theseItems)
                    {
                        newKernel.Add(new decimal[] { thisItem.getRule().getNumber(), thisItem.getDotIndex() + 1 });
                    }

                    int num = kernelSet.insert(newKernel);
                    if (automaton.hasState(num))
                    {// the state already exists
                        automaton.addTransition(state.getNumber(), thisComponent, num); // extract the connected items from the target state
                        State nextState = automaton.getState(num);
                        foreach (var thisItem in theseItems) { thisItem.connect(nextState.get(thisItem.getRule().getNumber(),
thisItem.getDotIndex() + 1));
                        }
                    }
                    else
                    {// new state needs to be created
                        State newState = new State(num, theseItems.Select((Item i) =>
{
    Item newItem = new Item(i.getRule(), i.getDotIndex() + 1);
    // connect the two items
    i.connect(newItem);
    return newItem;
}).ToArray());
                        automaton.addState(newState);
                        queue.Enqueue(newState);
                        automaton.addTransition(state.getNumber(), thisComponent, num);
                    }
                }
            } // pump all the lookahead tokens
            foreach (var pumping in pumpings) { pumping.Key.pumpAll(pumping.Value); }
            return automaton;
        }

        /*
        * Encodes the handle-finding FSA as a LR parse table.
         *
         * 
         *
         *  The parse table.
        */
        protected Tuple<ActionAndGoTo, List<Conflict>> buildParseTable(Automaton automaton, Grammar grammar)
        {
            int conflictsMode = grammar.getConflictsMode();
            List<Conflict> conflicts = new List<Conflict>();
            Dictionary<int, Dictionary<string, bool>> errors = new Dictionary<int, Dictionary<string, bool>>();
            // initialize the table
            ActionAndGoTo table = new ActionAndGoTo();
            foreach (var kvp in automaton.getTransitionTable())
    {
                var num = kvp.Key;
                var transitions = kvp.Value;
                foreach (var kvp2  in         transitions )
        {
                    var trigger = kvp2.Key;
                    var destination = kvp2.Value;
                    if (!grammar.hasNonterminal(trigger))
                    {
                        // terminal implies shift
                        table.AddAction(num, trigger, destination);
                    }
                    else
                    {
                        // nonterminal goes in the goto table
                        table.AddGoTo(num, trigger, destination);
                    }
                }
            }

            foreach (var kvp in automaton.getStates())
    {
                var num = kvp.Key;
                var state = kvp.Value;
                if (!table.Action.ContainsKey(num))
                {
                    table.Action.Add(num, new Dictionary<string, int>());
                }

                foreach (var item in state.getItems())
                {
                    if (item.isReduceItem())
                    {
                        int ruleNumber = item.getRule().getNumber();
                        foreach (var token in item.getLookahead())
                        {
                            if (errors.ContainsKey(num) && errors[num].ContainsKey(token))
                            {
                                // there was a previous conflict resolved as an error
                                // entry for this token.
                                continue;
                            }

                            if (table.Action[num].ContainsKey(token))
                            {
                                // conflict
                                int instruction = table.Action[num][token];
                                if (instruction > 0)
                                {
                                    if ((conflictsMode & Grammar.OPERATORS)== Grammar.OPERATORS)
                                    {
                                        if (grammar.hasOperator(token))
                                        {
                                            Dictionary<string, int> operatorInfo = grammar.getOperatorInfo(token);
                                            Nullable<int> rulePrecedence = item.getRule().getPrecedence();
                                            // unless the rule has given precedence
                                            if (rulePrecedence == null)
                                            {
                                                foreach (string c in item.getRule().getComponents().Reverse())
                                                {
                                                    // try to extract it from the rightmost terminal
                                                    if (grammar.hasOperator(c))
                                                    {
                                                        Dictionary<string, int> ruleOperatorInfo = grammar.getOperatorInfo(c);
                                                        rulePrecedence = ruleOperatorInfo["prec"];
                                                        break;
                                                    }
                                                }
                                            }

                                            if (rulePrecedence != null)
                                            {
                                                // if we actually have a rule precedence
                                                int tokenPrecedence = operatorInfo["prec"];
                                                if (rulePrecedence > tokenPrecedence)
                                                {
                                                    // if the rule precedence is higher, reduce
                                                    table.Action[num][token] = -ruleNumber;
                                                }

                                                else if(rulePrecedence < tokenPrecedence)
                                                {
                                                    // if the token precedence is higher, shift
                                                    // (i.e. don"t modify the table)
                                                }
                                                else 
                                        {
                                                    // precedences are equal, let"s turn to associativity
                                                    int assoc = operatorInfo["assoc"];
                                                    if (assoc == Grammar.RIGHT)
                                                    {
                                                        // if right-associative, shift
                                                        // (i.e. don"t modify the table)
                                                    }

                                                    else if(assoc == Grammar.LEFT)
                                            {
                                                        // if left-associative, reduce
                                                        table.Action[num][token] = -ruleNumber;
                                                    }

                                                    else if(assoc == Grammar.NONASSOC)
                                            {
                                                        // the token is nonassociative.
                                                        // this actually means an input error, so
                                                        // remove the shift entry from the table
                                                        // and mark this as an explicit error
                                                        // entry
                                                        table.Action[num].Remove(token);
                                                        errors[num][token] = true;
                                                    }
                                                }

                                                continue; // resolved the conflict, phew
                                            }
                                            // we couldn"t calculate the precedence => the conflict was not resolved
                                            // move along.
                                        }
                                    }

                                    // s/r
                                    if ((conflictsMode & Grammar.SHIFT)== Grammar.SHIFT)
                                    {
                                        conflicts.Add(new Conflict(num, token, item.getRule(), Grammar.SHIFT));
                                        continue;
                                    }
                                    else
                                    {
                                        throw new ShiftReduceConflictException(num, item.getRule(), token, automaton);
                                    }
                                }
                                else
                                {
                                    // r/r
                                    Rule originalRule = grammar.getRule(-instruction);
                                    Rule newRule = item.getRule();
                                    if ((conflictsMode & Grammar.LONGER_REDUCE)== Grammar.LONGER_REDUCE)
                                    {
                                        int count1 = originalRule.getComponents().Length;
                                        int count2 = newRule.getComponents().Length;
                                        if (count1 > count2)
                                        {
                                            // original rule is longer
                                            Rule[] resolvedRules = new Rule[] { originalRule, newRule };
                                            conflicts.Add(new Conflict(num, token, resolvedRules, Grammar.LONGER_REDUCE));
                                            continue;
                                        }

                                        else if(count2 > count1)
                                        {
                                            // new rule is longer
                                            table.Action[num][token] = -ruleNumber;
                                            Rule[] resolvedRules = new Rule[] { newRule, originalRule };
                                            conflicts.Add(new Conflict(num, token, resolvedRules, Grammar.LONGER_REDUCE));
                                            continue;
                                        }
                                    }

                                    if ((conflictsMode & Grammar.EARLIER_REDUCE)== Grammar.EARLIER_REDUCE)
                                    {
                                        if (-instruction < ruleNumber)
                                        {
                                            // original rule was earlier
                                            Rule[] resolvedRules = new Rule[] { originalRule, newRule };
                                            conflicts.Add(new Conflict(num, token, resolvedRules, Grammar.EARLIER_REDUCE));
                                            continue;
                                        }
                                        else
                                        {
                                            // new rule was earlier
                                            table.Action[num][token] = -ruleNumber;
                                            Rule[] resolvedRules = new Rule[] { newRule, originalRule };//WTM:  Change:  This was below the conflicts.Add statement
                                            conflicts.Add(new Conflict(num, token, resolvedRules, Grammar.EARLIER_REDUCE));
                                            continue;
                                        }
                                    }

                                    // everything failed, throw an exception
                                    throw new ReduceReduceConflictException(num, originalRule, newRule, token, automaton);
                                }
                            }

                            table.Action[num][token] = -ruleNumber;
                        }
                    }
                }
            }

            return new Tuple<ActionAndGoTo, List<Conflict>>(table, conflicts);
        }

        /*
                * Calculates the FIRST sets of all nonterminals.
                 *
                 *  The rules grouped by the LHS.
                 *
                 *  Calculated FIRST sets.
                */
        protected Dictionary<string, List<string>> calculateFirstSets(Dictionary<string, List<Rule>> rules)
        {
            // initialize
            Dictionary<string, List<string>> firstSets = new Dictionary<string, List<string>>();
            foreach (string key in rules.Keys)
            {
                firstSets.Add(key, new List<string>());
            }

            bool changes = false;
            do
            {
                foreach (var kvp in rules)
                {
                    var lhs = kvp.Key;
                    var ruleArray = kvp.Value;
                    foreach (Rule rule in ruleArray)
                    {
                        string[] components = rule.getComponents();
                        string[] newArray = new string[] { };
                        if (!components.Any())
                        {
                            newArray = new string[] { Grammar.EPSILON };
                        }
                        else
                        {
                            for (int i = 0; i < components.Length; i++)
                            {
                                string component = components[i];
                                if (rules.ContainsKey(component))
                                {
                                    // if nonterminal, copy its FIRST set to
                                    // this rule"s first set
                                    List<string> x = firstSets[component];
                                    if (!x.Contains(Grammar.EPSILON))
                                    {
                                        // if the component doesn"t derive
                                        // epsilon, merge the first sets and
                                        // we"re done
                                        newArray = Util.Util.union(newArray, x).ToArray();
                                        break;
                                    }
                                    else
                                    {
                                        // if all components derive epsilon,
                                        // the rule itself derives epsilon
                                        if (i < (components.Length - 1))
                                        {
                                            // more components ahead, remove epsilon
                                            x.RemoveAt(x.IndexOf(Grammar.EPSILON));
                                        }

                                        newArray = Util.Util.union(newArray, x).ToArray();
                                    }
                                }
                                else
                                {
                                    // if terminal, simply add it the the FIRST set
                                    // and we"re done
                                    newArray = Util.Util.union(newArray, new string[] { component }).ToArray();
                                    break;
                                }
                            }
                        }

                        if (Util.Util.different(newArray, firstSets[lhs]))
                        {
                            firstSets[lhs] = Util.Util.union(firstSets[lhs], newArray).ToList();
                            changes = true;
                        }
                    }
                }
            }
            while (changes);
            return firstSets;
        }
    }
}