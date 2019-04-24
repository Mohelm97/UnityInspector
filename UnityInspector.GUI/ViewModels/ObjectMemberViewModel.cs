using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityInspector.GUI.Models;

namespace UnityInspector.GUI.ViewModels
{
    public class ObjectMemberViewModel : ViewModel
    {
        public ObjectMember objectMember;
        public CommunicatorClient.Types Type { get { return objectMember.Type; } }
        public string Name { get { return objectMember.Name; } }
        private object _value;
        public object Value { get { return _value; } set { objectMember.Value = value; _value = objectMember.Value; } }

        public ObjectMemberViewModel (ObjectMember objectMember)
        {
            this.objectMember = objectMember;
            _value = objectMember.Value;
            if (_value is ArrayMember arrayMember)
            {
                _value = new ArrayMemberViewModel (arrayMember);
            }
        }

        public void Update (ObjectMember newObjectMember)
        {
            objectMember = newObjectMember;
            if (_value is ArrayMemberViewModel valueViewModel)
            {
                valueViewModel.Update ((ArrayMember) objectMember.Value);
            }
            else
            {
                _value = objectMember.Value;
                RaisePropertyChanged("Value");
            }
        }

        public override bool Equals (object obj)
        {
            if (obj != null && obj is ObjectMemberViewModel)
                return (objectMember.Equals (((ObjectMemberViewModel) obj).objectMember));

            return false;
        }
        public override int GetHashCode ()
        {
            return objectMember.GetHashCode ();
        }
    }
}
