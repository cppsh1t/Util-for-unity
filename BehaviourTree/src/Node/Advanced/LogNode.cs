using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    public class LogNode : ActionNode
    {
        public string message;

        public LogNode() { }
        public LogNode(string message)
        {
            this.message = message;
        }

        protected override void OnFixedUpdate() { }

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            Debug.Log(message);
            return State.Success;
        }
    }
}
