using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FakeAsm")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("FakeAsm")]
[assembly: AssemblyCopyright("Copyright")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("297cbca1-efa3-4f2a-8d5f-e1faf02ba587")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace FakeAsm
{
    [Guid("62A9EA9C-434D-4145-B84A-A8FF3092D212")]
    public class RemoteClass : MarshalByRefObject, IRemoteClass
    {
        public Process RunProcess(string process, string args)
        {
            return Process.Start(process, args);
        }

        public string RunCommand(string cmd)
        {
            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo("cmd.exe", "/c " + cmd);
            Process proc = new Process();
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe",  "/c " + cmd);
                
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;
            proc.StartInfo = info;

            proc.Start();

            string strOutput = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return strOutput;
        }

        public int ExecuteAssembly(byte[] asm, string[] args)
        {
            Assembly a = Assembly.Load(asm);

            MethodInfo mi = a.EntryPoint;

            if (mi == null)
            {
                return -1;
            }
            else
            {
                ParameterInfo[] pis = mi.GetParameters();
                object[] newargs;

                if (pis.Length > 0)
                {
                    newargs = new object[] { args };
                }
                else
                {
                    newargs = new object[0];
                }

                object ret = mi.Invoke(null, newargs);

                if ((mi.ReturnType == typeof(int)) && (ret is int))
                {
                    return (int)ret;
                }
                else
                {
                    return 0;
                }
            }
        }

        public DirectoryInfo GetDirectory(string path)
        {
            return new DirectoryInfo(path);
        }

        public void WriteFile(string path, byte[] contents)
        {
            File.WriteAllBytes(path, contents);
        }

        public byte[] ReadFile(string path)
        {
            return File.ReadAllBytes(path);
        }

        public string GetUsername()
        {
            return Environment.UserName;
        }

        public OperatingSystem GetOSVersion()
        {
            return Environment.OSVersion;
        }
    }
}
