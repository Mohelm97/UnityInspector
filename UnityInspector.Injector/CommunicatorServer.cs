using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityInspector.Communicator;

namespace UnityInspectorInjector
{
    class CommunicatorServer : Communicator<GameObject>
    {
        public new static CommunicatorServer Instance { get; set; }
        private Dictionary<int, GameObject> refDict = new Dictionary<int, GameObject> ();

        public CommunicatorServer() : base (CommunicatorType.Server)
        {
            Instance = this;
        }
        public override void Close()
        {
            Instance = null;
            server.Stop();
            client.Close();
        }

        public override GameObject ReadGameObject ()
        {
            throw new NotImplementedException ();
        }
        public override GameObject ReadGameObjectByRef ()
        {
            int gameObjectRef = ReadInt ();
            return refDict[gameObjectRef];
        }

        public override void WriteGameObject (GameObject obj)
        {
            // The instance id of an object is always guaranteed to be unique.
            int gameObjectRef = obj.GetInstanceID ();
            if (!refDict.ContainsKey(gameObjectRef))
                refDict.Add (gameObjectRef, obj);

            WriteInt (gameObjectRef);
            WriteString (obj.name);
            WriteBool (obj.activeSelf);
        }
        public void WriteGameObject (Transform obj)
        {
            WriteGameObject (obj.gameObject);
        }
        public override void WriteGameObjectRef (GameObject obj)
        {
            WriteInt (obj.GetInstanceID ());
        }

        public override void WriteVector3 (object value)
        {
            Vector3 vector3 = (Vector3) value;
            WriteFloat (vector3.x);
            WriteFloat (vector3.y);
            WriteFloat (vector3.z);
        }
        public override object ReadVector3 ()
        {
            return new Vector3 (ReadFloat (), ReadFloat (), ReadFloat ());
        }

        public override void WriteVector2 (object value)
        {
            Vector2 vector2 = (Vector2) value;
            WriteFloat (vector2.x);
            WriteFloat (vector2.y);
        }
        public override object ReadVector2 ()
        {
            return new Vector2 (ReadFloat (), ReadFloat ());
        }

        public override void WriteRect (object value)
        {
            Rect rect = (Rect) value;
            WriteFloat (rect.x);
            WriteFloat (rect.y);
            WriteFloat (rect.width);
            WriteFloat (rect.height);
        }
        public override object ReadRect ()
        {
            return new Rect (ReadFloat (), ReadFloat (), ReadFloat (), ReadFloat ());
        }

        public override void WriteEnum (object value)
        {
            Array enumType = Enum.GetValues (value.GetType ());
            WriteByte ((byte) enumType.Length);
            for (int i = 0; i < enumType.Length; i++)
                WriteString (enumType.GetValue (i).ToString ());
            WriteString (value.ToString ());
        }
        public override object ReadEnum ()
        {
            return new EnumWrapper(ReadString ());
        }

        public override void WriteColor (object value)
        {
            Color color = (Color) value;
            WriteByte ((byte) (color.r * 255));
            WriteByte ((byte) (color.g * 255));
            WriteByte ((byte) (color.b * 255));
            WriteByte ((byte) (color.a * 255));
        }
        public override object ReadColor ()
        {
            return new Color (ReadByte() / 255f, ReadByte () / 255f, ReadByte () / 255f, ReadByte () / 255f);
        }

        public override object ReadArrayAsMember ()
        {
            throw new NotImplementedException ();
        }
    }
    public class EnumWrapper
    {
        readonly string stringValue;
        public EnumWrapper (string value)
        {
            this.stringValue = value;
        }
        public object GetEnumValue (MemberInfo member)
        {
            Type enumType;
            if (member.MemberType == MemberTypes.Field)
                enumType = ((FieldInfo) member).FieldType;
            else
                enumType = ((PropertyInfo) member).PropertyType;
            return Enum.Parse (enumType, stringValue);
        }
    }
}
