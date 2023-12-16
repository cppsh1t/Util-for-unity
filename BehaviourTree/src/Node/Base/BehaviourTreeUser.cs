using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityUtil.MonoEditor;

namespace UnityUtil.BehaviourTree
{
    [AddComponentMenu("BehaviourTree/BehaviourTreeUser")]
    public class BehaviourTreeUser : MonoBehaviour
    {
        [SerializeField]
        private TextAsset behaviourTreeAsset;

        public BehaviourTree BehaviourTree {get; private set;}

        void Awake() 
        {
            if (behaviourTreeAsset == null)
            {
                throw new InvalidOperationException("BehaviourTree Asset can't be null");
            }
            BehaviourTree = TreeBuilder.BuildTree(behaviourTreeAsset);
            BehaviourTree.Init(this);
        }

        void Update() 
        {
            BehaviourTree.Update();
        }

        void FixedUpdate() 
        {
            BehaviourTree.FixedUpdate();
        }
    }
}