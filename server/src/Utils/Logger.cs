using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemDebug = System.Diagnostics.Debug;

namespace server.Utils
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

        public Logger(){
            _isInitialized = false;
            _logWriterQueue = new Queue<LogMessageHolder>();

            ResetColor();
            MakeDirs();

            _instance = this;
        }

        private void MakeDirs(){
            Directory.CreateDirectory(LogsPath);
        }

        public static bool IsInitialized => _instance._isInitialized;

        public void Initialize(){
            if (_isInitialized) {
                return;
            }

            _currentLogFileName = GenerateLogFileName();
            FileStream logFs;

            try {
                logFs = new FileStream($"{LogsPath}\\{_currentLogFileName}", FileMode.Append, FileAccess.Write);
            } catch (Exception e) {
                Error("Unnable to create log file", e);
                return;
            }


            SystemDebug.Assert(logFs != null);
            _logWriter = new StreamWriter(logFs);
            _logWriterQueueThread = new Thread(WriterQueueWorker){IsBackground = true};
            _logWriterQueueThread.Start();

            _isInitialized = true;
            _isRequestShutdown = false;
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
                lock (_logWriterQueue) {
                    while (_logWriterQueue.Count > 0) {
                        var entry = _logWriterQueue.Dequeue();
                        _logWriter.Write(FormatLogStr(entry.Time, entry.Message, entry.Level));
                        _logWriter.Flush();
                    }
                }

                Thread.Sleep(10);
            }

            _logWriter?.Close();
        }

        private static void AddToLogWriteQueue(string msg, string level){
            lock (_logWriterQueue) {
                _logWriterQueue.Enqueue(new LogMessageHolder(msg, level));
            }
        }

        private string GenerateLogFileName(){
            return $"{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.log";
        }

        internal static void SetNextColor(ConsoleColor clr){
            Console.ForegroundColor = clr;
        }

        internal static void ResetColor(){
            _lastPrintColor = Console.ForegroundColor;
        }

        internal static string FormatLogStr(long ticks, string msg, string level){
            var time = new DateTime(ticks);
            return $"{time.Hour}:{time.Minute}:{time.Second}.{time.Millisecond} ({level}) {msg}\r\n";
        }

        internal static string FormatPrintStr(string msg) {
            return $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} {msg}";
        }

        private static void AddLogMsg(string msg, string level, ConsoleColor clr){
            SystemDebug.Assert(IsInitialized);
            AddToLogWriteQueue($"({level}) {msg}", level);
            var str = FormatPrintStr($"({level}) {msg}");
            SetNextColor(clr);
            Console.WriteLine(str);
            ResetColor();
        }

        public static void Debug(string msg){
            AddLogMsg(msg, "Debug", ConsoleColor.Gray);
        }

        public static void Info(string msg) {
            AddLogMsg(msg, "Info", ConsoleColor.White);
        }
        public static void Warn(string msg) {
            AddLogMsg(msg, "Warn", ConsoleColor.Yellow);
        }

        public static void Error(string msg, Exception err = null){
            SystemDebug.Assert(IsInitialized);
            AddToLogWriteQueue($"(Error) {msg}", "Error");
            var str = err != null ? FormatPrintStr($"(Error) Exception ({err.GetType()}): '{err.Message}', {msg}\n{err.StackTrace}") : FormatPrintStr($"(Error) {msg}");
            SetNextColor(ConsoleColor.Red);
            Console.WriteLine(str);
            ResetColor();
        }

        private struct LogMessageHolder
        {
            public string Message;
            public long Time;
            public string Level;

            public LogMessageHolder(string msg, string level){
                Level = level;
                Message = msg;
                Time = DateTime.Now.Ticks;
            }
        }
    }
}
