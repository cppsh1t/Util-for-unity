using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Linq;

namespace UnityUtil.BehaviourTree
{
    public static class TreeBuilder
    {
        private static Dictionary<string, Type> builtinNodeMap = new();

        static TreeBuilder()
        {
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            Type[] nodeTypes = allTypes.Where(t => t.Namespace == typeof(BehaviourTree).Namespace 
                && !t.IsAbstract && typeof(Node).IsAssignableFrom(t)).ToArray();
            for(int i = 0; i < nodeTypes.Length; i++)
            {
                Type nodeType = nodeTypes[i];
                builtinNodeMap.TryAdd(nodeType.Name, nodeType);
            }
        }

        public static BehaviourTree BuildTree(MonoBehaviour behaviour, XmlDocument document)
        {
            try
            {
                XmlNode rootNode = document.SelectNodes("Root")[0];
                return BuildTree(behaviour, rootNode);
            }
            catch(IndexOutOfRangeException)
            {
                throw new ArgumentException("XmlDocument don't have Root Node");
            }
        }

        public static BehaviourTree BuildTree(MonoBehaviour behaviour, TextAsset textAsset)
        {
            XmlDocument document = new();
            document.LoadXml(textAsset.text);
            return BuildTree(behaviour, document);
        }

        private static BehaviourTree BuildTree(MonoBehaviour behaviour, XmlNode xmlNode)
        {
            BehaviourTree tree = new(behaviour);
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
                Type type = builtinNodeMap.GetValueOrDefault(name, Type.GetType(name)) 
                    ?? throw new InvalidOperationException($"Can't find type: {name}");
                if (type.IsAbstract || !typeof(Node).IsAssignableFrom(type)) continue;
                Node node = BuildNode(type, xmlNode);
                additiveNode.AddNode(node);
                if (node is IAdditiveNode)
                {
                    BuildTreeWithNodes(node as IAdditiveNode, xmlNode);
                }
            }
        }

        static Node BuildNode(Type type, XmlNode xmlNode)
        {
            Dictionary<string, string> map = new();
            ConstructorInfo TargetCon = null;
            foreach (XmlAttribute att in xmlNode.Attributes)
            {
                string name = att.Name;
                string value = att.Value;
                map.Add(name, value);
            }

            if (xmlNode.Attributes.Count == 0)
            {
                return Activator.CreateInstance(type) as Node;
            }

            ConstructorInfo[] constructorInfos = type.GetConstructors();
            for (int i = 0; i < constructorInfos.Length; i++)
            {
                ConstructorInfo constructorInfo = constructorInfos[i];
                ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
                string[] argNames = parameterInfos.Select(p => p.Name).ToArray();
                if (argNames.Length != map.Keys.Count) continue;
                bool isTarget = true;
                for (int j = 0; j < argNames.Length; j++)
                {
                    string argName = argNames[j];
                    if (!map.ContainsKey(argName))
                    {
                        isTarget = false;
                        break;
                    }
                }
                if (isTarget)
                {
                    TargetCon = constructorInfo;
                    break;
                }

            }

            if (TargetCon == null) throw new MissingMethodException("Missing Right Constructor");

            return MakeByStringMap(TargetCon, map) as Node;
        }


        private static object MakeByStringMap(ConstructorInfo constructor, Dictionary<string, string> map)
        {
            object[] parameters = new object[map.Keys.Count];
            ParameterInfo[] parameterInfos = constructor.GetParameters();
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                string resultString = map[parameterInfo.Name];
                object result = ConvertString(resultString, parameterInfo.ParameterType);
                parameters[i] = result;
            }
            return constructor.Invoke(parameters);
        }

        private static object ConvertString(string str, Type targetType)
        {
            if (targetType == typeof(string)) return str;
            if (targetType == typeof(object)) return str;

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(str);
            }
            if (targetType == typeof(double))
            {
                return Convert.ToDouble(str);
            }
            if (targetType == typeof(float))
            {
                return Convert.ToSingle(str);
            }
            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(str);
            }
            if (targetType == typeof(byte))
            {
                return Convert.ToByte(str);
            }
            if (targetType == typeof(char))
            {
                return Convert.ToChar(str);
            }
            throw new InvalidOperationException($"string can't cast to {targetType}");
        }
    }

}

