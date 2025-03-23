using System;
using System.IO;
using Serilog;
using AWC.DigitalCommerce.TicketsController.Properties;
using System.Diagnostics;

namespace AWC.DigitalCommerce.TicketsController
{
    public class Logger
    {
        public enum Severity
        {
            VERBOSE,
            DEBUG,
            INFORMATION,
            WARNING,
            ERROR,
            FATAL
        }

        public static void WriteToLog(string caller, string msg, Severity lv)
        {
            try
            {
                string fullLogPath = Path.Combine(Settings.Default.SerilogRootPath, caller);

                if (!Directory.Exists(fullLogPath))
                    Directory.CreateDirectory(fullLogPath);

                string fullLogFileName = Path.Combine(fullLogPath, caller + "-.log");

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(fullLogFileName,
                                  rollingInterval: RollingInterval.Day,
                                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}")
                    .CreateLogger();

                switch (lv)
                {
                    case Severity.VERBOSE:
                        Log.Verbose(msg);
                        break;
                    case Severity.DEBUG:
                        Log.Debug(msg);
                        break;
                    case Severity.INFORMATION:
                        Log.Information(msg);
                        break;
                    case Severity.WARNING:
                        Log.Warning(msg);
                        break;
                    case Severity.ERROR:
                        Log.Error(msg);
                        break;
                    case Severity.FATAL:
                        Log.Fatal(msg);
                        break;
                }
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void WriteToLog(string caller, Exception ex, Severity lv)
        {
            try
            {
                string msg = ex.Message;

                if (ex.StackTrace != null)
                {
                    var stackTrace = new StackTrace(ex, true);
                    var frame = stackTrace.GetFrame(0);
                    var method = frame.GetMethod();
                    msg = $"{msg}. Method: {method}";
                }



                string fullLogPath = Path.Combine(Settings.Default.SerilogRootPath, caller);

                if (!Directory.Exists(fullLogPath))
                    Directory.CreateDirectory(fullLogPath);

                string fullLogFileName = Path.Combine(fullLogPath, caller + "-.log");

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(fullLogFileName,
                                  rollingInterval: RollingInterval.Day,
                                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}")
                    .CreateLogger();

                switch (lv)
                {
                    case Severity.VERBOSE:
                        Log.Verbose(msg);
                        break;
                    case Severity.DEBUG:
                        Log.Debug(msg);
                        break;
                    case Severity.INFORMATION:
                        Log.Information(msg);
                        break;
                    case Severity.WARNING:
                        Log.Warning(msg);
                        break;
                    case Severity.ERROR:
                        Log.Error(msg);
                        Helper.ShowMessage(msg, System.Windows.Forms.MessageBoxIcon.Error);
                        break;
                    case Severity.FATAL:
                        Log.Fatal(msg);
                        break;
                }
                Log.CloseAndFlush();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
