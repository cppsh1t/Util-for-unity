using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    public class BehaviourTree
    {
        public readonly RootNode rootNode = new();
        public Node.State TreeState { get; private set; }

        public void Init(MonoBehaviour behaviour)
        {
            rootNode.SetBehaviour(behaviour);
        }

        public void SetNodeOnRoot(Node node)
        {
            rootNode.AddNode(node);
        }

        public Node.State Update()
        {
            if (rootNode.CurrentState == Node.State.Running)
            {
                TreeState = rootNode.Update();
            }
            return TreeState;
        }

        public void FixedUpdate()
        {
            if (rootNode.CurrentState == Node.State.Running)
            {
                rootNode.FixedUpdate();
            }
        }
    }

}
