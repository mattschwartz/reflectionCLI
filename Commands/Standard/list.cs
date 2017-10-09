using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;


namespace reflectionCli {

    public class list : ICommand    {

        public list() {
            Console.WriteLine("Valid Commands:");
            Program.activeasm.Select(a => a.Value).ToList().ForEach(x => {
                Console.WriteLine(Environment.NewLine + "   - " + x.FullName);
                x.DefinedTypes.Where(z => (
                    //this has to be done this way as the ICommand interface is not object equivalent for runtime loaded assemblies
                    z.ImplementedInterfaces.Where(a => (a.Name == "ICommand"))
                                            .ToList()
                                            .Count != 0
                ))
                .ToList()
                .ForEach(y => Console.WriteLine("       - " + y.Name));
            });
        }

        public list(string name) {
            Console.WriteLine($"Valid Commands for {name}:");
            Program.activeasm.Select(a => a.Value).ToList().ForEach(x => {
                Console.WriteLine(Environment.NewLine + "   - " + x.FullName);
                x.DefinedTypes.Where(z => (
                    //this has to be done this way as the ICommand interface is not object equivalent for runtime loaded assemblies
                    z.ImplementedInterfaces.Where(a => (a.Name == "ICommand"))
                                            .ToList()
                                            .Count != 0
                ))
                .Where(a => (a.Name == name))
                .ToList()
                .ForEach(y => {
                    y.AsType().GetConstructors().ToList().ForEach(z => {
                        Console.WriteLine(Environment.NewLine + "      - " + name);
                        z.GetParameters().ToList().ForEach(a => {
                            Console.WriteLine($"        - {a.Name} ({a.ParameterType.FullName})");
                        });
                    });
                });
            });
        }
        public bool ExitVal()   {
            return false;
        }
    }
}