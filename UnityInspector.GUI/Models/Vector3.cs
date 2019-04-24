using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityInspector.GUI.Models
{
    class Vector3
    {
        public ObjectMember objectMember;
        private float _x, _y, _z;
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
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
                if (objectMember != null) objectMember.UpdateValue ();
            }
        }


        public Vector3 (float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
