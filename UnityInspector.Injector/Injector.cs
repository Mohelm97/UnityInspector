using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections;
using Commands = UnityInspectorInjector.CommunicatorServer.Commands;
namespace UnityInspectorInjector
{
    public class Injector
    {
        public static GameObject test;
        public static void OnLoad ()
        {
            if (test != null)
                MonoBehaviour.Destroy(test);
            if (CommunicatorServer.Instance != null)
                CommunicatorServer.Instance.Close();
            test = new GameObject ();
            MonoBehaviour.DontDestroyOnLoad (test);
            
            test.AddComponent<InjectorComp> ();
            Application.runInBackground = true;
        }
    }
    public class InjectorComp : MonoBehaviour
    {
        CommunicatorServer communicator;
        void Start ()
        {
            communicator = new CommunicatorServer();
            StartCoroutine (Poll ());
        }
        IEnumerator Poll ()
        {
            while (true)
            {
                yield return null;
                int commandsExeced = 0;
                while (communicator.DataAvailable () && commandsExeced < 10)
                {
                    commandsExeced++;
                    Commands command = (Commands) communicator.ReadByte ();
                    ExecuteCommand (command);
                }

            }
        }

        void ExecuteCommand (Commands command)
        {
            if (command == Commands.Destroy)
            {
                communicator.Close();
                Destroy(gameObject);
            }
            else if (command == Commands.GetHierarchy)
            {
                // I don't use WriteArray in this method for speed prpouses
                GameObject[] gameObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();
                communicator.WriteInt (gameObjects.Length);
                for (int i = 0; i < gameObjects.Length; i++)
                    communicator.WriteGameObject (gameObjects[i]);
            }
            else if (command == Commands.GetChildren)
            {
                GameObject obj = communicator.ReadGameObjectByRef ();
                communicator.WriteInt (obj.transform.childCount);
                for (int i = 0; i < obj.transform.childCount; i++)
                    communicator.WriteGameObject (obj.transform.GetChild (i));
            }
            else if (command == Commands.InvokeMethod)
            {
                object obj = ReadNestedRef (out string methodName);

                MethodInfo methodInfo = obj.GetType ().GetMethod (methodName);
                methodInfo.Invoke (obj, communicator.ReadObjects ());
            }
            else if (command == Commands.GetComponents)
            {
                GameObject obj = communicator.ReadGameObjectByRef ();
                Component[] comps = obj.GetComponents<Component> ();
                communicator.WriteInt (comps.Length);
                for (int i = 0; i < comps.Length; i++)
                {
                    Type compType = comps[i].GetType ();
                    communicator.WriteString (compType.Name);
                    PropertyInfo enabledField = compType.GetProperty ("enabled");
                    if (enabledField != null)
                        communicator.WriteByte ((Byte) ((bool) enabledField.GetValue (comps[i], null) ? 1 : 2));
                    else
                        communicator.WriteByte (0);
                }
            }
            else if (command == Commands.GetComponentMembers)
            {
                GameObject obj = communicator.ReadGameObjectByRef ();
                int compIndex = communicator.ReadInt ();
                Component comp = obj.GetComponents<Component> ()[compIndex];
                Type compType = comp.GetType ();
                MemberInfo[] members = GetFilteredMembers (compType);
                communicator.WriteInt (members.Length);
                for (int i = 0; i < members.Length; i++)
                {
                    MemberInfo member = members[i];

                    object value;
                    if (member.MemberType == MemberTypes.Field)
                        value = ((FieldInfo) member).GetValue (comp);
                    else
                        value = ((PropertyInfo) member).GetValue (comp, null);

                    if (value is Component)
                        value = ((Component) value).gameObject;

                    communicator.WriteByte ((byte) CommunicatorServer.GetObjectType (value));
                    communicator.WriteString (member.Name);
                    communicator.WriteObject (value, true);

                }
            }
            else if (command == Commands.GetValueOfMember)
            {
                object obj = ReadNestedRef (out string memberName);

                MemberInfo member = obj.GetType ().GetMember (memberName)[0];
                object value;
                if (memberName[0] != '[')
                {
                    if (member.MemberType == MemberTypes.Field)
                        value = ((FieldInfo)member).GetValue(obj);
                    else
                        value = ((PropertyInfo)member).GetValue(obj, null);
                }
                else
                {
                    int index = int.Parse(memberName.Substring(1, memberName.Length - 2));
                    value = ((Array)obj).GetValue(index);
                }
                if (value is Component)
                    value = ((Component)value).gameObject;
                CommunicatorServer.Instance.WriteObject (value);
            }
            else if (command == Commands.SetValueOfMember)
            {
                object obj = ReadNestedRef (out string memberName);
                object value = communicator.ReadObject();
                //TODO: If value is gameobject but membertype is component we need to get the component of that gameobject

                if (memberName[0] != '[')
                {
                    MemberInfo member = obj.GetType().GetMember(memberName)[0];

                    if (value is EnumWrapper)
                        value = ((EnumWrapper)value).GetEnumValue(member);

                    if (value != null)
                    {
                        if (member.MemberType == MemberTypes.Field)
                            ((FieldInfo)member).SetValue(obj, value);
                        else
                            ((PropertyInfo)member).SetValue(obj, value, null);
                    }
                }
                else
                {
                    int index = int.Parse(memberName.Substring(1, memberName.Length - 2));
                    ((Array)obj).SetValue (value, index);
                }
                
            }
        }

        object ReadNestedRef (out string memberName)
        {
            object obj = communicator.ReadGameObjectByRef ();
            memberName = communicator.ReadString ();
            int colonLocation = memberName.IndexOf (':');
            if (colonLocation != -1)
            {
                int compIndex = int.Parse (memberName.Substring (0, colonLocation));
                obj = ((GameObject) obj).GetComponents<Component> ()[compIndex];
                memberName = memberName.Substring (colonLocation + 1);
            }
            string[] path = memberName.Split('.');
            for (int i=0; i < path.Length - 1; i++)
            {
                string point = path[i];
                if (point[0] != '[')
                {
                    MemberInfo member = obj.GetType().GetMember(path[i])[0];
                    if (member.MemberType == MemberTypes.Field)
                        obj = ((FieldInfo)member).GetValue (obj);
                    else
                        obj = ((PropertyInfo)member).GetValue(obj, null);
                } else
                {
                    //TODO
                }
            }
            memberName = path[path.Length - 1];
            return obj;
        }
        public MemberInfo[]  GetFilteredMembers (Type type)
        {
            return Array.FindAll (type.GetMembers (), m =>
            {
                if (m.DeclaringType != type)
                    return false;
                if (m.MemberType == MemberTypes.Property && !((PropertyInfo) m).CanWrite)
                    return false;
                if (m.MemberType != MemberTypes.Field && m.MemberType != MemberTypes.Property)
                    return false;
                return true;
            });
        }

        private void OnDestroy()
        {
            communicator.Close();
        }
    }
}
