using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

namespace UnityUtil.BehaviourTree
{
    public static class TreeBuilder
    {
        private static Dictionary<string, NodeDefinition> nodeTypeMap = new();

        public static ICollection<NodeDefinition> NodeDefinitions => nodeTypeMap.Values;

        static TreeBuilder()
        {
            HashSet<Type> nodeTypes = new()
            {
                typeof(DelayNode),typeof(LogNode),typeof(ParallelNode),
                typeof(RepeatNode),typeof(SequenceNode),typeof(WaitNode)
            };

            IEnumerable<Type> otherNodeTypes = Assembly.GetCallingAssembly().GetTypes().Where(type => !type.IsAbstract && typeof(Node).IsAssignableFrom(type));
            nodeTypes.AddRange(otherNodeTypes);
            nodeTypes.Remove(typeof(RootNode));

            foreach(var nodeType in nodeTypes)
            {
                nodeTypeMap.Add(nodeType.Name, NodeDefinition.CretaeDefinition(nodeType));
            }
        }

        public static NodeDefinition GetNodeDefinition(string nodeName)
        {
            return nodeTypeMap.GetValueOrDefault(nodeName);
        }

        public static BehaviourTree BuildTree(XmlDocument document)
        {
            XmlNode rootNode = document.SelectSingleNode("/RootNode")
                   ?? throw new InvalidOperationException("This BehaviourTree xml not have Root Node");
            return BuildTree(rootNode);
        }

        public static BehaviourTree BuildTree(TextAsset textAsset)
        {
            XmlDocument document = new();
            document.LoadXml(textAsset.text);
            return BuildTree(document);
        }

        private static BehaviourTree BuildTree(XmlNode xmlNode)
        {
            BehaviourTree tree = new();
            BuildTreeWithNodes(tree.rootNode, xmlNode);
            return tree;
        }

        static void BuildTreeWithNodes(IAdditiveNode additiveNode, XmlNode fatherNode)
        {
            if (!fatherNode.HasChildNodes) return;
            string[] names = new string[fatherNode.ChildNodes.Count];
            int index = 0;
            foreach (XmlNode node in fatherNode.ChildNodes)
            {
                names[index] = node.Name;
                index++;
            }

            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                XmlNode xmlNode = fatherNode.ChildNodes[i];
                NodeDefinition definition = nodeTypeMap.GetValueOrDefault(name)
                    ?? throw new InvalidOperationException($"Can't find type: {name}");
                Node node = BuildNode(definition, xmlNode);
                additiveNode.AddNode(node);
                if (node is IAdditiveNode)
                {
                    BuildTreeWithNodes(node as IAdditiveNode, xmlNode);
                }
            }
        }

        static Node BuildNode(NodeDefinition nodeDefinition, XmlNode xmlNode)
        {
            Node node = Activator.CreateInstance(nodeDefinition.NodeObjectType) as Node
                ?? throw new MissingMethodException("Node need noArgs Constructor");
            if (xmlNode.Attributes.Count == 0) return node;

            foreach (XmlAttribute att in xmlNode.Attributes)
            {
                string name = att.Name;
                string value = att.Value;
                nodeDefinition.SetMemberValue(name, node, value);
            }

            return node;
        }

    }

}

