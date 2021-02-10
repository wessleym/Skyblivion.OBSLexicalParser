using Dissect.Extensions;
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
        public AnalysisResult Analyze(Grammar grammar)
        {
            Automaton automaton = this.BuildAutomaton(grammar);
            var builtTable = this.BuildParseTable(automaton, grammar);
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
        protected Automaton BuildAutomaton(Grammar grammar)
        {
            // the eventual automaton
            Automaton automaton = new Automaton();
            // the queue of states that need processing
            Queue<State> queue = new Queue<State>();
            // the BST for state kernels
            Analysis.KernelSet.KernelSet kernelSet = new Analysis.KernelSet.KernelSet();
            // rules grouped by their name
            var groupedRules = grammar.GroupedRules;
            // FIRST sets of nonterminals
            Dictionary<string, List<string>> firstSets = this.CalculateFirstSets(groupedRules);
            // keeps a list of tokens that need to be pumped
            // through the automaton
            List<Tuple<Item, List<string>>> pumpings = new List<Tuple<Item, List<string>>>();
            // the item from which the whole automaton
            // is derived
            Item initialItem = new Item(grammar.StartRule, 0);
            //const ruct the initial state
            State state = new State(kernelSet.Insert(new decimal[][] { new decimal[] { initialItem.Rule.Number, initialItem.DotIndex} }), new Item[] { initialItem });
            // the initial item automatically has EOF
            // as its lookahead
            pumpings.Add(new Tuple<Item, List<string>>(initialItem, new List<string>() { Dissect.Parser.Parser.EOF_TOKEN_TYPE }));
            queue.Enqueue(state);
            automaton.AddState(state);
            while (queue.Any())
            {
                state = queue.Dequeue();
                // items of this state are grouped by
                // the active component to calculate
                // transitions easily
                Dictionary<string, List<Item>> groupedItems = new Dictionary<string, List<Item>>();
                // calculate closure
                List<string> added = new List<string>();
                List<Item> currentItems = state.Items.ToList();//It's important to make a copy here, or else items will get added to this list twice (once below and then once again in State.
                for (int x = 0; x < currentItems.Count; x++)
                {
                    Item item = currentItems[x];
                    if (!item.IsReduceItem)
                    {
                        string component = item.ActiveComponent;
                        groupedItems.AddNewListIfNotContainsKeyAndAddValueToList(component, item);
                        // if nonterminal
                        if (grammar.HasNonterminal(component))
                        {
                            // calculate lookahead
                            string[] cs = item.UnrecognizedComponents.ToArray();
                            List<string> lookahead = new List<string>();
                            for (int i=0;i<cs.Length;i++)

                            {
                                var c = cs[i];
                                if (!grammar.HasNonterminal(c))
                                {
                                    // if terminal, add it and break the loop
                                    lookahead = Util.Util.Union(lookahead, new string[] { c }).ToList();
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
                                        lookahead = Util.Util.Union(lookahead, newSet).ToList();
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
                                        lookahead = Util.Util.Union(lookahead, newSet).ToList();
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
                                    state.Add(newItem);
                                }
                                else
                                {
                                    // if it was expanded, each original
                                    // rule might bring new lookahead tokens,
                                    // so get the rule from the current state
                                    newItem = state.Get(rule.Number, 0);
                                }

                                if (connect)
                                {
                                    item.Connect(newItem);
                                }

                                if (pump)
                                {
                                    pumpings.Add(new Tuple<Item, List<string>>(newItem, lookahead));
                                }
                            }
                        }

                        // mark the component as processed
                        added.Add(component);
                    }
                } // calculate transitions
                foreach (var kvp in groupedItems)
                {
                    var thisComponent = kvp.Key;
                    var theseItems = kvp.Value;
                    List<decimal[]> newKernel = new List<decimal[]>();
                    foreach (var thisItem in theseItems)
                    {
                        newKernel.Add(new decimal[] { thisItem.Rule.Number, thisItem.DotIndex+ 1 });
                    }

                    int num = kernelSet.Insert(newKernel);
                    if (automaton.HasState(num))
                    {// the state already exists
                        automaton.AddTransition(state.Number, thisComponent, num); // extract the connected items from the target state
                        State nextState = automaton.GetState(num);
                        foreach (var thisItem in theseItems)
                        {
                            thisItem.Connect(nextState.Get(thisItem.Rule.Number, thisItem.DotIndex+ 1));
                        }
                    }
                    else
                    {// new state needs to be created
                        State newState = new State(num, theseItems.Select(item =>
                        {
                            Item newItem = new Item(item.Rule, item.DotIndex+ 1);
                            // connect the two items
                            item.Connect(newItem);
                            return newItem;
                        }).ToArray());
                        automaton.AddState(newState);
                        queue.Enqueue(newState);
                        automaton.AddTransition(state.Number, thisComponent, num);
                    }
                }
            } // pump all the lookahead tokens
            foreach (var pumping in pumpings)
            {
                pumping.Item1.PumpAll(pumping.Item2);
            }
            return automaton;
        }

        /*
        * Encodes the handle-finding FSA as a LR parse table.
         *
         * 
         *
         *  The parse table.
        */
        protected Tuple<ActionAndGoTo, List<Conflict>> BuildParseTable(Automaton automaton, Grammar grammar)
        {
            int conflictsMode = grammar.ConflictsMode;
            List<Conflict> conflicts = new List<Conflict>();
            Dictionary<int, Dictionary<string, bool>> errors = new Dictionary<int, Dictionary<string, bool>>();
            // initialize the table
            ActionAndGoTo table = new ActionAndGoTo();
            foreach (var kvp in automaton.TransitionTable)
            {
                var num = kvp.Key;
                var transitions = kvp.Value;
                foreach (var kvp2 in transitions)
                {
                    var trigger = kvp2.Key;
                    var destination = kvp2.Value;
                    if (!grammar.HasNonterminal(trigger))
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

            foreach (var kvp in automaton.States)
            {
                var num = kvp.Key;
                var state = kvp.Value;
                Dictionary<string, int> actionDictionary = table.Action.GetOrAdd(num, ()=>new Dictionary<string, int>());

                foreach (var item in state.Items)
                {
                    if (item.IsReduceItem)
                    {
                        int ruleNumber = item.Rule.Number;
                        foreach (var token in item.Lookahead)
                        {
                            Dictionary<string, bool>? errorsOfNum;
                            if (errors.TryGetValue(num, out errorsOfNum) && errorsOfNum.ContainsKey(token))
                            {
                                // there was a previous conflict resolved as an error
                                // entry for this token.
                                continue;
                            }

                            int instruction;
                            if (actionDictionary.TryGetValue(token, out instruction))
                            {
                                // conflict
                                if (instruction > 0)
                                {
                                    if ((conflictsMode & Grammar.OPERATORS) == Grammar.OPERATORS)
                                    {
                                        if (grammar.HasOperator(token))
                                        {
                                            Dictionary<string, int> operatorInfo = grammar.GetOperatorInfo(token);
                                            Nullable<int> rulePrecedence = item.Rule.Precedence;
                                            // unless the rule has given precedence
                                            if (rulePrecedence == null)
                                            {
                                                foreach (string c in item.Rule.Components.Reverse())
                                                {
                                                    // try to extract it from the rightmost terminal
                                                    if (grammar.HasOperator(c))
                                                    {
                                                        Dictionary<string, int> ruleOperatorInfo = grammar.GetOperatorInfo(c);
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
                                                    actionDictionary[token] = -ruleNumber;
                                                }

                                                else if (rulePrecedence < tokenPrecedence)
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

                                                    else if (assoc == Grammar.LEFT)
                                                    {
                                                        // if left-associative, reduce
                                                        actionDictionary[token] = -ruleNumber;
                                                    }

                                                    else if (assoc == Grammar.NONASSOC)
                                                    {
                                                        // the token is nonassociative.
                                                        // this actually means an input error, so
                                                        // remove the shift entry from the table
                                                        // and mark this as an explicit error
                                                        // entry
                                                        actionDictionary.Remove(token);
                                                        errors[num].Add(token, true);
                                                    }
                                                }

                                                continue; // resolved the conflict, phew
                                            }
                                            // we couldn"t calculate the precedence => the conflict was not resolved
                                            // move along.
                                        }
                                    }

                                    // s/r
                                    if ((conflictsMode & Grammar.SHIFT) == Grammar.SHIFT)
                                    {
                                        conflicts.Add(new Conflict(num, token, item.Rule, Grammar.SHIFT));
                                        continue;
                                    }
                                    else
                                    {
                                        throw new ShiftReduceConflictException(num, item.Rule, token, automaton);
                                    }
                                }
                                else
                                {
                                    // r/r
                                    Rule originalRule = grammar.GetRule(-instruction);
                                    Rule newRule = item.Rule;
                                    if ((conflictsMode & Grammar.LONGER_REDUCE) == Grammar.LONGER_REDUCE)
                                    {
                                        int count1 = originalRule.Components.Length;
                                        int count2 = newRule.Components.Length;
                                        if (count1 > count2)
                                        {
                                            // original rule is longer
                                            Rule[] resolvedRules = new Rule[] { originalRule, newRule };
                                            conflicts.Add(new Conflict(num, token, resolvedRules, Grammar.LONGER_REDUCE));
                                            continue;
                                        }

                                        else if (count2 > count1)
                                        {
                                            // new rule is longer
                                            actionDictionary[token] = -ruleNumber;
                                            Rule[] resolvedRules = new Rule[] { newRule, originalRule };
                                            conflicts.Add(new Conflict(num, token, resolvedRules, Grammar.LONGER_REDUCE));
                                            continue;
                                        }
                                    }

                                    if ((conflictsMode & Grammar.EARLIER_REDUCE) == Grammar.EARLIER_REDUCE)
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
                                            actionDictionary[token] = -ruleNumber;
                                            //WTM:  Change:  In PHP, this resolvedRules declaration was below the conflicts.Add statement, but this else statement was never reached anyway as far as I can tell.
                                            Rule[] resolvedRules = new Rule[] { newRule, originalRule };
                                            conflicts.Add(new Conflict(num, token, resolvedRules, Grammar.EARLIER_REDUCE));
                                            continue;
                                        }
                                    }

                                    // everything failed, throw an exception
                                    throw new ReduceReduceConflictException(num, originalRule, newRule, token, automaton);
                                }
                            }

                            actionDictionary[token] = -ruleNumber;
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
        protected Dictionary<string, List<string>> CalculateFirstSets(Dictionary<string, List<Rule>> rules)
        {
            // initialize
            Dictionary<string, List<string>> firstSets = new Dictionary<string, List<string>>();
            foreach (string key in rules.Keys)
            {
                firstSets.Add(key, new List<string>());
            }

            bool changes;
            do
            {
                changes = false;
                foreach (var kvp in rules)
                {
                    var lhs = kvp.Key;
                    var ruleArray = kvp.Value;
                    foreach (Rule rule in ruleArray)
                    {
                        string[] components = rule.Components;
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
                                        newArray = Util.Util.Union(newArray, x).ToArray();
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

                                        newArray = Util.Util.Union(newArray, x).ToArray();
                                    }
                                }
                                else
                                {
                                    // if terminal, simply add it the the FIRST set
                                    // and we"re done
                                    newArray = Util.Util.Union(newArray, new string[] { component }).ToArray();
                                    break;
                                }
                            }
                        }

                        if (Util.Util.Different(newArray, firstSets[lhs]))
                        {
                            firstSets[lhs] = Util.Util.Union(firstSets[lhs], newArray).ToList();
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