using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityUtil.BehaviourTree
{
    public interface IAdditiveNode
    {
        public void AddNode(Node node);
        public ICollection<Node> GetChildren();
    }

    public sealed class RootNode : Node, IAdditiveNode
    {
        public Node Child {get; private set;}
        private Node[] castChildArray;

        protected override void OnFixedUpdate() 
        { 
            Child.FixedUpdate();
        }

        protected override void OnStart() { }
        protected override void OnStop() { }
        protected override State OnUpdate() => Child.Update();

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