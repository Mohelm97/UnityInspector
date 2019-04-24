using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityInspector.GUI.Models
{
   class EnumMember
    {
        private string _value;

        public ObjectMember objectMember;
        public string[] Values { get; }
        public string Value {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (objectMember != null)
                    objectMember.UpdateValue ();
            }
        }

        public EnumMember (string[] values, string value)
        {
            Values = values;
            _value = value;
        }
    }
}
