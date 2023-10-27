using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;

[AttributeUsage(AttributeTargets.Method)]
public class OnInspectorAttribute : Attribute
{
    public string Name { get; private set; }

    public OnInspectorAttribute() { }

    public OnInspectorAttribute(string name) => Name = name;
}


#if UNITY_EDITOR

[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoEditor : Editor
{
    private MonoBehaviour Target => target as MonoBehaviour;
    private Type TargetType => target.GetType();

    private static readonly HashSet<Type> initTypes = new();
    
    private static readonly Dictionary<Type, Dictionary<string, MethodInfo>> typeMap = new();

    private static readonly GUILayoutOption width = GUILayout.Width(300);
    private static readonly GUILayoutOption height = GUILayout.Height(25);

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
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var methodMap = typeMap.GetValueOrDefault(TargetType);

        if (methodMap == null) return;

        foreach (var (name, callback) in methodMap)
        {
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(name, width, height))
            {
                callback.Invoke(target, default);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}

#endif