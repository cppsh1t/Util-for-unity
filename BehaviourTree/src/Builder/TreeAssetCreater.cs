using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtil.BehaviourTree
{
#if UNITY_EDITOR

    class TreeAssetCreateWindow : EditorWindow
    {

        [MenuItem("Assets/Create/BehaviourTree/BehaviourTreeAsset")]
        public static void OpenWindow()
        {
            if (Selection.activeObject != null)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!File.Exists(path) && Directory.Exists(path))
                {
                    TreeAssetCreateWindow window = GetWindow<TreeAssetCreateWindow>();
                    window.titleContent = new GUIContent("BehaviourTreeAssetCreater");
                }
            }
        }

        public void CreateGUI()
        {
            string selectedFolderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var root = rootVisualElement;
            root.style.height = new StyleLength(200);
            TextField nameField = new("name");
            nameField.style.marginTop = new StyleLength(16);
            Button button = new(() => CreateTreeAsset($"{selectedFolderPath}/{nameField.value}.xml")) { text = "create" };
            button.style.height = new StyleLength(30);
            button.style.marginTop = new StyleLength(16);
            root.Add(nameField);
            root.Add(button);
        }

        private void CreateTreeAsset(string path)
        {
            XmlDocument document = new();
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(xmlDeclaration);
            document.AppendChild(document.CreateElement("RootNode"));
            document.Save(path);
            AssetDatabase.Refresh();
        }
    }

#endif
}


