using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityUtil.BehaviourTree
{
    public abstract class Node
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        public State CurrentState { get; private set; }
        public bool Started { get; private set; }
        public virtual string Group {get; private set;} = "default";

        protected MonoBehaviour behaviour;

        public virtual void Init(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        public State Update()
        {
            if (!Started)
            {
                OnStart();
                Started = true;
            }

            CurrentState = OnUpdate();

            if (CurrentState == State.Failure || CurrentState == State.Success)
            {
                OnStop();
                Started = false;
            }

            return CurrentState;
        }

        public void FixedUpdate()
        {
            if (!Started)
            {
                OnStart();
                Started = true;
            }
            OnFixedUpdate();
        }

        protected abstract void OnStart();

        protected abstract void OnStop();

        protected abstract State OnUpdate();

        protected abstract void OnFixedUpdate();
    }
}

