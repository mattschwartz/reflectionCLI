﻿using System;
using ReflectionCli.Lib.Enums;

namespace ReflectionCli.Lib
{
    public class LoggingService : ILoggingService
    {
        public Verbosity Verbosity = Verbosity.Debug;
        private readonly ILoggingService _loggingService;

        private string _textToBeWrittenInConsole = string.Empty;
        private string _textReadInFromConsole = string.Empty;

        public Verbosity GetVerbosity()
        {
            return Verbosity;
        }

        public void LogDebug(string debug)
        {
            if (Verbosity >= Verbosity.Debug) {
                Log($"[DBG] {debug}");
            }
        }

        public void LogError(string error)
        {
            if (Verbosity >= Verbosity.Error) {
                Log($"[ERR] {error}");
            }
        }

        public void Log(string info)
        {
            _textToBeWrittenInConsole = info;
            Console.WriteLine(info);
        }

        public void Log(object info)
        {
            _textToBeWrittenInConsole = info.ToString();
            Console.WriteLine(info);
        }

        public void LogResult(string info)
        {
            Log(info);
        }
        public void LogResult(object info)
        {
            Log(info);
        }

        public void Log()
        {
            Log(Environment.NewLine);
        }

        public void LogInfo(string info)
        {
            if (Verbosity >= Verbosity.Info) {
                Log($"[INF] {info}");
            }
        }

        public void LogWarning(string warning)
        {
            if (Verbosity >= Verbosity.Warning) {
                Log($"[WRN] {warning}");
            }
        }

        public void LogException(Exception ex)
        {
            if (Verbosity >= Verbosity.Info)
            {
                Log();
                Log($"[INF] {ex.Message}");
            }

            if (Verbosity >= Verbosity.Debug)
            {
                Log();
                Log($"[DBG] {ex.ToString()}");
            }

            if (ex.InnerException !=  null)
            {
                Log();
                Log("Inner:");
                LogException(ex.InnerException);
            }
        }

        public string Internal(string info)
        {
            _textReadInFromConsole = info;
            return info;
        }

        public string ReadLineFromConsole()
        {
            return Internal(Console.ReadLine());
        }

        public void SetVerbosity(Verbosity verbosity)
        {
            Verbosity = verbosity;
        }

        public string GetLastTextWrittenInConsole()
        {
            return _textToBeWrittenInConsole;
        }
    }
}
