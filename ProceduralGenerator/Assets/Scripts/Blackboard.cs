using System.Collections.Generic;

namespace Lyred
{
    [System.Serializable]
    public class Blackboard
    {
        public List<BlackboardItem> items = new ();

        public Blackboard()
        {
            items.Add(new BlackboardItem("int", 2));
        }
    }

    [System.Serializable]
    public struct BlackboardItem
    {
        public string Id;
        public object Value;

        public BlackboardItem(string id, object value)
        {
            Id =id;
            Value = value;
        }
    }
}