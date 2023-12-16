using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR

namespace UnityUtil.MonoEditor
{
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class ToolkitEditor : Editor
    {
        private Type TargetType => target.GetType();
        private static readonly HashSet<Type> initTypes = new();
        private static readonly Dictionary<Type, Dictionary<string, MethodInfo>> typeMap = new();

        private void Init()
        {
            if (initTypes.Contains(TargetType)) return;
            initTypes.Add(TargetType);

            MethodInfo[] methods = TargetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (methods.Length == 0) return;

            Dictionary<string, MethodInfo> methodMap = new();

            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];

                if (method.GetParameters().Count() != 0) continue;

                var attr = method.GetCustomAttribute<OnInspectorAttribute>();
                if (attr == null) continue;

                string name = attr.Name ?? method.Name;
                methodMap.Add(name, method);
            }

            typeMap.Add(TargetType, methodMap);
        }

        public override VisualElement CreateInspectorGUI()
        {
            Init();
            VisualElement root = new();
            VisualElement defaultInspector = new();
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
            root.Add(defaultInspector);

            var methodMap = typeMap.GetValueOrDefault(TargetType);

            if (methodMap != null)
            {
                foreach (var (name, callback) in methodMap)
                {
                    UnityEngine.UIElements.Button button = new(() => callback.Invoke(target, default))
                    {
                        name = name,
                        text = name,
                    };
                    button.style.height = 30;
                    root.Add(button);
                }
            }
            return root;
        }
    }

}


#endif