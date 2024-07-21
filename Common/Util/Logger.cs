using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.Util
{
    public class Logger
    {
        private readonly string ModuleName;
        private static FileInfo? LogFile;
        private static object _lock = new();

        public Logger(string moduleName)
        {
            ModuleName = moduleName;
        }

        public void Log(string message, LoggerLevel level, params object[] args)
        {
            lock (_lock)
            {
                // 格式化日志消息
                string formattedMessage;
                try
                {
                    formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
                }
                catch (FormatException ex)
                {
                    formattedMessage = $"[ERROR] Format exception: {ex.Message}";
                }

                // 输出到控制台
                Console.Write("[");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(DateTime.Now.ToString("HH:mm:ss"));
                Console.ResetColor();

                Console.Write("] ");
                Console.Write("[");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(ModuleName);
                Console.ResetColor();

                Console.Write("] ");
                Console.Write("[");

                Console.ForegroundColor = (ConsoleColor)level;
                Console.Write(level);
                Console.ResetColor();

                Console.WriteLine("] " + formattedMessage);

                // 写入到日志文件
                var logMessage = $"[{DateTime.Now:HH:mm:ss}] [{ModuleName}] [{level}] {formattedMessage}";
                WriteToFile(logMessage);
            }
        }

        public void Info(string message, params object[] args)
        {
            Log(message, LoggerLevel.INFO, args ?? new object[] { });
        }

        public void Warn(string message, params object[] args)
        {
            Log(message, LoggerLevel.WARN, args ?? new object[] { });
        }

        public void Error(string message, params object[] args)
        {
            Log(message, LoggerLevel.ERROR, args ?? new object[] { });
        }

        public void Fatal(string message, params object[] args)
        {
            Log(message, LoggerLevel.FATAL, args ?? new object[] { });
        }

        public void Debug(string message, params object[] args)
        {
            Log(message, LoggerLevel.DEBUG, args ?? new object[] { });
        }

        public static void SetLogFile(FileInfo file)
        {
            LogFile = file;
        }

        public static void WriteToFile(string message)
        {
            try
            {
                if (LogFile == null)
                {
                    throw new Exception("LogFile is not set");
                }
                using StreamWriter sw = LogFile.AppendText();
                sw.WriteLine(message);
            }
            catch
            {
                // Handle or log exception as necessary
            }
        }

#pragma warning disable CS8602 
        public static Logger GetByClassName() => new(new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name);
#pragma warning restore CS8602
    }

    public enum LoggerLevel
    {
        INFO = ConsoleColor.Cyan,
        WARN = ConsoleColor.Yellow,
        ERROR = ConsoleColor.Red,
        FATAL = ConsoleColor.DarkRed,
        DEBUG = ConsoleColor.Blue
    }

    public class LoggerLevelHelper
    {

    }
}
