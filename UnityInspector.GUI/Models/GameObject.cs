using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityInspector.Communicator;

namespace UnityInspector.GUI.Models
{
    public class GameObject
    {
        private bool _active;
        public int Address { get; }
        public string Name { get; }
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                CommunicatorClient.Instance.InvokeMethod (this, "SetActive", new object[] { _active });
            }
        }
        public GameObject[] Children
        {
            get
            {
                if (CommunicatorClient.Instance == null || !CommunicatorClient.Instance.Connected)
                    return new GameObject[0];
                if (Address == 0)
                    return CommunicatorClient.Instance.ReadHierarchy ();
                return CommunicatorClient.Instance.GetChildren (this);
            }
        }

        public Component[] Components
        {
            get
            {
                if (Address == 0 || CommunicatorClient.Instance == null || !CommunicatorClient.Instance.Connected)
                    return new Component[0];
                return CommunicatorClient.Instance.GetComponents (this);
            }
        }

        public GameObject (int address, string name, bool active)
        {
            Address = address;
            Name = name;
            _active = active;
        }

        public override string ToString ()
        {
            return Name;
        }
        public override bool Equals (object obj)
        {
            if (obj is GameObject && obj != null)
                return Address == ((GameObject) obj).Address;
            return false;
        }
        public override int GetHashCode ()
        {
            return Address;
        }
    }
}
