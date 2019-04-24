using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UnityInspector.Communicator;

namespace UnityInspector.GUI.Models
{
    class CommunicatorClient : Communicator<GameObject>
    {
        public new static CommunicatorClient Instance { get; set; }
        public CommunicatorClient() : base (CommunicatorType.Clinet)
        {
            Instance = this;
        }
        public override void Close()
        {
            WriteByte((byte)Commands.Destroy);
            Flush ();
            client.Close();
            Instance = null;
        }

        public GameObject[] ReadHierarchy ()
        {
            WriteByte ((byte) Commands.GetHierarchy);
            int len = ReadInt ();
            GameObject[] gameObjects = new GameObject[len];
            for (int i = 0; i < len; i++)
            {
                gameObjects[i] = ReadGameObject ();
            }
            return gameObjects;
        }
        public GameObject[] GetChildren (GameObject gameObject)
        {
            WriteByte ((byte) Commands.GetChildren);
            WriteGameObjectRef (gameObject);
            int len = ReadInt ();
            GameObject[] gameObjects = new GameObject[len];
            for (int i = 0; i < len; i++)
            {
                gameObjects[i] = ReadGameObject ();
            }
            return gameObjects;
        }
        public void InvokeMethod (GameObject gameObject, string method, object[] args)
        {
            WriteByte ((byte) Commands.InvokeMethod);
            WriteGameObjectRef (gameObject);
            WriteString (method);
            WriteObjects (args);
        }
        public Component[] GetComponents (GameObject gameObject)
        {
            WriteByte ((byte) Commands.GetComponents);
            WriteGameObjectRef (gameObject);
            int len = ReadInt ();
            Component[] components = new Component[len];
            for (int i = 0; i < len; i++)
            {
                components[i] = new Component (gameObject, i, ReadString (), ReadByte ());
            }
            return components;
        }
        public ObjectMember[] GetComponentMembers (Component component)
        {
            WriteByte ((byte) Commands.GetComponentMembers);
            WriteGameObjectRef (component.Owner);
            WriteInt (component.Index);
            int len = ReadInt ();
            ObjectMember[] members = new ObjectMember[len];
            for (int i = 0; i < len; i++)
            {
                Types type = (Types)ReadByte();
                string name = ReadString();
                members[i] = new ObjectMember (component,
                    type,
                    name,
                    ReadObject ());
            }
            return members;
        }
        public object GetValueOfMember (GameObject gameObject, string member, object value)
        {
            WriteByte ((byte) Commands.GetValueOfMember);
            WriteGameObjectRef (gameObject);
            WriteString (member);
            return ReadObject ();
        }
        public void SetValueOfMember (GameObject gameObject, string member, object value)
        {
            WriteByte ((byte) Commands.SetValueOfMember);
            WriteGameObjectRef (gameObject);
            WriteString (member);
            WriteObject (value);
        }

        public override object ReadArrayAsMember ()
        {
            ArrayMember ret =  new ArrayMember (ReadInt (), (Types) ReadByte ());
            return ret;
        }

        public override void WriteGameObject (GameObject obj)
        {
            throw new NotImplementedException ();
        }
        public override GameObject ReadGameObject ()
        {
            return new GameObject (ReadInt (), ReadString (), ReadBool ());
        }
        public override void WriteGameObjectRef (GameObject obj)
        {
            WriteInt (obj.Address);
        }
        public override GameObject ReadGameObjectByRef ()
        {
            return ReadGameObject ();
        }

        public override void WriteVector3 (object value)
        {
            Vector3 vector3 = (Vector3) value;
            WriteFloat (vector3.X);
            WriteFloat (vector3.Y);
            WriteFloat (vector3.Z);
        }
        public override object ReadVector3 ()
        {
            return new Vector3 (ReadFloat (), ReadFloat (), ReadFloat ());
        }

        public override void WriteVector2 (object value)
        {
            Vector2 vector2 = (Vector2) value;
            WriteFloat (vector2.X);
            WriteFloat (vector2.Y);
        }
        public override object ReadVector2 ()
        {
            return new Vector2 (ReadFloat (), ReadFloat ());
        }

        public override void WriteRect(object value)
        {
            Rect rect = (Rect) value;
            WriteFloat (rect.X);
            WriteFloat (rect.Y);
            WriteFloat (rect.W);
            WriteFloat (rect.H);
        }
        public override object ReadRect ()
        {
            return new Rect (ReadFloat (), ReadFloat (), ReadFloat (), ReadFloat ());
        }

        public override void WriteEnum (object value)
        {
            WriteString (((EnumMember) value).Value);
        }
        public override object ReadEnum ()
        {
            byte len = ReadByte ();
            string[] values = new string[len];
            for (int i = 0; i < len; i++)
            {
                values[i] = ReadString ();
            }
            return new EnumMember (values, ReadString ());
        }

        public override void WriteColor (object value)
        {
            Color color = ((Color) value);
            WriteByte (color.R);
            WriteByte (color.G);
            WriteByte (color.B);
            WriteByte (color.A);
        }
        public override object ReadColor ()
        {
            Color color = new Color ();
            color.R = ReadByte ();
            color.G = ReadByte ();
            color.B = ReadByte ();
            color.A = ReadByte ();
            return color;
        }
    }
}
