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
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using NDesk.Options;

namespace ExampleRemotingService
{
    class Program
    {
        class RemoteType : MarshalByRefObject
        {
        }

        static void SetAllowTransparentProxy(bool allow)
        {
            ConfigurationManager.AppSettings.Set("microsoft:Remoting:AllowTransparentProxyMessage", allow.ToString());
        }

        static void Main(string[] args)
        {
            try
            {
                bool secure = false;
                int port = 12345;
                string ipc = string.Empty;
                bool bind_any = false;
                bool showhelp = false;
                TypeFilterLevel typefilter = TypeFilterLevel.Low;
                CustomErrorsModes custom_errors = CustomErrorsModes.Off;
                string name = "RemotingServer";
                bool disable_transparent_proxy_fix = false;

                OptionSet p = new OptionSet() {
                    { "s|secure", "Enable secure mode", v => secure = v != null },
                    { "p|port=", "Specify the local TCP port to listen on", v => port = int.Parse(v) },
                    { "t|typefilter=", "Specify the type filter level (low,full), default low",
                        v => typefilter = (TypeFilterLevel)Enum.Parse(typeof(TypeFilterLevel), v, true) },
                    { "i|ipc=", "Specify listening pipe name for IPC channel", v => ipc = v },
                    { "e|error=", "Set custom error mode (On, Off, RemoteOnly) (don't show full errors in remote calls)",
                            v => custom_errors = (CustomErrorsModes)Enum.Parse(typeof(CustomErrorsModes), v, true) },
                    { "n|name=", "Set the remoting class name", v => name = v },
                    { "d", "Enable the 'AllowTransparentProxyMessage' setting fix.", v => disable_transparent_proxy_fix = v != null },
                    { "a|any", "When using TCP bind to any interface, otherwise only to localhost", v => bind_any = v != null },
                    { "h|?|help",   v => showhelp = v != null },
                };

                Console.WriteLine("Example .NET Remoting Server");
                Console.WriteLine("Copyright (c) James Forshaw 2014");
                Console.WriteLine(".NET Version: {0}", Environment.Version);
                p.Parse(args);
                if (showhelp)
                {
                    p.WriteOptionDescriptions(Console.Out);
                }
                else
                {
                    SetAllowTransparentProxy(!disable_transparent_proxy_fix);
                    RemotingConfiguration.CustomErrorsMode = custom_errors;

                    Console.WriteLine("Enable Transparent Proxy Fix: {0}", disable_transparent_proxy_fix);
                    Console.WriteLine("Custom Errors Mode: {0}", custom_errors);
                    Console.WriteLine("Type Filter Level: {0}", typefilter);

                    Trace.Listeners.Add(new ConsoleTraceListener(true));

                    IChannel chan;
                    IDictionary properties = new Hashtable();

                    BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
                    serverSinkProvider.TypeFilterLevel = typefilter;

                    if (!string.IsNullOrEmpty(ipc))
                    {
                        properties["portName"] = ipc;
                        properties["authorizedGroup"] = "Everyone";

                        chan = new IpcChannel(properties, new BinaryClientFormatterSinkProvider(), serverSinkProvider);
                    }
                    else
                    {
                        Console.WriteLine("Any Bind: {0}", bind_any);
                        properties["port"] = port;
                        properties["rejectRemoteRequests"] = !bind_any;
                        chan = new TcpChannel(properties, new BinaryClientFormatterSinkProvider(), serverSinkProvider);
                    }

                    ChannelServices.RegisterChannel(chan, secure);    //register channel

                    RemotingConfiguration.RegisterWellKnownServiceType(
                        typeof(RemoteType),
                        name,
                        WellKnownObjectMode.Singleton);

                    bool isipc = chan is IpcChannel;

                    Console.WriteLine("Server Activated at {0}://{1}/{2}", isipc ? "ipc" : "tcp", isipc ? ipc : "HOST:" + port.ToString(), name);
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
