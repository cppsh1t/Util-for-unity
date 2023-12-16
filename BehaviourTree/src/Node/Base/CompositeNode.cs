using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace UnityUtil.BehaviourTree
{
    public abstract class CompositeNode : Node, IAdditiveNode
    {
        public virtual ICollection<Node> Children { get; private set; } = new List<Node>();

        public virtual void AddNode(Node node)
        {
            node.SetBehaviour(behaviour);
            Children.Add(node);
        }

        public virtual void AddNode(params Node[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                Node node = nodes[i];
                node.SetBehaviour(behaviour);
                Children.Add(node);
            }
        }

        public virtual void RemoveNode(Node node) => Children.Remove(node);
        
        public ICollection<Node> GetChildren()
        {
            return Children;
        }

    }

}