using System;
using ReflectionCli.Lib;

namespace ReflectionCli
{
    public class Exit : ICommand
    {
        private readonly ILoggingService _loggingService;

        public Exit(ILoggingService loggingService) 
        {
            _loggingService = loggingService;
        }

        public void Run(IParam param)
        {
            _loggingService.LogInfo("Shutting down....");

            Program.ShutDown = true;
        }
    }
}