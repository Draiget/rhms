using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using server.Drivers.Kernel;
using SystemDebug = System.Diagnostics.Debug;

namespace server.Utils.Logging
{
    public class Logger
    {
        private const string LogsPath = "logs";
        private static Logger _instance;

        private bool _isInitialized;
        private bool _isRequestShutdown;
        private StreamWriter _logWriter;
        private static ConsoleColor _lastPrintColor;
        private string _currentLogFileName;
        private Thread _logWriterQueueThread;

        private static volatile Queue<LogMessageHolder> _logWriterQueue;

        public static bool IsInitialized => _instance._isInitialized;

        public Logger(){
            _isInitialized = false;
            _logWriterQueue = new Queue<LogMessageHolder>();

            _lastPrintColor = ConsoleColor.White;
            MakeDirs();

            _instance = this;
        }

        private static void MakeDirs(){
            Directory.CreateDirectory(LogsPath);
        }

        public void Initialize(){
            if (_isInitialized) {
                return;
            }

            _currentLogFileName = GenerateLogFileName();
            OpenFileStream();

            _logWriterQueueThread = new Thread(WriterQueueWorker) {
                IsBackground = true,
                Name ="logger"
            };
            _logWriterQueueThread.Start();

            _isInitialized = true;
            _isRequestShutdown = false;
        }

        private void OpenFileStream(){
            FileStream logFs;

            try {
                logFs = new FileStream($"{LogsPath}\\{_currentLogFileName}", FileMode.Append, FileAccess.Write);
            } catch (Exception e) {
                Error("Unnable to create log file", e);
                return;
            }

            _logWriter = new StreamWriter(logFs);
            SystemDebug.Assert(logFs != null);
        }

        private void CloseFileStream(){
            if (_logWriter != null) {
                _logWriter.Flush();
                _logWriter.Close();
                _logWriter.Dispose();
                _logWriter = null;
            }
        }

        public void Shutdown(){
            if (!_isInitialized) {
                return;
            }

            _isRequestShutdown = true;
            _logWriterQueueThread.Join(2000);
        }

        private void WriterQueueWorker(){
            while (!_isRequestShutdown) {
                // Reopen file stream, if date was changed and we need write to another file
                // TODO: Remake string comparing solution to date checking (will be much faster)
                if (GenerateLogFileName() != _currentLogFileName) {
                    CloseFileStream();
                    OpenFileStream();
                }

                lock (_logWriterQueue) {
                    while (_logWriterQueue.Count > 0) {
                        var entry = _logWriterQueue.Dequeue();
                        _logWriter.Write(FormatLogStr(entry.Time, entry.Message, entry.Level, entry.ContextThread));
                        _logWriter.Flush();
                    }
                }

                Thread.Sleep(10);
            }

            _logWriter?.Close();
        }

        private static void AddToLogWriteQueue(string msg, LogLevel level, Thread contextThread){
            lock (_logWriterQueue) {
                _logWriterQueue.Enqueue(new LogMessageHolder(msg, level, contextThread));
            }
        }

        private static string GenerateLogFileName(){
            return $"{DateTime.Now.Day:00}_{DateTime.Now.Month:00}_{DateTime.Now.Year}.log";
        }

        internal static void SetNextColor(ConsoleColor clr){
            _lastPrintColor = Console.ForegroundColor;
            Console.ForegroundColor = clr;
        }

        internal static void ResetColor(){
            Console.ForegroundColor = _lastPrintColor;
        }

        internal static string FormatLogStr(long ticks, string msg, LogLevel level, Thread ctx){
            var time = new DateTime(ticks);
            return $"{time.Hour:00}:{time.Minute:00}:{time.Second:00}.{time.Millisecond:000} " +
                   $"[{ctx.Name ?? "<?>"}-{ctx.ManagedThreadId}] ({level}) {msg}\r\n";
        }

        internal static string FormatPrintStr(LogLevel level, string msg) {
            return $"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}:{DateTime.Now.Second:00} " +
                   $"[{Thread.CurrentThread.Name ?? "<?>"}-{Thread.CurrentThread.ManagedThreadId}] ({level}) {msg}";
        }

        private static void AddLogMsg(string msg, LogLevel level, ConsoleColor clr){
            SystemDebug.Assert(IsInitialized, "Logger not initialized");
            AddToLogWriteQueue($"{msg}", level, Thread.CurrentThread);
            var str = FormatPrintStr(level, $"{msg}");
            SetNextColor(clr);
            Console.WriteLine(str);
            ResetColor();
        }

        public static void Debug(string msg){
            AddLogMsg(msg, LogLevel.Debug, ConsoleColor.Gray);
        }

        public static void Info(string msg) {
            AddLogMsg(msg, LogLevel.Info, ConsoleColor.White);
        }

        public static void Warn(string msg) {
            AddLogMsg(msg, LogLevel.Warn, ConsoleColor.Yellow);
        }

        public static void Error(string msg, Exception err = null){
            SystemDebug.Assert(IsInitialized, "Logger not initialized");
            AddToLogWriteQueue(err != null ? $"Exception details ({err.GetType()}): '{err.Message}'\n{err.StackTrace}" : $"{msg}", LogLevel.Error, Thread.CurrentThread);
            var str = err != null ? FormatPrintStr(LogLevel.Error, $"Exception details ({err.GetType()}): '{err.Message}'\n{err.StackTrace}") : FormatPrintStr(LogLevel.Error, $"{msg}");
            SetNextColor(ConsoleColor.Red);
            Console.WriteLine(FormatPrintStr(LogLevel.Error, $"{msg}"));
            Console.WriteLine(str);
            ResetColor();
        }

        public static void Auto(DriverLogLevel level, string message){
            LogLevel logLevel;
            ConsoleColor logColor;

            switch (level) {
                case DriverLogLevel.Info:
                    logLevel = LogLevel.Info;
                    logColor = ConsoleColor.White;
                    break;
                case DriverLogLevel.Warning:
                    logLevel = LogLevel.Warn;
                    logColor = ConsoleColor.Yellow;
                    break;
                case DriverLogLevel.Error:
                    logLevel = LogLevel.Error;
                    logColor = ConsoleColor.Red;
                    break;
                default:
                    logLevel = LogLevel.Debug;
                    logColor = ConsoleColor.Gray;
                    break;
            }

            AddLogMsg(message, logLevel, logColor);
        }
    }
}
