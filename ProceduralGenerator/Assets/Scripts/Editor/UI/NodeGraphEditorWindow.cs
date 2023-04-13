using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lyred
{
    public class NodeGraphEditorWindow : EditorWindow
    {/*
        public class Test : AssetModificationProcessor {

            static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt) {
                NodeGraphEditorWindow wnd = GetWindow<NodeGraphEditorWindow>();
                wnd.ClearIfSelected(path);
                return AssetDeleteResult.DidNotDelete;
            }
        }*/

        SerializedNodeGraph serializer;
        NodeGraphSettings settings;

        NodeGraphView treeView;
        InspectorView inspectorView;
        BlackboardGraphView _blackboardGraphView;
        //OverlayView overlayView;
        ToolbarMenu toolbarMenu;
        Label titleLabel;

        [MenuItem("Lyred/Editor")]
        public static void OpenWindow() {
            var wnd = GetWindow<NodeGraphEditorWindow>();
            wnd.titleContent = new GUIContent("Graph Node Editor");
            wnd.minSize = new Vector2(800, 600);
        }

        public static void OpenWindow(NodeGraph graph) {
            var wnd = GetWindow<NodeGraphEditorWindow>();
            wnd.titleContent = new GUIContent("Graph Node Editor");
            wnd.minSize = new Vector2(800, 600);
            wnd.SelectTree(graph);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line) {
            if (Selection.activeObject is NodeGraph) {
                OpenWindow(Selection.activeObject as NodeGraph);
                return true;
            }
            return false;
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
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    EditorApplication.delayCall += OnSelectionChange;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange() {
            if (Selection.activeGameObject) {
                NodeGraphRunner runner = Selection.activeGameObject.GetComponent<NodeGraphRunner>();
                if (runner) {
                    SelectTree(runner.graph);
                }
            }
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
            treeView.PopulateView(serializer);
            _blackboardGraphView.Bind(serializer);
        }

        void ClearSelection() {
            serializer = null;
            //overlayView.Show();
            treeView.ClearView();
        }

        void ClearIfSelected(string path) {
            if (AssetDatabase.GetAssetPath(serializer.graph) == path) {
                // Need to delay because this is called from a will delete asset callback
                EditorApplication.delayCall += () => {
                    SelectTree(null);
                };
            }
        }

        private void OnNodeSelectionChanged(NodeView node) {
            inspectorView.UpdateSelection(serializer, node);
        }

        private void OnInspectorUpdate() {
            treeView?.UpdateNodeStates();
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
        private static List<string> GetAssetPaths<T>() where T : UnityEngine.Object {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<string> paths = new List<string>();
            foreach (var assetId in assetIds) {
                string path = AssetDatabase.GUIDToAssetPath(assetId);
                paths.Add(path);
            }
            return paths;
        }
    }
    }
