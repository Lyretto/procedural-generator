using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lyred
{
    [Serializable]
    public class Blackboard
    {
        [SerializeReference]
        public List<BlackboardItem> items = new ();

        public void AddItem(string name, object defaultValue, ItemType type)
        {
            items.Add(new BlackboardItem(name, defaultValue, type));
        }
    }

    [Serializable]
    public class BlackboardItem
    {
        public string Id;
        public string id;
        public object Value;
        public object DefaultValue;
        public ItemType type;

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