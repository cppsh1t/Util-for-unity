using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace UnityUtil.BehaviourTree
{
    #if UNITY_EDITOR
    [Obsolete]
    public class BehaviourTreeShowWindow : EditorWindow
    {
        private BehaviourTreeUser user;
        private BehaviourTree tree;

        // [MenuItem("BehaviourTree/BehaviourTreeShowWindow")]
        static void ShowWindow()
        {
            Object activeObj = Selection.activeObject;
            ShowWindow(activeObj);
        }

        public static void ShowWindow(Object activeObj)
        {

            if (activeObj is GameObject)
            {
                bool exist = (activeObj as GameObject).TryGetComponent<BehaviourTreeUser>(out var user);
                if (exist)
                {
                    var window = GetWindow<BehaviourTreeShowWindow>();
                    window.user = user;
                    window.Show();
                    window.Init();
                    return;
                }
            }

            Debug.LogWarning("This Object don't have BehaviourTreeUser Component");
        }

        private void Init()
        {
            tree ??= user.BehaviourTree;
        }

        void OnGUI()
        {
            if (tree != null)
            {
                GUILayout.TextField($"Tree State: {tree.TreeState}");
            }
            else
            {
                GUILayout.TextField("Window must show in Playing!");
            }
        }
    }

    #endif

}

