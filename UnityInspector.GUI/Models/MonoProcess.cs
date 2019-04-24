using System;

namespace UnityInspector.GUI.ViewModels
{
    public class MonoProcess
    {
        public IntPtr MonoModule { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}