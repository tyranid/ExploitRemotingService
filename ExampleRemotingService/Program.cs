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
using System.Diagnostics;
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

        static void Main(string[] args)
        {
            try
            {
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
                bool secure = false;
                int port = 12345;                
                string ipc = String.Empty;
                bool showhelp = false;
                TypeFilterLevel typefilter = TypeFilterLevel.Low;

                OptionSet p = new OptionSet () {
   	            { "s|secure", "Enable secure mode", v => secure = v != null },   	 
                { "p|port=", "Specify the local TCP port to listen on", v => port = int.Parse(v) },
                { "t|typefilter=", "Specify the type filter level (low,full), default low", 
                    v => typefilter = (TypeFilterLevel)Enum.Parse(typeof(TypeFilterLevel), v, true) },
                { "i|ipc=", "Specify listening pipe name for IPC channel", v => ipc = v },
   	            { "h|?|help",   v => showhelp = v != null },
                };
                
                p.Parse (args);

                if (showhelp)
                {
                    Console.WriteLine("Example .NET Remoting Server");
                    Console.WriteLine("Copyright (c) James Forshaw 2014");
                    p.WriteOptionDescriptions(Console.Out);
                }
                else
                {
                    Trace.Listeners.Add(new ConsoleTraceListener(true));

                    IChannel chan;
                    IDictionary properties = new Hashtable();

                    BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
                    serverSinkProvider.TypeFilterLevel = typefilter;

                    if (!String.IsNullOrEmpty(ipc))
                    {
                        properties["portName"] = ipc;
                        properties["authorizedGroup"] = "Everyone";

                        chan = new IpcChannel(properties, new BinaryClientFormatterSinkProvider(), serverSinkProvider);
                    }
                    else
                    {
                        properties["port"] = port;
                        chan = new TcpChannel(properties, new BinaryClientFormatterSinkProvider(), serverSinkProvider);
                    }

                    ChannelServices.RegisterChannel(chan, secure);    //register channel

                    RemotingConfiguration.RegisterWellKnownServiceType(
                        typeof(RemoteType),
                        "RemotingServer",
                        WellKnownObjectMode.Singleton);

                    bool isipc = chan is IpcChannel;

                    Console.WriteLine("Server Activated at {0}://{1}/RemotingServer", isipc ? "ipc" : "tcp", isipc ? ipc : "HOST:" + port.ToString());

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
