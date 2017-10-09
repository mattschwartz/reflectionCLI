using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using System.Runtime.Loader;

namespace reflectionCli {
    public class LoadAssembly : ICommand    {

        public LoadAssembly(String path) {
            try
            {
                Assembly TempAsm = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(File.ReadAllBytes(path)));

                //need to check and
                var tempicommand = TempAsm.GetTypes().ToList().Where(x => (x.Name == "ICommand")).ToList();

                if (Program.activeasm.Select(x => x.Value.GetName().Name).Contains(TempAsm.GetName().Name)) {
                    throw new Exception($"An Assembly named {TempAsm.GetName().Name} already exists.");
                }

                if (tempicommand.Count == 0) {
                    throw new Exception("Unable to find ICommand");
                }

                if (tempicommand.Count > 1) {
                    throw new Exception("Multiple ICommands Found");
                }

                Type type = tempicommand[0];

                var extvalinfo = type.GetMethods().ToList()
                                    .Where(x => (x.Name == "ExitVal"))
                                    .Where(x => (x.ReturnType == typeof(bool)))
                                    .ToList();

                if (extvalinfo.Count == 0) {
                    throw new Exception("ICommand did not return the correct exit value");
                }

                Program.activeasm.Add(new Guid() ,TempAsm);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public bool ExitVal()   {
            return false;
        }
    }

}