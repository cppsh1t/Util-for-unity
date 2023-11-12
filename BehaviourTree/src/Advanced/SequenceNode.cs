using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UnityUtil.BehaviourTree
{
    public class SequenceNode : CompositeNode
    {
        public int CurrentIndex {get; private set;}
        protected Node CurrentChild => ChildrenArray[CurrentIndex];
        protected Node[] ChildrenArray;

        protected override void OnFixedUpdate()
        {
            CurrentChild.FixedUpdate();
        }

        protected override void OnStart()
        {
            CurrentIndex = 0;
            ChildrenArray = Children.ToArray();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            switch (CurrentChild.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    CurrentIndex++;
                    break;
            }

            if (CurrentIndex == Children.Count)
                return State.Success;
            else
                return State.Running;
        }
    }
}


