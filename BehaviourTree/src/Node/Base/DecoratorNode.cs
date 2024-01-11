using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    public abstract class DecoratorNode : Node, IAdditiveNode
    {
        public Node Child { get; private set; }
        private Node[] castChildArray;
        public void AddNode(Node node)
        {
            Child = node;
            node.Init(behaviour);
            castChildArray = new Node[1] {node};
        }

        public void ClearChild()
        {
            Child = null;
            castChildArray[0] = null;
        }

        public ICollection<Node> GetChildren()
        {
            return castChildArray;
        }
    }
}
