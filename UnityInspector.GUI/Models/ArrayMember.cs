using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityInspector.Communicator;

namespace UnityInspector.GUI.Models
{
    public class ArrayMember
    {
        public ObjectMember objectMember;
        public int Length { get; }
        public CommunicatorClient.Types BaseType { get; }
        public ObjectMember[] Values
        {
            get
            {
                object[] values = (object[]) (Array) objectMember.GetValue ();
                ObjectMember[] valuesAsMember = new ObjectMember[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    valuesAsMember[i] = new ObjectMember (objectMember, BaseType, "[" + i + "]", values[i]);
                }
                return valuesAsMember;
            }
        }

        public ArrayMember (int length, Communicator<GameObject>.Types baseType)
        {
            Length = length;
            BaseType = baseType;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ArrayMember)
                return objectMember.Equals(((ArrayMember)obj).objectMember);
            return false;
        }
        public override int GetHashCode()
        {
            return objectMember.GetHashCode();
        }
    }
}
