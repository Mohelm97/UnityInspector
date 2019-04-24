using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UnityInspector.Communicator
{
    public abstract class Communicator <T>
    {
        public static Communicator<T> Instance { get; set; }
        private const int Port = 23123;
        public enum Commands : byte
        {
            Destroy = 0,
            GetHierarchy = 1,
            GetChildren = 2,
            InvokeMethod = 3,
            GetComponents = 4,
            GetComponentType = 5,
            GetComponentMembers = 6,
            GetValueOfMember = 7,
            SetValueOfMember = 8
        }
        public enum Types : byte
        {
            Undefined = 0,
            Unknown = 1,
            Bool = 2,
            Byte = 3,
            Short = 4,
            Int = 5,
            Long = 6,
            Float = 7,
            Double = 8,
            String = 9,
            Array = 10,
            ArrayAsMember = 11,
            GameObject = 12,
            Vector3 = 14,
            Enum = 15,
            Color = 16,
            Vector2 = 17,
            Rect = 18,
        }
        public enum CommunicatorType
        {
            Server,
            Clinet
        }
        protected TcpListener server;
        protected TcpClient client;
        private NetworkStream pipe;
        private ASCIIEncoding stringEncoding;

        public Communicator (CommunicatorType communicatorType)
        {
            Instance = this;
            stringEncoding = new ASCIIEncoding ();
            try
            {
                if (communicatorType == CommunicatorType.Server)
                {
                    server = new TcpListener (IPAddress.Any, Port);
                    server.Start ();
                }
                else
                {
                    client = new TcpClient ("localhost", Port);
                    pipe = client.GetStream ();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex);
            }
        }
        public abstract void Close();
        public bool Connected { get { return client.Connected; } }
        public bool DataAvailable ()
        {
            if (client != null && client.Connected)
                return pipe.DataAvailable;
            
            if (server.Pending ())
            {
                client = server.AcceptTcpClient();
                pipe = client.GetStream();
            }
            return false;
        }

        public void WriteBool (bool value)
        {
            pipe.Write (BitConverter.GetBytes (value), 0, 1);
        }
        public bool ReadBool ()
        {
            byte[] inBuffer = new byte[1];
            pipe.Read (inBuffer, 0, 1);
            return BitConverter.ToBoolean (inBuffer, 0);
        }

        public void WriteByte (byte value)
        {
            pipe.WriteByte (value);
        }
        public byte ReadByte ()
        {
            return (byte) pipe.ReadByte ();
        }

        public void WriteShort (short value)
        {
            pipe.Write (BitConverter.GetBytes (value), 0, 2);
        }
        public short ReadShort ()
        {
            byte[] inBuffer = new byte[2];
            pipe.Read (inBuffer, 0, 2);
            return BitConverter.ToInt16 (inBuffer, 0);
        }

        public void WriteInt (int value)
        {
            pipe.Write (BitConverter.GetBytes (value), 0, 4);
        }
        public int ReadInt ()
        {
            byte[] inBuffer = new byte[4];
            pipe.Read (inBuffer, 0, 4);
            return BitConverter.ToInt32 (inBuffer, 0);
        }

        public void WriteLong (long value)
        {
            pipe.Write (BitConverter.GetBytes (value), 0, 8);
            pipe.Flush ();
        }
        public long ReadLong ()
        {
            byte[] inBuffer = new byte[8];
            pipe.Read (inBuffer, 0, 8);
            return BitConverter.ToInt64 (inBuffer, 0);
        }

        public void WriteFloat (float value)
        {
            pipe.Write (BitConverter.GetBytes (value), 0, 4);
            pipe.Flush ();
        }
        public float ReadFloat ()
        {
            byte[] inBuffer = new byte[4];
            pipe.Read (inBuffer, 0, 4);
            return BitConverter.ToSingle (inBuffer, 0);
        }

        public void WriteDouble (double value)
        {
            pipe.Write (BitConverter.GetBytes (value), 0, 8);
            pipe.Flush ();
        }
        public double ReadDouble ()
        {
            byte[] inBuffer = new byte[8];
            pipe.Read (inBuffer, 0, 8);
            return BitConverter.ToDouble (inBuffer, 0);
        }

        public void WriteString (string value)
        {
            if (value.Length > 255)
                Console.Error.WriteLine ("The length of the string must be lower than 256");
            byte len = (byte) value.Length;
            pipe.WriteByte (len);
            if (len != 0)
                pipe.Write (stringEncoding.GetBytes (value), 0, len);
        }
        public string ReadString ()
        {
            int len = pipe.ReadByte ();
            if (len == 0)
                return "";
            byte[] inBuffer = new byte[len];
            pipe.Read (inBuffer, 0, len);
            return stringEncoding.GetString (inBuffer);
        }

        public void WriteArrayAsMember (Array value)
        {
            if (value != null && value.Length > 0)
            {
                WriteInt (value.Length);
                WriteByte ((byte) GetObjectType (value.GetValue (0)));
            }
            else
            {
                WriteInt (0);
                WriteByte ((byte) Types.Undefined);
            }
        }
        public abstract object ReadArrayAsMember ();

        public void WriteArray (Array value)
        {
            if (value != null && value.Length > 0)
            {
                Types firstElementType = GetObjectType (value.GetValue (0));
                WriteInt (value.Length);
                WriteByte ((byte) firstElementType);
                for (int i = 0; i < value.Length; i++)
                {
                    WriteObject (value.GetValue (i), true, firstElementType);
                }
            }
            else
            {
                WriteInt (0);
            }
        }
        public Array ReadArray ()
        {
            int len = ReadInt ();
            Array values = new object[len];
            Types knownType = Types.Undefined;
            if (len > 0)
                knownType = (Types) ReadByte ();

            for (int i = 0; i < len; i++)
            {
                values.SetValue(ReadObject (knownType), i);
            }
            return values;
        }

        public void WriteObjects (object[] objects)
        {
            WriteInt (objects.Length);
            for (int i = 0; i < objects.Length; i++)
            {
                object obj = objects[i];
                WriteObject (obj);
            }
        }
        public object[] ReadObjects ()
        {
            int len = ReadInt ();
            object[] objects = new object[len];
            for (int i = 0; i < len; i++)
            {
                objects[i] = ReadObject ();
            }
            return objects;
        }

        public static Types GetObjectType (object obj, bool IsMember = false)
        {
            if (obj is bool)
                return Types.Bool;
            if (obj is byte)
                return Types.Byte;
            if (obj is short)
                return Types.Short;
            if (obj is int)
                return Types.Int;
            if (obj is long)
                return Types.Long;
            if (obj is float)
                return Types.Float;
            if (obj is double)
                return Types.Double;
            if (obj is string)
                return Types.String;
            if (obj is Array && IsMember)
                return Types.ArrayAsMember;
            if (obj is Array)
                return Types.Array;
            if (obj is T)
                return Types.GameObject;
            if (obj != null && obj.GetType ().Name == "Vector3")
                return Types.Vector3;
            if (obj is Enum || (obj != null && obj.GetType ().Name == "EnumMember"))
                return Types.Enum;
            if (obj != null && obj.GetType ().Name.StartsWith ("Color"))
                return Types.Color;
            if (obj != null && obj.GetType ().Name == "Vector2")
                return Types.Vector2;
            if (obj != null && obj.GetType ().Name == "Rect")
                return Types.Rect;
            return Types.Unknown;
        }
        public void WriteObject (object obj, bool IsMember = false, Types knownType = Types.Undefined)
        {
            Types objType = knownType;
            if (knownType == Types.Undefined)
            {
                objType = GetObjectType (obj, IsMember);
                WriteByte ((byte) objType);
            }
            switch (objType)
            {
                case Types.Bool:
                    WriteBool ((bool) obj);
                    break;
                case Types.Byte:
                    WriteByte ((byte) obj);
                    break;
                case Types.Short:
                    WriteShort ((short) obj);
                    break;
                case Types.Int:
                    WriteInt ((int) obj);
                    break;
                case Types.Long:
                    WriteLong ((long) obj);
                    break;
                case Types.Float:
                    WriteFloat ((float) obj);
                    break;
                case Types.Double:
                    WriteDouble ((double) obj);
                    break;
                case Types.String:
                    WriteString ((string) obj);
                    break;
                case Types.Array:
                    WriteArray ((Array) obj);
                    break;
                case Types.ArrayAsMember:
                    WriteArrayAsMember ((Array) obj);
                    break;
                case Types.GameObject:
                    WriteGameObject ((T) obj);
                    break;
                case Types.Vector3:
                    WriteVector3 (obj);
                    break;
                case Types.Enum:
                    WriteEnum (obj);
                    break;
                case Types.Color:
                    WriteColor (obj);
                    break;
                case Types.Vector2:
                    WriteVector2 (obj);
                    break;
                case Types.Rect:
                    WriteRect (obj);
                    break;
                case Types.Unknown:
                    WriteString ((obj==null)?"null":obj.GetType().Name);
                    break;

            }
        }
        public object ReadObject (Types knownType = Types.Undefined)
        {
            Types objType = knownType;
            if (knownType == Types.Undefined)
                objType = (Types) ReadByte ();

            switch (objType)
            {
                case Types.Bool:
                    return ReadBool ();
                case Types.Byte:
                    return ReadByte ();
                case Types.Short:
                    return ReadShort ();
                case Types.Int:
                    return ReadInt ();
                case Types.Long:
                    return ReadLong ();
                case Types.Float:
                    return ReadFloat ();
                case Types.Double:
                    return ReadDouble ();
                case Types.String:
                    return ReadString ();
                case Types.Array:
                    return ReadArray ();
                case Types.ArrayAsMember:
                    return ReadArrayAsMember ();
                case Types.GameObject:
                    return ReadGameObjectByRef ();
                case Types.Vector3:
                    return ReadVector3 ();
                case Types.Enum:
                    return ReadEnum ();
                case Types.Color:
                    return ReadColor ();
                case Types.Vector2:
                    return ReadVector2 ();
                case Types.Rect:
                    return ReadRect ();
                case Types.Unknown:
                    return ReadString ();
            }
            return null;
        }

        public void Flush ()
        {
            pipe.Flush ();
        }


        public abstract void WriteGameObject (T obj);
        public abstract T ReadGameObject ();
        public abstract void WriteGameObjectRef (T obj);
        public abstract T ReadGameObjectByRef ();

        public abstract void WriteVector3 (object value);
        public abstract object ReadVector3 ();

        public abstract void WriteVector2 (object value);
        public abstract object ReadVector2 ();

        public abstract void WriteRect (object value);
        public abstract object ReadRect ();

        public abstract void WriteEnum (object value);
        public abstract object ReadEnum ();

        public abstract void WriteColor (object value);
        public abstract object ReadColor ();

        
    }
}
