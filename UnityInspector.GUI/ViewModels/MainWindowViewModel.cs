using SharpMonoInjector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UnityInspector.GUI.Models;

namespace UnityInspector.GUI.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        private MonoProcess _selectedProcess;
        CommunicatorClient communicator;

        public ICollection<MonoProcess> Processes { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand InjectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand RefreshTreeCommand { get; set; }
        private int _refreshRate;
        public int RefreshRate {
            get {
                return _refreshRate;
            }
            set {
                _refreshRate = value;
                if (refreshTimer == null && _refreshRate != 0)
                {
                    refreshTimer = new DispatcherTimer (new TimeSpan (10000000 / _refreshRate), DispatcherPriority.Background, delegate
                    {
                        RefreshTree (null);
                    }, Application.Current.MainWindow.Dispatcher);
                }
                else if (_refreshRate != 0)
                {
                    refreshTimer.Interval = new TimeSpan (10000000 / _refreshRate);
                    if (!refreshTimer.IsEnabled) refreshTimer.Start ();
                }
                else if (refreshTimer != null && _refreshRate == 0)
                {
                    refreshTimer.Stop ();
                }
            }
        }
        private DispatcherTimer refreshTimer;
        public GameObjectViewModel MainSceneGameObject { get; set; }

        private string _status;
        public string Status { get { return _status; } set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }
        public bool IsConnected { get { return CommunicatorClient.Instance != null && CommunicatorClient.Instance.Connected; } }
        public MonoProcess SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                RaisePropertyChanged ("SelectedProcess");
            }
        }

        public MainWindowViewModel () {
            RefreshCommand = new RelayCommand (GetProcesses);
            InjectCommand = new RelayCommand (Inject);
            DisconnectCommand = new RelayCommand (Disconnect);
            RefreshTreeCommand = new RelayCommand (RefreshTree);
            MainSceneGameObject = new GameObjectViewModel (new GameObject (0, "MainScene", true));
            MainSceneGameObject.IsExpanded = true;
            Status = "Idle";
        }
        private void RefreshTree (object parameter)
        {
            MainSceneGameObject.Update (null, true);
        }
        private async void Inject (object parameter)
        {
            Status = "Injecting";
            await Task.Run (() =>
            {
                Status = "Getting access";
                IntPtr handle = Native.OpenProcess (ProcessAccessRights.PROCESS_ALL_ACCESS, false, SelectedProcess.Id);
                string AssemblyPath = @"UnityInspector.Injector.dll";
                if (handle == IntPtr.Zero)
                {
                    Status = "Access denied";
                    return;
                }

                byte[] file;

                try
                {
                    Status = "Reading UnityInspector.Injector.dll";
                    file = File.ReadAllBytes (AssemblyPath);
                }
                catch (IOException)
                {
                    Status = "Can't read UnityInspector.Injector.dll";
                    return;
                }

                using (Injector injector = new Injector (handle, SelectedProcess.MonoModule))
                {
                    try
                    {
                        Status = "Injecting";
                        IntPtr asm = injector.Inject (file, "UnityInspectorInjector", "Injector", "OnLoad");
                    }
                    catch (InjectorException ie)
                    {
                        Status = ie.Message;
                        return;
                    }
                    catch (Exception e)
                    {
                        Status = e.Message;
                        return;
                    }
                }
                StartCommunicator ();
            });
        }

        private void StartCommunicator ()
        {
            Status = "Starting Communicator";
            communicator = new CommunicatorClient();
            Status = "Connected";
            RaisePropertyChanged ("IsConnected");
            MainSceneGameObject.Update ();
        }
        private void Disconnect (object parameter)
        {
            RefreshRate = 0;
            if (communicator != null)
                communicator.Close ();
            RaisePropertyChanged("IsConnected");
            RefreshTree (null);
            Status = "Idel";
        }
        private async void GetProcesses (object parameter)
        {
            Collection<MonoProcess> processes = new Collection<MonoProcess> ();
            string prevStatus = Status;
            await Task.Run (() =>
            {
                Status = "Getting Processes";
                int cp = Process.GetCurrentProcess ().Id;
                foreach (Process p in Process.GetProcesses ())
                {
                    if (p.Id == cp || p.MainWindowHandle == IntPtr.Zero)
                        continue;
                    const ProcessAccessRights flags = ProcessAccessRights.PROCESS_QUERY_INFORMATION | ProcessAccessRights.PROCESS_VM_READ;
                    IntPtr handle;

                    if ((handle = Native.OpenProcess (flags, false, p.Id)) != IntPtr.Zero)
                    {
                        if (ProcessUtils.GetMonoModule (handle, out IntPtr mono))
                        {
                            processes.Add (new MonoProcess
                            {
                                MonoModule = mono,
                                Id = p.Id,
                                Name = p.ProcessName
                            });
                        }

                        Native.CloseHandle (handle);
                    }
                }
            });
            Processes = processes;
            RaisePropertyChanged ("Processes");
            Status = prevStatus;
            if (Processes.Count > 0)
            {
                SelectedProcess = Processes.First();
            }
                
        }

    }
}
