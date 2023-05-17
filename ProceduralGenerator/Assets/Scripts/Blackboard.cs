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
    }

    [System.Serializable]
    public class BlackboardItem
    {
        public string Id;
        public object Value;
        public string Type;

        public BlackboardItem(string id, object value, string type)
        {
            Id = id;
            Value = value;
            Type = type;
        }
    }
}