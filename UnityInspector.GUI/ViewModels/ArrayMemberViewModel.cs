using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityInspector.GUI.Models;

namespace UnityInspector.GUI.ViewModels
{
    public class ArrayMemberViewModel : ViewModel
    {
        public ArrayMember arrayMember;
        public int Length { get { return arrayMember.Length; } }
        public CommunicatorClient.Types BaseType { get { return arrayMember.BaseType; } }
        public bool IsExpanded { get; set; }
        public string StringType { get { return arrayMember.BaseType + "[" + arrayMember.Length + "]"; } }
        private ObservableCollection<ObjectMemberViewModel> _values;

        public object Values
        {
            get
            {
                UpdateValues ();
                return _values;
            }
        }

        public ArrayMemberViewModel (ArrayMember arrayMember)
        {
            this.arrayMember = arrayMember;
        }
        public void Update (ArrayMember arrayMember)
        {
            this.arrayMember = arrayMember;
            RaisePropertyChanged("StringType");
            if (IsExpanded)
                RaisePropertyChanged("Values");
        }
        private void UpdateValues ()
        {
            ObjectMemberViewModel[] values = Array.ConvertAll (arrayMember.Values, e => new ObjectMemberViewModel (e));
            if (_values == null)
            {
                _values = new ObservableCollection<ObjectMemberViewModel> (values);
            }
            else
            {
                Utils.SyncObservableCollection (_values, values);
                for (int i = 0; i < _values.Count; i++)
                    _values[i].Update (values[i].objectMember);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ArrayMemberViewModel)
                return (arrayMember.Equals(((ArrayMemberViewModel)obj).arrayMember));

            return false;
        }
        public override int GetHashCode()
        {
            return arrayMember.GetHashCode();
        }
    }
}
