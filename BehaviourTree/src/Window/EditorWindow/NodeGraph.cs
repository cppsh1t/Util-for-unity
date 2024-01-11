using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace UnityUtil.BehaviourTree.Editor
{
    public class NodeGraph : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset visualTree;

        [SerializeField]
        private StyleSheet styleSheet;

        private Button loadButton;
        private Button saveButton;
        private VisualElement inspector;


        [MenuItem("BehaviourTree/BehaviourTreeEditor")]
        public static void BuildEditorWindow()
        {
            NodeGraph nodeGraph = GetWindow<NodeGraph>();
            nodeGraph.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            visualTree.CloneTree(root);
            root.styleSheets.Add(styleSheet);

            var buttonBox = root.Q("InspectorPanel").Q("ButtonBox");
            loadButton = buttonBox.Q<Button>("Load");
            saveButton = buttonBox.Q<Button>("Save");
            inspector = root.Q("InspectorPanel").Q("Inspector");

            XmlField xmlField = root.Q("InspectorPanel").Q<XmlField>("XmlField");

            NodeCanvas nodeCanvas = root.Q<GraphView>("NodeCanvas") as NodeCanvas;
            nodeCanvas.nodeNameLabel = inspector.Q<Label>("NodeName");
            nodeCanvas.valueBox = inspector.Q("ValueBox");
            nodeCanvas.xmlField = xmlField;
            saveButton.RegisterCallback<ClickEvent>(_ => nodeCanvas.Save());
            loadButton.RegisterCallback<ClickEvent>(_ => nodeCanvas.Load());
        }
    }
}

