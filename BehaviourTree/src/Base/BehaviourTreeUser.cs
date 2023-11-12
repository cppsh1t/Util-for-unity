using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    [AddComponentMenu("BehaviourTree/BehaviourTreeUser")]
    public class BehaviourTreeUser : MonoBehaviour
    {
        [SerializeField]
        private TextAsset behaviourTreeAsset;

        private BehaviourTree behaviourTree;

        void Awake() 
        {
            if (behaviourTreeAsset == null)
            {
                throw new InvalidOperationException("BehaviourTree Asset can't be null");
            }
            behaviourTree = TreeBuilder.BuildTree(this, behaviourTreeAsset);
        }

        void Update() 
        {
            behaviourTree.Update();
        }

        void FixedUpdate() 
        {
            behaviourTree.FixedUpdate();
        }
    }
}