using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    /*
     * Class StageMap
     * Represents the stage map. Will map original target indexes to mapped target indexes and duplicate them as needed
     * Target duplication algorithm assumes that stages go only forward, i.e., that the quest progression is stage by
     * stage.
     * @package Ormin\OBSLexicalParser\Builds\QF\Factory\Map
     */
    class StageMap
    {
        private OrderableDictionary<int, List<int>> stageMap =  new OrderableDictionary<int, List<int>>(); 
        private Dictionary<int, List<int>> mappedTargetsIndex =  new Dictionary<int, List<int>>(); 
        public StageMap(IDictionary<int,List<int>> stageMapUnordered)
        {
            stageMap = new OrderableDictionary<int, List<int>>(stageMapUnordered);
            /*
             * Sort them by stage ids
             */
            stageMap.OrderBy(s => s.Key);
            /*
             * Do some validation
             */
            Nullable<int> length = null;
            foreach (var kvp in stageMap)
            {
                var stageId = kvp.Key;
                var rows = kvp.Value;
                if (length == null)
                {
                    length = rows.Count;
                }
                else if (length.Value != rows.Count)
                {
                    throw new ConversionException("Invalid stage map metadata - stageID " + stageId + " expected " + length.Value + " rows but had " + rows.Count);
                }
            }

            int nextFreeIndex = length.Value; //Next free index equals length, as per all 0-N arrays
            var resultStageMap = stageMap;
            Dictionary<int, bool> targetsStateMap = new Dictionary<int, bool>();
            Dictionary<int, List<int>> mappedTargetsIndex = new Dictionary<int, List<int>>();
            /*
             * Traverse through the map, and duplicate targets as needed
             */
            foreach (var kvp in stageMap)
            {
                var stageId = kvp.Key;
                var rows = kvp.Value;
                int targetIndex = 0;
                foreach (var row in rows)
                {
                    if (row != 0)
                    {
                        if (targetsStateMap.ContainsKey(targetIndex))
                        {
                            if (targetsStateMap[targetIndex] == false)
                            {
                                //We're changing the state from not used to used, in which case we need to do the duplication
                                if (!mappedTargetsIndex.ContainsKey(targetIndex))
                                {
                                    mappedTargetsIndex[targetIndex] = new List<int>();
                                }

                                mappedTargetsIndex[targetIndex].Add(nextFreeIndex);
                                int fillInValue = 0;
                                foreach (var kvp2 in resultStageMap)
                                {
                                    var resultStageId = kvp.Key;
                                    var resultRows = kvp.Value;
                                    //This stage id marks the start of the block, so we switch the fillin value
                                    if (resultStageId == stageId)
                                    {
                                        fillInValue = 1;
                                    }

                                    //Check if this stage id result is 0, if so - switch out the block
                                    if (stageMap[resultStageId][targetIndex] == 0)
                                    {
                                        fillInValue = 0;
                                    }

                                    resultStageMap[resultStageId].Add(fillInValue);
                                    /*
                                     * If we already started filling out, remove original target as it wont be used anymore
                                     */
                                    if (resultStageId >= stageId)
                                    {
                                        resultStageMap[resultStageId][targetIndex] = 0;
                                    }
                                }

                                ++nextFreeIndex; //Increase the index
                            }
                        }

                        //We mark that the target is now used, perhaps was already used in which case this operation
                        //is doing nothing
                        targetsStateMap[targetIndex] = true;
                    }
                    else
                    {
                        if (targetsStateMap.ContainsKey(targetIndex))
                        {
                            //We mark that the target state changed to false ( perhaps it was already false, in which case
                            //this operation is doing nothing
                            targetsStateMap[targetIndex] = false;
                        } //If the target was not used before, then we just continue, as it"s still not used.
                    }

                    ++targetIndex;
                }
            }

            this.stageMap = resultStageMap;
            this.mappedTargetsIndex = mappedTargetsIndex;
        }

        public IEnumerable<int> getStageIds()
        {
            return stageMap.Select(s => s.Key);
        }

        public List<int> getStageTargetsMap(int stageId)
        {
            return this.stageMap[stageId];
        }

        public Dictionary<int, List<int>> getMappedTargetsIndex()
        {
            return this.mappedTargetsIndex;
        }
    }
}