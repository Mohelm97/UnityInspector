using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityInspector.GUI
{
    class Utils
    {
        public static void SyncObservableCollection<T> (ObservableCollection <T> collection, T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int oldIndex = collection.IndexOf (array[i]);
                if (oldIndex == -1)
                {
                    collection.Insert (i, array[i]);
                }
                else
                {
                    T oldChild = collection[oldIndex];
                    if (oldIndex != i)
                        collection.Move (oldIndex, i);
                }
            }
            // If we move all object to the correct order, 
            // and add the new one in the correct position
            // then the deleted one will be after children.length
            int collectionCount = collection.Count;
            for (int i = array.Length; i < collectionCount; i++)
            {
                collection.RemoveAt (array.Length);
            }
        }
    }
}
