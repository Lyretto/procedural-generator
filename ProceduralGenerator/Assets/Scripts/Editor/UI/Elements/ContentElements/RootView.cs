using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lyred {
    public class RootView : VisualElement 
{
        public new class UxmlFactory : UxmlFactory<RootView, UxmlTraits> { }

        public System.Action<NodeGraph> OnTreeSelected;

        DropdownField assetSelector;

        public void Show()
        {
            var behaviourTrees = GetAssetPaths<GameObject>();
            assetSelector.choices = new List<string>();
            behaviourTrees.ForEach(treePath => {
                assetSelector.choices.Add(ToMenuFormat(treePath));
            });
        }
        
        public string ToMenuFormat(string one) {
            // Using the slash creates submenus...
            return one.Replace("/", "|");
        }
        
        public static List<string> GetAssetPaths<T>() where T : UnityEngine.Object {
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