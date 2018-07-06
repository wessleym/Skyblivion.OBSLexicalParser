using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class TES5BuildPlanBuilder
    {
        private readonly TES5ScriptDependencyGraph graph;
        public TES5BuildPlanBuilder(TES5ScriptDependencyGraph graph)
        {
            this.graph = graph;
        }

        public Dictionary<int, List<Dictionary<string, List<string>>>> CreateBuildPlan(BuildSourceFilesCollection scripts, int threads = BuildTargetCommand.DefaultThreads)
        {
            Dictionary<string, string> codeScripts = new Dictionary<string, string>();
            /*
             * Mapping script names to build names
             */
            Dictionary<string, string> scriptToBuild = new Dictionary<string, string>();
            foreach (var kvp in scripts)
            {
                var buildName = kvp.Key;
                var buildScripts = kvp.Value;
                foreach (var script in buildScripts)
                {
                    string scriptName = script.Substring(0, script.Length-4);
                    string scriptNameKey = scriptName.ToLower();
                    codeScripts.Add(scriptNameKey, scriptName);
                    scriptToBuild.Add(scriptNameKey, buildName);
                }
            }

            List<Dictionary<string, List<string>>> preparedChunks = new List<Dictionary<string, List<string>>>();
            List<string> nonpairedScripts = new List<string>();
            int previousCount = codeScripts.Count;
            /*
             * Prepare chunks of scripts and push lone scripts into a different array
             */
            while (codeScripts.Any())
            {
                var currentScript = codeScripts.First().Key;
                string[] preparedChunk = this.graph.getScriptsToCompile(currentScript);
                if (preparedChunk.Length > 1)
                {
                    /*
                     * Chunk mapped per-build
                     */
                    Dictionary<string, List<string>> preparedMappedChunk = new Dictionary<string, List<string>>();
                    foreach (var chunkScript in preparedChunk)
                    {
                        string chunkScriptKey = chunkScript.ToLower();
                        codeScripts.Remove(chunkScriptKey);
                        preparedMappedChunk.AddNewListIfNotContainsKeyAndAddValueToList(scriptToBuild[chunkScriptKey], chunkScript);
                    }

                    preparedChunks.Add(preparedMappedChunk);
                }
                else
                {
                    string nonpairedChunkScript = preparedChunk[0];
                    nonpairedScripts.Add(nonpairedChunkScript);
                    string nonpairedChunkScriptKey = nonpairedChunkScript.ToLower();
                    codeScripts.Remove(nonpairedChunkScriptKey);
                }

                if (codeScripts.Count >= previousCount)
                {
                    throw new InvalidOperationException("Error in planning build, circuit breaker on.");
                }
                else
                {
                    previousCount = codeScripts.Count;
                }
            }

            Dictionary<int, List<Dictionary<string, List<string>>>> threadBuckets = new Dictionary<int, List<Dictionary<string, List<string>>>>();
            Dictionary<int, int> threadBucketsSizes = new Dictionary<int, int>();
            int bucket = 0;
            foreach (var chunk in preparedChunks)
            {
                threadBucketsSizes.AddIfNotContainsKey(bucket, 0);
                threadBuckets.GetOrAddNewIfNotContainsKey(bucket).Add(chunk);
                foreach (var kvp in chunk)
                {
                    var chunkBuild = kvp.Key;
                    var chunkScripts = kvp.Value;
                    threadBucketsSizes[bucket] += chunkScripts.Count;
                }

                bucket++;
                if (bucket == threads)
                {
                    bucket = 0;
                }
            }

            //Evening the buckets
            int biggestBucket = threadBucketsSizes.Max(kvp=>kvp.Value);
            foreach (var kvp in threadBuckets)
            {
                var bucketKey = kvp.Key;
                int bucketSize = threadBucketsSizes[bucketKey];
                int neededScripts = biggestBucket - bucketSize;
                Dictionary<string, List<string>> eveningChunk = new Dictionary<string, List<string>>();
                if (neededScripts >= nonpairedScripts.Count)
                {
                    foreach (var nonpairedScript in nonpairedScripts)
                    {
                        string chunkScriptBuild = scriptToBuild[nonpairedScript.ToLower()];
                        eveningChunk.GetOrAddNewIfNotContainsKey(chunkScriptBuild).Add(nonpairedScript);
                    }

                    threadBuckets[bucketKey].Add(eveningChunk);
                    //Not sure if should be here but prolly yes?
                    threadBucketsSizes[bucketKey] += nonpairedScripts.Count;
                    nonpairedScripts = new List<string>();
                    break;
                }

                string[] sliceOfNonpairedScripts = nonpairedScripts.Take(neededScripts).ToArray();
                foreach (var sliceOfNonpairedScript in sliceOfNonpairedScripts)
                {
                    string chunkScriptBuild = scriptToBuild[sliceOfNonpairedScript.ToLower()];
                    eveningChunk.GetOrAddNewIfNotContainsKey(chunkScriptBuild).Add(sliceOfNonpairedScript);
                }

                threadBuckets[bucketKey].Add(eveningChunk);
                threadBucketsSizes[bucketKey] += neededScripts;
                nonpairedScripts = nonpairedScripts.Skip(neededScripts).ToList();
            }

            Dictionary<int, List<Dictionary<string, List<string>>>> restChunks = new Dictionary<int, List<Dictionary<string, List<string>>>>();
            int restChunkBucket = 0;
            foreach (var nonpairedScript in nonpairedScripts)
            {
                Dictionary<string, List<string>> singleScriptChunk = new Dictionary<string, List<string>>();
                string chunkScriptBuild = scriptToBuild[nonpairedScript.ToLower()];
                singleScriptChunk.AddNewListIfNotContainsKey(chunkScriptBuild);
                restChunks.AddNewListIfNotContainsKey(restChunkBucket);

                singleScriptChunk[chunkScriptBuild].Add(nonpairedScript);
                restChunks[restChunkBucket].Add(singleScriptChunk);
                restChunkBucket++;
                if (restChunkBucket == threads)
                {
                    restChunkBucket = 0;
                }
            }

            foreach (var kvp in restChunks)
            {
                var bucketKey = kvp.Key;
                var restOfScriptsChunks = kvp.Value;
                foreach (var restOfScriptsChunk in restOfScriptsChunks)
                {
                    threadBuckets[bucketKey].Add(restOfScriptsChunk);
                }
            }

            return threadBuckets;
        }
    }
}