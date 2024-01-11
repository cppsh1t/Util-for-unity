using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtil.BehaviourTree.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public NodeDefinition NodeDefinition { get; private set; }
        public NodeDefinition.NodeType NodeType { get; private set; }
        internal Port inputPort;
        internal Port outputPort;
        public readonly Dictionary<string, string> nodeValueMap = new();
        private NodeCanvas canvas;

        protected NodeView(NodeDefinition.NodeType NodeType, NodeCanvas nodeCanvas)
        {
            canvas = nodeCanvas;
            this.NodeType = NodeType;
            CreateInputPorts();
            CreateOutputPorts();
        }

        public NodeView(NodeDefinition nodeDefinition, NodeCanvas nodeCanvas) : this(nodeDefinition.ChildrenType, nodeCanvas)
        {
            NodeDefinition = nodeDefinition;
            title = nodeDefinition.NodeName;
        }

        internal void SetMemberValue(string name, string value)
        {
            nodeValueMap[name] = value;
        }

        private void CreateInputPorts()
        {
            if (NodeType == NodeDefinition.NodeType.Root)
            {
                //no input
            }
            else
            {
                inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (inputPort != null)
            {
                inputPort.portName = "";
                inputPort.style.flexDirection = FlexDirection.Column;
                this.inputContainer.Add(inputPort);
            }
        }

        private void CreateOutputPorts()
        {
            if (NodeType == NodeDefinition.NodeType.Action)
            {
                //no output
            }
            else if (NodeType == NodeDefinition.NodeType.Composite)
            {
                outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else
            {
                outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (outputPort != null)
            {
                outputPort.portName = "";
                outputPort.style.flexDirection = FlexDirection.Column;
                this.outputContainer.Add(outputPort);
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();
            canvas.nodeNameLabel.text = title;

            CreateNodeInspector(canvas.valueBox);
        }

        private void CreateNodeInspector(VisualElement valueBox)
        {
            valueBox.Clear();
            if (NodeDefinition == null) return;

            foreach (var part in NodeDefinition.memberTypeMap)
            {
                var (name, type) = part;
                UnityEngine.UIElements.PopupWindow popupWindow = new() { text = "NodeMember" };
                Label nameLabel = new() { text = $"memberName: {name}" };
                Label typeLabel = new() { text = $"type: {type.Name}" };
                TextField setterField = new();
                popupWindow.Add(nameLabel);
                popupWindow.Add(typeLabel);
                popupWindow.Add(setterField);
                valueBox.Add(popupWindow);
                string fieldValue = nodeValueMap.GetValueOrDefault(name);
                if(fieldValue != null)
                {
                    setterField.value = fieldValue;
                }

                setterField.RegisterValueChangedCallback(evt => SetMemberValue(name, evt.newValue));
            }
        }

        public static NodeView CreateRootNodeView(NodeCanvas nodeCanvas)
        {
            var root = new NodeView(NodeDefinition.NodeType.Root, nodeCanvas) { title = "RootNode" };
            return root;
        }

    }


}


