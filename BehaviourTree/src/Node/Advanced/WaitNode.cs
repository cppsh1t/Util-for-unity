using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    public class WaitNode : ActionNode
    {  
        [SerializeField]
        public float Duration {get; private set;}
        public float StartTime {get; private set;}
        public float RemainTime => Time.time - StartTime;

        public WaitNode() { }
        public WaitNode(float duration)
        {
            SetDuration(duration);
        }

        protected override void OnFixedUpdate() { }

        protected override void OnStart() => StartTime = Time.time;

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (RemainTime > Duration)
            {
                return State.Success;
            }
            return State.Running;
        }

        public void SetDuration(float time) => Duration = time;

    }
}


