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
    public class GameObjectViewModel : ViewModel
    {
        public GameObject gameObject;
        public string Name { get { return gameObject.Name; } }
        public bool Active { get { return gameObject.Active; }
            set
            {
                gameObject.Active = value;
                RaisePropertyChanged ("Active");
            }
        }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        private ObservableCollection<GameObjectViewModel> _children;
        public ObservableCollection<GameObjectViewModel> Children
        {
            get
            {
                UpdateChildren ();
                return _children;
            }
        }

        private ObservableCollection<ComponentViewModel> _components;
        public ObservableCollection<ComponentViewModel> Components
        {
            get
            {
                UpdateComponents ();
                return _components;
            }
        }

        public GameObjectViewModel (GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        void UpdateChildren ()
        {
            GameObjectViewModel[] children = Array.ConvertAll (gameObject.Children, e => new GameObjectViewModel (e));
            if (_children == null)
            {
                _children = new ObservableCollection<GameObjectViewModel> (children);
            }
            else
            {
                Utils.SyncObservableCollection (_children, children);
                for (int i = 0; i < _children.Count; i++)
                    _children[i].Update (children[i].gameObject, IsExpanded);
            }
        }
        void UpdateComponents ()
        {
            ComponentViewModel[] components = Array.ConvertAll (gameObject.Components, e => new ComponentViewModel (e));
            if (_components == null)
            {
                _components = new ObservableCollection<ComponentViewModel> (components);
            }
            else
            {
                Utils.SyncObservableCollection (_components, components);
                for (int i = 0; i < _components.Count; i++)
                    _components[i].Update (components[i].component);
            }
        }

        public void Update (GameObject newGameObject, bool parentIsExpanded)
        {
            if (newGameObject != null)
                gameObject = newGameObject;
            RaisePropertyChanged ("Name");
            RaisePropertyChanged ("Active");
            if (parentIsExpanded)
                RaisePropertyChanged ("Children");
             RaisePropertyChanged ("Components");
        }
        public void Update ()
        {
            Update (null, true);
        }

        public override bool Equals (object obj)
        {
            if (obj != null && obj is GameObjectViewModel)
                return (gameObject.Equals(((GameObjectViewModel) obj).gameObject));

            return false;
        }
        public override int GetHashCode ()
        {
            return gameObject.GetHashCode ();
        }
    }
}
