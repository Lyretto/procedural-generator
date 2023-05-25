using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lyred
{
    [Serializable]
    public class Blackboard
    {
        [SerializeReference] public List<BlackboardItem> items = new ();

        public BlackboardItem AddItem(string name, object defaultValue, ItemType type)
        {
            var item = new BlackboardItem(name, defaultValue, type);
            items.Add(item);
            return item;
        }
    }

    [Serializable]
    public class BlackboardItem
    {
        [SerializeReference] public string Id;
        [SerializeReference]  public string id;
        public object Value;
        public object DefaultValue;
        [SerializeReference] public ItemType type;

        public BlackboardItem(string name, object value, ItemType type)
        {
            Id = name;
            id = Guid.NewGuid().ToString();
            Value = value;
            DefaultValue = value;
            this.type = type;
        }

        public object GetValue() => Value ?? DefaultValue;
    }
    
    public enum ItemType
    {
        Float,
        Int,
        Vector2,
        Vector3,
        Color,
        Spline,
        GameObject,
        Mesh,
        Bool,
    }
}