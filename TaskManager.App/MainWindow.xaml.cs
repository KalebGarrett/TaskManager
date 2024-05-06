using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace Name.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private PerformanceCounter cpuCounter;
    private PerformanceCounter ramCounter;
    private PerformanceCounter diskCounter;

    public MainWindow()
    {
        InitializeComponent();

        cpuCounter = new PerformanceCounter("Processor", "% Idle Time", "_Total");
        ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        diskCounter = new PerformanceCounter("PhysicalDisk", "Disk Bytes/sec", "_Total");
        var timer = new Timer(ShowPcStats, null, 0, 1000);
    }

    private void ShowPcStats(object test)
    {
        Dispatcher.Invoke(() =>
        {
            CpuText.Text = $"{CpuUsageDifference(cpuCounter.NextValue())}%";

            var memoryInfo = GC.GetGCMemoryInfo();
            var installedMemory = memoryInfo.TotalAvailableMemoryBytes;
            var availableMemory = ramCounter.NextValue();
            var usedMemoryPercentage = (ConvertByteToGb(installedMemory) - ConvertMbToGb(availableMemory)) / ConvertByteToGb(installedMemory) * 100;
            RamText.Text = $"{Math.Round(usedMemoryPercentage)}%";

            DiskText.Text = $"{diskCounter.NextValue()} Bytes";
            PcText.Text = Environment.MachineName;

            var osVersion = Environment.OSVersion;
            OsText.Text = osVersion.Version.ToString();

            var bitType = Environment.Is64BitOperatingSystem;

            if (bitType)
            {
                BitText.Text = "64-bit";
            }
            else
            {
                BitText.Text = "32-bit";
            }
        });
    }

    private double CpuUsageDifference(float counter)
    {
        return 100 - Math.Round(counter);
    }

    private float ConvertMbToGb(float data)
    {
        var convertedCounter = data / 1000;
        return (float) Math.Round(convertedCounter);
    }

    private long ConvertByteToGb(long data)
    {
        return data / 1000000000;
    }
}