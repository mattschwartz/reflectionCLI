﻿using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace reflectionCli {
    class Program    {
        public static void Main(string[] args)  {
            var exit = false;
            while(exit == false)    {
                Console.WriteLine();
                Console.WriteLine("Enter command (help to display help): ");
                var command = Parser.Parse(Console.ReadLine());
                exit = command.ExitVal();
            }
        }
    }

    public static class Parser    {
        public static ICommand Parse(string commandString) {

            var commandParts = commandString.Split(' ').ToList();
            var commandName = commandParts[0];
            var args = commandParts.Skip(1).ToList();

            var commandtypes = Assembly.GetEntryAssembly().DefinedTypes
                                .Where(x => x.ImplementedInterfaces.Contains(typeof(ICommand)))
                                .Where(x => (x.Name == commandName))
                                .ToList();

            if (commandtypes.Count == 0) {
                return new error("unable to find command");
            }

            if (commandtypes.Count > 1) {
                string msg = "multiple commands found: " + Environment.NewLine;
                commandtypes.ForEach(x => { msg = msg + "   " + x.Name + Environment.NewLine; });
                return new error(msg);
            }

            //Assembly assembly = typeof(commandset).GetTypeInfo().Assembly;
            //Type type = assembly.GetType("reflectionCli.commandset+" + commandName);
            Type type = commandtypes[0].AsType();

            ConstructorInfo constructorInfo = type.GetConstructors()[0];

            object result = null;
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            if (parameters.Length == 0) {
                result = Activator.CreateInstance(type, null);
            }
            else {
                result = Activator.CreateInstance(type, new object[] { args } );
            }

            return (ICommand)result;
        }
    }


}
