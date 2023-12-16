using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    public class RepeatNode : DecoratorNode 
    {
        public bool stopWhenFailure = true;

        protected override void OnFixedUpdate() 
        { 
            Child.FixedUpdate();
        }

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            var state = Child.Update();
            if (state == State.Failure && stopWhenFailure)
                return State.Failure;
            return State.Running;
        }

    }
}