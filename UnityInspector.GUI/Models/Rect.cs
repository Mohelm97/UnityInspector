using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityInspector.GUI.Models
{
    class Rect
    {
        public ObjectMember objectMember;
        private float _x, _y, _w, _h;
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
        public float W
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
                if (objectMember != null) objectMember.UpdateValue ();
            }
        }
        public float H
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
                if (objectMember != null) objectMember.UpdateValue ();
            }
        }


        public Rect (float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
