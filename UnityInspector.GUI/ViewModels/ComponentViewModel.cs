using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnityInspector.GUI.Models;
using Component = UnityInspector.GUI.Models.Component;

namespace UnityInspector.GUI.ViewModels
{
    public class ComponentViewModel : ViewModel
    {
        public Component component;
        public string Name { get { return component.Name; } }
        public bool IsBehaviour { get { return component.IsBehaviour; } }
        public Visibility Visibility { get { return IsBehaviour ? Visibility.Visible : Visibility.Hidden; } }
        public bool Enabled { get { return component.Enabled; } set { component.Enabled = value; } }
        public bool IsSelected { get; set; }

        private ObservableCollection<ObjectMemberViewModel> _members;
        public ObservableCollection <ObjectMemberViewModel> Members {
            get
            {
                UpdateMembers ();
                return _members;
            }
        }

        public ComponentViewModel (Component component)
        {
            this.component = component;
        }

        public void Update (Component newComponent)
        {
            component = newComponent;
            RaisePropertyChanged ("Enabled");
            RaisePropertyChanged ("Members");
        }

        private void UpdateMembers ()
        {
            ObjectMemberViewModel[] members = Array.ConvertAll (component.Members, e => new ObjectMemberViewModel (e));
            if (_members == null)
            {
                _members = new ObservableCollection<ObjectMemberViewModel> (members);
            }
            else
            {
                Utils.SyncObservableCollection (_members, members);
                for (int i = 0; i < _members.Count; i++)
                    _members[i].Update (members[i].objectMember);
            }
        }

        public override bool Equals (object obj)
        {
            if (obj != null && obj is ComponentViewModel)
                return (component.Equals (((ComponentViewModel) obj).component));

            return false;
        }
        public override int GetHashCode ()
        {
            return component.GetHashCode ();
        }
    }
}
