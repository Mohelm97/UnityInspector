using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityInspector.Communicator;

namespace UnityInspector.GUI.Models
{
    public class ObjectMember
    {
        private object _value;

        public Component Owner { get; }
        public ObjectMember Parent { get; }
        public CommunicatorClient.Types Type { get; }
        public string Name { get; }
        public object Value {
            get {
                return _value;
            }
            set
            {
                object tempValue;
                if (value is string)
                    tempValue = FromString ((string) value);
                else
                    tempValue = value;
                if (!_value.Equals(tempValue) && tempValue != null)
                {
                    _value = tempValue;
                    UpdateValue ();
                }
            }
        }

        public ObjectMember (Component owner, Communicator<GameObject>.Types type, string name, object value)
        {
            Owner = owner;
            Type = type;
            Name = name;
            _value = value;
            if (_value is Vector3)
                ((Vector3) _value).objectMember = this;
            else if (_value is EnumMember)
                ((EnumMember) _value).objectMember = this;
            else if (_value is Vector2)
                ((Vector2) _value).objectMember = this;
            else if (_value is Rect)
                ((Rect) _value).objectMember = this;
            else if (_value is ArrayMember)
                ((ArrayMember) _value).objectMember = this;
        }
        public ObjectMember (ObjectMember parent, Communicator<GameObject>.Types type, string name, object value) : this (parent.Owner, type, name, value)
        {
            Parent = parent;
        }

        public object FromString (string value)
        {
            double valueDouble = 0;
            if (Type > CommunicatorClient.Types.Bool && Type < CommunicatorClient.Types.String)
            {
                bool isDouble = double.TryParse (value, out valueDouble);
                if (!isDouble)
                    return null;
            }

            switch (Type)
            {
                case CommunicatorClient.Types.Bool:
                    return value == "True";
                case CommunicatorClient.Types.Byte:
                    return (byte) valueDouble;
                case CommunicatorClient.Types.Short:
                    return (short) valueDouble;
                case CommunicatorClient.Types.Int:
                    return (int) valueDouble;
                case CommunicatorClient.Types.Long:
                    return (long) valueDouble;
                case CommunicatorClient.Types.Float:
                    return (float) valueDouble;
                case CommunicatorClient.Types.Double:
                    return (double) valueDouble;
                case CommunicatorClient.Types.String:
                    return value;
                case CommunicatorClient.Types.GameObject:
                    return null;
            }

            return null;
        }
        public void UpdateValue ()
        {
            CommunicatorClient.Instance.SetValueOfMember (Owner.Owner, GetPath (), _value);
        }
        public object GetValue ()
        {
            return CommunicatorClient.Instance.GetValueOfMember (Owner.Owner, GetPath (), _value);
        }
        public string GetPath()
        {
            return (Parent == null) ? Owner.Index + ":" + Name : Parent.GetPath() + "." + Name;
        }

        public override bool Equals (object obj)
        {
            if (obj != null && obj is ObjectMember)
                return (Owner.Equals (((ObjectMember) obj).Owner) && Name == ((ObjectMember) obj).Name);
            return false;
        }
        public override int GetHashCode ()
        {
            return Owner.GetHashCode () + Name.GetHashCode();
        }
    }
}
