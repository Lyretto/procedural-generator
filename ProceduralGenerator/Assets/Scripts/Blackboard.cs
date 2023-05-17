using System.Collections.Generic;

namespace Lyred
{
    [System.Serializable]
    public class Blackboard
    {
        public List<BlackboardItem> items = new ();

        public Blackboard()
        {
            items.Add(new BlackboardItem("Spykes", null, "Spline"));
        }

        public void AddItem(string name, object defaultValue, string type)
        {
            items.Add(new BlackboardItem(name, defaultValue, type));
        }
    }

    [System.Serializable]
    public class BlackboardItem
    {
        public string Id;
        public object Value;
        public object DefaultValue;
        public string Type;

        public BlackboardItem(string id, object value, string type)
        {
            Id = id;
            Value = value;
            DefaultValue = value;
            Type = type;
        }

        public object GetValue() => Value ?? DefaultValue;
    }
}