using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityUtil.BehaviourTree
{
    public class ParallelNode : CompositeNode
    {
        private Node[] childrenArray;
        public bool AllFinshed => FinishArray.All(n => n == true);
        private bool allSuccess = true;
        public bool[] FinishArray { get; private set; }

        protected override void OnStart()
        {
            childrenArray = Children.ToArray();
            FinishArray = new bool[childrenArray.Length];
            for (int i = 0; i < FinishArray.Length; i++)
            {
                FinishArray[i] = false;
            }
        }

        protected override void OnStop() { }
        protected override void OnFixedUpdate()
        {
            for (int i = 0; i < FinishArray.Length; i++)
            {
                if (FinishArray[i] == false)
                {
                    childrenArray[i].FixedUpdate();
                }
            }
        }

        protected override State OnUpdate()
        {
            for (int i = 0; i < FinishArray.Length; i++)
            {
                if (FinishArray[i] == false)//还没结束的节点，继续执行
                {
                    var state = childrenArray[i].Update();
                    if (state == State.Success)//如果执行完毕，更改状态
                    {
                        FinishArray[i] = true;
                    }
                    else if (state == State.Failure)
                    {
                        FinishArray[i] = true;
                        allSuccess = false;
                    }

                }
            }

            if (!AllFinshed) return State.Running;

            return allSuccess ? State.Success : State.Failure;
        }

    }

}

