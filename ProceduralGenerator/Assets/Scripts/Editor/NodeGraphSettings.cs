using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Lyred
{
    internal class NodeGraphSettings : ScriptableObject
    {
        public VisualTreeAsset behaviourTreeXml;
        public StyleSheet behaviourTreeStyle;
        public VisualTreeAsset nodeXml;
        public StyleSheet blackBoardItemStyle;
        public VisualTreeAsset blackBoardItemXml;
        public StyleSheet defaultInputStyle;
        public string newNodeBasePath = "Assets/";

        private static NodeGraphSettings FindSettings()
        {
            var guids = AssetDatabase.FindAssets("t:NodeGraphSettings");
            if (guids.Length > 1)
            {
                Debug.LogWarning($"Found multiple settings files, using the first.");
            }

            switch (guids.Length)
            {
                case 0:
                    return null;
                default:
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath<NodeGraphSettings>(path);
            }
        }

        internal static NodeGraphSettings GetOrCreateSettings()
        {
            var settings = FindSettings();
            if (settings != null) return settings;

            settings = CreateInstance<NodeGraphSettings>();
            AssetDatabase.CreateAsset(settings, "Assets/NodeGraphSettings.asset");
            AssetDatabase.SaveAssets();
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    internal static class MyCustomSettingsUIElementsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Project/MyCustomUIElementsSettings", SettingsScope.Project)
            {
                label = "NodeGraph",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = NodeGraphSettings.GetSerializedSettings();
                    var title = new Label()
                    {
                        text = "Node Graph Settings"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new InspectorElement(settings));

                    rootElement.Bind(settings);
                },
            };
            return provider;
        }
    }
}
