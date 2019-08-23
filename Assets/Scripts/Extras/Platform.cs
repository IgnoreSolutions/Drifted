/**

Copyright (C) 2019 Mike Santiago - All Rights Reserved
axiom@ignoresolutions.xyz

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted, provided that the above
copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

*/


using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Drifted.Extras
{
    public enum Platform
    {
        macOS,
        Windows,
        Linux,
        PS4,
        Vita,
        Switch,
        XboxOne,
        VR,
        Unknown
    }
    public static class PlatformUtilities
    {
        public static Platform GetCurrentPlatform()
        {
            bool isWindows = (Path.DirectorySeparatorChar == '\\');

            if (isWindows) return Platform.Windows;
            else
            {
                string unixType = SystemInfo.operatingSystem;
                UnityEngine.Debug.Log(unixType);
                if (unixType.Contains("Mac OS X") || unixType.Contains("macOS")) return Platform.macOS;
                else if (unixType.Contains("Linux")) return Platform.Linux;
            }

            return Platform.Unknown;
        }

        private static string ReadProcessOutput(string name, string args = "")
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.RedirectStandardOutput = true;
                if (!String.IsNullOrEmpty(args)) p.StartInfo.Arguments += $" {args}";
                p.StartInfo.FileName = name;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (output == null) output = "";
                return output.Trim();
            }
            catch
            {
                return "";
            }
        }
    }
}
