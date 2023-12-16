using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtil.BehaviourTree.Editor
{
    public class NodeCanvas : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeCanvas, GraphView.UxmlTraits> { }
        private NodeView rootNodeView;

        internal Label nodeNameLabel;
        internal VisualElement valueBox;
        internal XmlField xmlField;

        public NodeCanvas()
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            foreach (var definition in NodeDefinitionHolder.definitions)
            {
                evt.menu.AppendAction($"{definition.NodeName}", evt => CreateNodeView(definition));
            }
        }

        public override EventPropagation DeleteSelection()
        {
            selection.Remove(rootNodeView);
            return base.DeleteSelection();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        private void CreateNodeView(NodeDefinition definition)
        {
            NodeView nodeView = new(definition, this);
            // nodeView.SetPosition(mousePosition);
            AddElement(nodeView);
        }

        public void Save()
        {
            if (xmlField == null || xmlField.value == null)
            {
                Debug.LogWarning("No Document in Editing.");
                return;
            }

            if (xmlField.value is not TextAsset)
            {
                Debug.LogWarning("Wrong Type of BehaviourTree Xml");
                return;
            }

            string path = AssetDatabase.GetAssetPath(xmlField.value);
            if (!path.EndsWith(".xml"))
            {
                Debug.LogWarning("Wrong Type of BehaviourTree Xml");
                return;
            }

            XmlDocument document = new();
            document.LoadXml((xmlField.value as TextAsset).text);

            XmlNode root = document.SelectSingleNode("/RootNode");
            root.RemoveAll();
            root.Attributes.RemoveAll();
            Rect position = rootNodeView.GetPosition();

            XmlAttribute wdith = document.CreateAttribute("_width");
            wdith.Value = position.width.ToString();
            XmlAttribute hegiht = document.CreateAttribute("_hegiht");
            hegiht.Value = position.height.ToString();

            XmlAttribute xPosition = document.CreateAttribute("_x");
            xPosition.Value = position.x.ToString();
            XmlAttribute yPosition = document.CreateAttribute("_y");
            yPosition.Value = position.y.ToString();
            root.Attributes.Append(wdith);
            root.Attributes.Append(hegiht);
            root.Attributes.Append(xPosition);
            root.Attributes.Append(yPosition);

            if (rootNodeView.outputPort.connected)
            {
                NodeView childNodeView = rootNodeView.outputPort.connections.First().input.node as NodeView;
                BuildXmlNode(document, root, childNodeView);
            }

            document.Save(path);
            Debug.Log($"Save TreeAsset: {path}");
            AssetDatabase.Refresh();
        }

        private void BuildXmlNode(XmlDocument document, XmlNode fatherNode, NodeView nodeView)
        {
            string nodeName = nodeView.title;
            XmlNode xmlNode = document.CreateElement(nodeName);

            Rect position = nodeView.GetPosition();

            XmlAttribute wdith = document.CreateAttribute("_width");
            wdith.Value = position.width.ToString();
            XmlAttribute hegiht = document.CreateAttribute("_hegiht");
            hegiht.Value = position.height.ToString();

            XmlAttribute xPosition = document.CreateAttribute("_x");
            xPosition.Value = position.x.ToString();
            XmlAttribute yPosition = document.CreateAttribute("_y");
            yPosition.Value = position.y.ToString();
            xmlNode.Attributes.Append(wdith);
            xmlNode.Attributes.Append(hegiht);
            xmlNode.Attributes.Append(xPosition);
            xmlNode.Attributes.Append(yPosition);

            foreach (var (name, value) in nodeView.nodeValueMap)
            {
                if (string.IsNullOrEmpty(name)) continue;

                XmlAttribute attribute = document.CreateAttribute(name);
                attribute.Value = value;
                xmlNode.Attributes.Append(attribute);
            }
            fatherNode.AppendChild(xmlNode);

            if (nodeView.outputPort != null && nodeView.outputPort.connected)
            {
                foreach (Edge edge in nodeView.outputPort.connections)
                {
                    NodeView childNodeView = edge.input.node as NodeView;
                    BuildXmlNode(document, xmlNode, childNodeView);
                }
            }
        }

        public void Load()
        {
            if (xmlField == null || xmlField.value == null)
            {
                Debug.LogWarning("No Document in Editing.");
                return;
            }

            if (xmlField.value is not TextAsset)
            {
                Debug.LogWarning("Wrong Type of BehaviourTree Xml");
                return;
            }

            string path = AssetDatabase.GetAssetPath(xmlField.value);
            if (!path.EndsWith(".xml"))
            {
                Debug.LogWarning("Wrong Type of BehaviourTree Xml");
                return;
            }

            Debug.Log($"Load TreeAsset: {path}");

            XmlDocument document = new();
            document.LoadXml((xmlField.value as TextAsset).text);

            XmlNode root = document.SelectSingleNode("/RootNode");
            DeleteElements(graphElements);
            rootNodeView = NodeView.CreateRootNodeView(this);
            AddElement(rootNodeView);

            float width = 0, hegiht = 0, x = 0, y = 0;
            foreach (XmlAttribute att in root.Attributes)
            {
                switch (att.Name)
                {
                    case "_width": width = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    case "_hegiht": hegiht = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    case "_x": x = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    case "_y": y = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    default: break;
                }
            }
            rootNodeView.SetPosition(new Rect(x, y, width, hegiht));


            if (root.HasChildNodes)
            {
                foreach (XmlNode childNode in root.ChildNodes)
                {
                    BuildGraph(childNode, rootNodeView);
                }
            }

        }

        private void BuildGraph(XmlNode xmlNode, NodeView fatherNodeView)
        {
            string nodeName = xmlNode.Name;
            NodeDefinition nodeDefinition = TreeBuilder.GetNodeDefinition(nodeName);
            NodeView nodeView = new(nodeDefinition, this);
            Edge edge = fatherNodeView.outputPort.ConnectTo(nodeView.inputPort);
            AddElement(edge);

            float width = 0, hegiht = 0, x = 0, y = 0;
            foreach (XmlAttribute att in xmlNode.Attributes)
            {
                switch (att.Name)
                {
                    case "_width": width = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    case "_hegiht": hegiht = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    case "_x": x = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    case "_y": y = (float)Convert.ChangeType(att.Value, typeof(float)); break;
                    default: nodeView.SetMemberValue(att.Name, att.Value); break;
                }
            }
            AddElement(nodeView);
            nodeView.SetPosition(new Rect(x, y, width, hegiht));

            if (xmlNode.HasChildNodes)
            {
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    BuildGraph(childNode, nodeView);
                }
            }
        }
    }
}