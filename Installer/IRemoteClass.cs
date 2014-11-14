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

namespace FakeAsm
{
    public interface IRemoteClass
    {
        Process RunProcess(string process, string args);

        string RunCommand(string cmd);

        int ExecuteAssembly(byte[] asm, string[] args);

        DirectoryInfo GetDirectory(string path);

        void WriteFile(string path, byte[] contents);

        byte[] ReadFile(string path);

        string GetUsername();

        OperatingSystem GetOSVersion();
    }
}
