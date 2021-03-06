﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflectionCli.Lib;

namespace ReflectionCli
{
    public class ParserService : IParserService
    {
        private readonly IAssemblyService _assemblyservice;
        private readonly ILoggingService _loggingservice;

        public ParserService(IAssemblyService assemblyservice, ILoggingService loggingservice)
        {
            _assemblyservice = assemblyservice;
            _loggingservice = loggingservice;
        }

        public void Parse(string commandString)
        {
            try
            {
                string asmName;
                string commandName;

                if (string.IsNullOrEmpty(commandString))
                {
                    throw new Exception($"Please Enter the name of a command");
                }

                var commandtypes = new List<TypeInfo>();

                // switch to search in a specific assembly
                if (commandString[0] == '@')
                {
                    // search specific assembly
                    asmName = commandString.Split(' ')[0].Remove(0, 1);

                    commandName = commandString.Split(' ')[1];

                    commandtypes = _assemblyservice.Get()
                        .Where(t => t.GetName().Name.Equals(asmName, StringComparison.CurrentCultureIgnoreCase))
                        .SelectMany(t => 
                            t.DefinedTypes
                                .Where(u => u.ImplementedInterfaces.Select(v => v.Name).Contains(nameof(ICommand)))
                                .Where(u => u.Name.Equals(commandName, StringComparison.CurrentCultureIgnoreCase)))
                        .ToList();
                }
                else
                {
                    // search for commands in all active assemblies
                    commandName = commandString.Split(' ')[0];

                    commandtypes = _assemblyservice.Get()
                        .SelectMany(t =>
                            t.DefinedTypes
                                .Where(u => u.ImplementedInterfaces.Select(v => v.Name).Contains(nameof(ICommand)))
                                .Where(u => u.Name.Equals(commandName, StringComparison.CurrentCultureIgnoreCase)))
                        .ToList();
                }

                if (commandtypes.Count == 0)
                {
                    throw new Exception($"unable to find command {commandName}");
                }

                if (commandtypes.Count > 1)
                {
                    string msg = $"multiple commands found:{Environment.NewLine}";
                    commandtypes.ForEach(t => { msg = msg + $"   {t.FullName}{Environment.NewLine}"; });

                    throw new Exception(msg);
                }

                Type type = commandtypes[0].AsType();

                var constructors = type.GetConstructors();

                if (constructors.Count() > 1)
                {
                    throw new Exception($"Multiple constructors found for {commandName}");
                }

                var constructorparams = constructors[0].GetParameters()
                    .Select(t => Program.ServiceProvider.GetService(t.ParameterType));

                object[] constructorparamsarray = constructorparams.ToArray();

                bool nullconstructor = constructorparams.Count() == 0;
                nullconstructor = nullconstructor || (constructorparams.Where(t => t != null).Count() == 0);

                var functioninstance = nullconstructor ? Activator.CreateInstance(type) : Activator.CreateInstance(type, constructorparamsarray);

                MethodInfo methodinfo = null;
                var args = ArgumentsParser.ParseArgumentsFromString(commandString, type, ref methodinfo);
                ParameterInfo[] paramsinfo = methodinfo.GetParameters();

                methodinfo.Invoke(functioninstance, args);
            }
            catch (Exception ex)
            {
                _loggingservice.LogException(ex);
            }
        }
    }
}