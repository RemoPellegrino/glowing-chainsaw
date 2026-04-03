using FlaUI.Core;
using System.Diagnostics;

namespace FlaUI.PoC.Factories
{
    internal class AppFactory
    {
        public static AppHandle GetApplication(string location, string? processNameToOverride)
        {
            var app = Application.Launch(location);

            if (string.IsNullOrEmpty(processNameToOverride))
                return new AppHandle(app, Process.GetProcessById(app.ProcessId), true);

            Thread.Sleep(1000);

            var process = GetSingleProcessByName(processNameToOverride);

            var attachedApp = Application.Attach(process);

            return new AppHandle(attachedApp, process, false);
        }
        private static Process GetSingleProcessByName(string name)
        {
            var processes = Process.GetProcessesByName(name);
            if(processes == null || processes.Length == 0)
            {
                throw new ProcessNameNotFoundException(name);
            }
            if(processes.Length > 1)
            {
                throw new AmbiguousProcessNameException(name);
            }
            return processes[0];
        }

    }
    public abstract class CustomException(string message, Exception? innerException)
        : Exception(message, innerException)
    { }
    public class AmbiguousProcessNameException(string processName, Exception? innerException = null)
        : CustomException($"More than 1 processes named '{processName}' found", innerException)
    { }
    public class ProcessNameNotFoundException(string processName, Exception? innerException = null)
        : CustomException($"Process named '{processName}' not found", innerException)
    { }
    internal class AppHandle(Application app, Process process, bool ownsProcess)
    {
        public Application Application { get; } = app;
        public Process Process { get; } = process;
        public bool OwnsProcess { get; } = ownsProcess;

        public void Close()
        {
            Application.Close();
        }
    }
}
