using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace aydocs.NotchWin.Utils
{
    internal class HardwareMonitor
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        private Timer timer;

        public static string usageString = " ";

        public static HardwareMonitor instance;

        private Computer computer;
        private float lastCpu = 0;
        private string lastRam = "";

        private readonly object _lock = new object();

        private IHardware? cpuHardware;
        private ISensor? cpuLoadSensor;

        private const int IntervalMs = 1500;

        public HardwareMonitor()
        {
            instance = this;

            computer = new Computer()
            {
                IsMemoryEnabled = true,
                IsCpuEnabled = true
            };

            try
            {
                computer.Open();
                InitializeSensors();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("HardwareMonitor: error opening Computer: " + ex);
#endif
            }

            timer = new Timer(TimerCallback, null, IntervalMs, IntervalMs);
        }

        private void InitializeSensors()
        {
            try
            {
                foreach (var hw in computer.Hardware)
                {
                    if (hw == null) continue;

                    if (hw.HardwareType == HardwareType.Cpu && cpuHardware == null)
                    {
                        cpuHardware = hw;

                        foreach (var s in hw.Sensors)
                        {
                            if (s.SensorType == SensorType.Load && s.Name == "CPU Total")
                            {
                                cpuLoadSensor = s;
                                break;
                            }
                        }
                    }

                    if (hw.SubHardware != null && hw.SubHardware.Length > 0)
                    {
                        foreach (var sub in hw.SubHardware)
                        {
                            if (sub == null) continue;

                            if (sub.HardwareType == HardwareType.Cpu && cpuHardware == null)
                            {
                                cpuHardware = sub;
                                foreach (var s in sub.Sensors)
                                {
                                    if (s.SensorType == SensorType.Load && s.Name == "CPU Total")
                                    {
                                        cpuLoadSensor = s;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (cpuHardware != null && cpuLoadSensor != null)
                        break;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("HardwareMonitor: InitializeSensors exception: " + ex);
#endif
            }
        }

        private void TimerCallback(object? state)
        {
            if (!Monitor.TryEnter(_lock))
            {
#if DEBUG
                Debug.WriteLine("HardwareMonitor SKIPPED due to reentrant call");
#endif
                return;
            }

            try
            {
#if DEBUG
                Debug.WriteLine($"HardwareMonitor BEGIN");
#endif
                if (cpuHardware == null || cpuLoadSensor == null)
                {
                    InitializeSensors();
                }

                if (cpuHardware != null)
                {
                    try
                    {
                        cpuHardware.Update();
                        if (cpuLoadSensor != null && cpuLoadSensor.Value.HasValue)
                        {
                            lastCpu = Mathf.LimitDecimalPoints((float)cpuLoadSensor.Value.GetValueOrDefault(), 1);
                        }
                    }
                    catch { /* tolerate sensor update failures */ }
                }

                try
                {
                    MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
                    if (GlobalMemoryStatusEx(memStatus))
                    {
                        double totalPhysGB = memStatus.ullTotalPhys / (1024.0 * 1024.0 * 1024.0);
                        double availPhysGB = memStatus.ullAvailPhys / (1024.0 * 1024.0 * 1024.0);
                        double usedPhysGB = totalPhysGB - availPhysGB;

                        lastRam = Mathf.LimitDecimalPoints((float)usedPhysGB, 1) + "GB / " + Mathf.LimitDecimalPoints((float)totalPhysGB, 0) + "GB";
                    }
                }
                catch { }

                usageString = $"CPU: {lastCpu}%    RAM: {lastRam}";

#if DEBUG
                Debug.WriteLine($"HardwareMonitor END");
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine($"HardwareMonitor EXCEPTION");
#endif
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        public static void Stop()
        {
            try
            {
                if (instance != null)
                {
                    // stop and dispose timer
                    try
                    {
                        instance.timer?.Change(Timeout.Infinite, Timeout.Infinite);
                        instance.timer?.Dispose();
                    }
                    catch { }

                    if (instance.computer != null)
                    {
                        try
                        {
                            instance.computer.Close();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
