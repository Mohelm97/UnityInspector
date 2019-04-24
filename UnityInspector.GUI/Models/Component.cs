using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityInspector.GUI.Models
{
    public class Component
    {
        public GameObject Owner { get; }
        public string Name { get; }
        public bool IsBehaviour { get; }
        public int Index { get; }
        private bool _enabled;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (!IsBehaviour) return;
                _enabled = value;
                CommunicatorClient.Instance.SetValueOfMember (Owner, Index + ":enabled", _enabled);
            }
        }

        public Component (GameObject gameObject, int index, string name, byte state)
        {
            Index = index;
            Owner = gameObject;
            Name = name;
            if (state == 0)
            {
                IsBehaviour = false;
                _enabled = true;
            } else
            {
                IsBehaviour = true;
                _enabled = (state == 0x01);
            }
        }

        public ObjectMember[] Members
        {
            get
            {
                return CommunicatorClient.Instance.GetComponentMembers (this);
            }
        }
        public override string ToString ()
        {
            return Name;
        }

        public override bool Equals (object obj)
        {
            
            if (obj != null && obj is Component) {
                return (Owner.Equals(((Component) obj).Owner) && Index == ((Component) obj).Index && Name == ((Component) obj).Name);
            }
            return false;
        }
        public override int GetHashCode ()
        {
            return Owner.GetHashCode () + Index + Name.GetHashCode ();
        }
    }
}
