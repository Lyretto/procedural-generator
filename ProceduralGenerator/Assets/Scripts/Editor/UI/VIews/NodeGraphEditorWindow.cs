using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lyred
{
    public class NodeGraphEditorWindow : EditorWindow
    {
        SerializedNodeGraph serializer;
        NodeGraphSettings settings;
        NodeGraphView treeView;
        InspectorView inspectorView;
        BlackboardGraphView _blackboardGraphView;

        private ScrollView blackboardItemPopup;
        //OverlayView overlayView;
        ToolbarMenu toolbarMenu;
        private Button blackboardAddButton;
        Label titleLabel;

        [MenuItem("Lyred/Editor")]
        public static void OpenWindow() => CreateWindow();

        private static void OpenWindow(NodeGraph graph)
        {
            CreateWindow().SelectTree(graph);
        }

        private static NodeGraphEditorWindow CreateWindow()
        {
            var wnd = GetWindow<NodeGraphEditorWindow>();
            wnd.titleContent = new GUIContent("Graph Node Editor");
            wnd.minSize = new Vector2(800, 600);
            return wnd;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line) {
            if (Selection.activeObject is not NodeGraph) return false;
            OpenWindow(Selection.activeObject as NodeGraph);
            return true;
        }

        public void CreateGUI() {

            settings = NodeGraphSettings.GetOrCreateSettings();
            var root = rootVisualElement;
            var visualTree = settings.behaviourTreeXml;
            visualTree.CloneTree(root);
            var styleSheet = settings.behaviourTreeStyle;
            root.styleSheets.Add(styleSheet);
            treeView = root.Q<NodeGraphView>();
            inspectorView = root.Q<InspectorView>();
            _blackboardGraphView = root.Q<BlackboardGraphView>();
            toolbarMenu = root.Q<ToolbarMenu>();
           // overlayView = root.Q<OverlayView>("OverlayView");
            titleLabel = root.Q<Label>("TitleLabel");
            blackboardAddButton = root.Q<Button>("add-blackboard-item");
            blackboardItemPopup = root.Q<ScrollView>("item-popup");
            
            toolbarMenu.RegisterCallback<MouseEnterEvent>((evt) => {
                toolbarMenu.menu.MenuItems().Clear();
                var behaviourTrees = GetAssetPaths<NodeGraph>();
                behaviourTrees.ForEach(path => {
                    var fileName = System.IO.Path.GetFileName(path);
                    toolbarMenu.menu.AppendAction($"{fileName}", (a) => {
                        var tree = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
                        SelectTree(tree);
                    });
                });
                toolbarMenu.menu.AppendSeparator();
                toolbarMenu.menu.AppendAction("New Graph...", (a) => OnToolbarNewAsset());
            });
            treeView.OnNodeSelected = OnNodeSelectionChanged;
            treeView.OnNodeDeselected = _ => OnNodeSelectionChanged(null);
            //overlayView.OnTreeSelected += SelectTree;
            Undo.undoRedoPerformed += OnUndoRedo;

            SelectTree(serializer?.graph);
        }

        private void OnUndoRedo() {
            if (serializer != null) {
                treeView.PopulateView(serializer);
            }
        }

        private void OnEnable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj) {
            switch (obj) {
                case PlayModeStateChange.EnteredEditMode:
                    EditorApplication.delayCall += OnSelectionChange;
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    EditorApplication.delayCall -= OnSelectionChange;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }
        
        private void OnSelectionChange()
        {
            var activeObject = Selection.activeGameObject;
            if (serializer != null && (!activeObject || activeObject != serializer.graph.parentObject))
            {
                if(!serializer.graph.parentObject) return; // && !activeObject)
                foreach (Transform t in serializer.graph.parentObject.transform)
                {
                    DestroyImmediate(t.gameObject);
                }
                SelectTree(null);
            }

            if (!activeObject) return;
            
            var runner = Selection.activeGameObject.GetComponent<NodeGraphRunner>();
            
            if (!runner) return;
            
            SelectTree(runner.graph);
            runner.graph.Generate(runner.gameObject);
        }

        private void SelectTree(NodeGraph newTree) {
            if (!newTree) {
                ClearSelection();
                return;
            }

            serializer = new SerializedNodeGraph(newTree);

            if (titleLabel != null) {
                var path = AssetDatabase.GetAssetPath(serializer.graph);
                if (path == "") {
                    path = serializer.graph.name;
                }
                titleLabel.text = $"Graph View ({path})";
            }
            treeView?.PopulateView(serializer);
            
            _blackboardGraphView.Bind(newTree.blackboard, rootVisualElement);

            blackboardAddButton.clickable.clicked += () => {
                var popup = new GenericMenu();
                var enumValues = System.Enum.GetValues(typeof(ItemType));
                foreach (ItemType value in enumValues)
                {
                    var menuItem = new GUIContent(value.ToString());
                    popup.AddItem(menuItem, false, () =>
                    {
                        serializer.graph.blackboard.AddItem(value.ToString(), default, value);
                        _blackboardGraphView.Bind(serializer.graph.blackboard, rootVisualElement);
                    });
                }
                
                popup.ShowAsContext();
            };
        }

        private void ClearSelection() {
            serializer = null;
            treeView.ClearView();
        }

        private void ClearIfSelected(string path) {
            if (AssetDatabase.GetAssetPath(serializer.graph) == path) {
                EditorApplication.delayCall += () => { SelectTree(null); };
            }
        }

        private void OnNodeSelectionChanged(NodeView node) {
            if (serializer != null && node?.node != serializer.graph.currentNode)
            {
                serializer.graph.currentNode = node?.node;
                serializer.graph.Generate( serializer.graph.parentObject);
            }

            inspectorView.UpdateSelection(serializer, node);
        }

        void OnToolbarNewAsset() {
            var tree = CreateNewTree("New Behaviour Tree", "Assets/");
            if (tree) {
                SelectTree(tree);
            }
        }
        private static NodeGraph CreateNewTree(string assetName, string folder) {
            
            var path = System.IO.Path.Join(folder, $"{assetName}.asset");
            if (System.IO.File.Exists(path)) {
                Debug.LogError($"Failed to create behaviour tree asset: Path already exists:{assetName}");
                return null;
            }
            var tree = CreateInstance<NodeGraph>();
            tree.name = assetName;
            AssetDatabase.CreateAsset(tree, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(tree);
            return tree;
        }
        
        private static List<string> GetAssetPaths<T>() where T : Object 
        {
            var assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            return assetIds.Select(AssetDatabase.GUIDToAssetPath).ToList();
        }
    }
}
