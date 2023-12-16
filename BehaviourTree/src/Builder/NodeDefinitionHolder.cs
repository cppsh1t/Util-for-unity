using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;

namespace UnityUtil.BehaviourTree
{
    public static class NodeDefinitionHolder
    {
        public readonly static List<NodeDefinition> definitions = new();
        private static string path = @"D:\unityhaha\Chat\Assets\BehaviourTree\src\Builder\NodeDefinitionHolder.cs";
        private static string beAdd = "definitions";

        static NodeDefinitionHolder()
        {
            //StartToken
definitions.Add(NodeDefinition.CretaeDefinition(typeof(UnityUtil.BehaviourTree.DelayNode)));
definitions.Add(NodeDefinition.CretaeDefinition(typeof(UnityUtil.BehaviourTree.LogNode)));
definitions.Add(NodeDefinition.CretaeDefinition(typeof(UnityUtil.BehaviourTree.ParallelNode)));
definitions.Add(NodeDefinition.CretaeDefinition(typeof(UnityUtil.BehaviourTree.RepeatNode)));
definitions.Add(NodeDefinition.CretaeDefinition(typeof(UnityUtil.BehaviourTree.SequenceNode)));
definitions.Add(NodeDefinition.CretaeDefinition(typeof(UnityUtil.BehaviourTree.WaitNode)));
            //EndToken
        }

        #if UNITY_EDITOR

        [MenuItem("BehaviourTree/GenerateCode")]
        public static void GenerateCode()
        {
            string[] lines = File.ReadAllLines(path);

            int startLine = -999;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Contains("StartToken"))
                {
                    startLine = i + 1;
                    break;
                }
            }

            if (startLine == -999)
            {
                throw new ArgumentNullException("Can't find StartToken in code");
            }

            int endLine = -999;

            for (int i = startLine; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Contains("EndToken"))
                {
                    endLine = i - 1;
                    break;
                }
            }

            if (endLine == -999)
            {
                throw new ArgumentNullException("Can't find EndToken in code");
            }

            List<string> lineList = new(lines.Length - (endLine - startLine));
            for (int i = 0; i < startLine; i++)
            {
                lineList.Add(lines[i]);
            }

            for (int i = endLine + 1; i < lines.Length; i++)
            {
                lineList.Add(lines[i]);
            }

            lineList.InsertRange(startLine, GetAddStrings());
            File.WriteAllLines(path, lineList);
            AssetDatabase.Refresh();
            Debug.Log("GenerateCode Over");
        }

        private static IEnumerable<string> GetAddStrings()
        {
            HashSet<Type> nodeTypes = new()
            {
                typeof(DelayNode),typeof(LogNode),typeof(ParallelNode),
                typeof(RepeatNode),typeof(SequenceNode),typeof(WaitNode)
            };

            IEnumerable<Type> otherNodeTypes = TypeCache.GetTypesDerivedFrom<Node>().Where(type => !type.IsAbstract);
            nodeTypes.AddRange(otherNodeTypes);
            nodeTypes.Remove(typeof(RootNode));

            List<string> strings = new(nodeTypes.Count);
            foreach (var type in nodeTypes)
            {
                strings.Add($"{beAdd}.Add(NodeDefinition.CretaeDefinition(typeof({type.Namespace}.{type.Name})));");
            }
            return strings;
        }

        #endif
    }

}
