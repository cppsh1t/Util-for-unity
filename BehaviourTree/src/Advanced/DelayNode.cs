using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    public class DelayNode : DecoratorNode
    {
        public float Duration { get; private set; }
        public float StartTime { get; private set; }
        public float RemainTime => Time.time - StartTime;

        public DelayNode() { }
        public DelayNode(float duration)
        {
            SetDuration(duration);
        }

        protected override void OnFixedUpdate()
        {
            if (RemainTime > Duration)
            {
                Child.FixedUpdate();
            }
        }

        protected override void OnStart() => StartTime = Time.time;

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (RemainTime > Duration)
            {
                return Child.Update();
            }
            return State.Running;
        }

        public void SetDuration(float time) => Duration = time;
    }

}