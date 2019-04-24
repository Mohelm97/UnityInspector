using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityInspector.GUI.Models
{
    class Vector2
    {
        public ObjectMember objectMember;
        private float _x, _y;
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                if (objectMember != null) objectMember.UpdateValue ();
            }
        }
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                if (objectMember != null) objectMember.UpdateValue ();
            }
        }


        public Vector2 (float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
