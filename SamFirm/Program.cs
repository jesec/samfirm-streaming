using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace SamFirm
{
    internal static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("This program needs two arguments: MODEL and CSC");
                return -1;
            }
            Command.Firmware FW = Command.UpdateCheckAuto(args[0], args[1], true);
            string filename = string.Join("_", new string[] { FW.Model, FW.Region });
            Console.WriteLine("\nOutput: " + Path.GetFullPath(filename));
            if (FW.BinaryNature == 1)
            {
                Decrypt.SetDecryptKey(FW.Version, FW.LogicValueFactory);
            }
            else
            {
                Decrypt.SetDecryptKey(FW.Version, FW.LogicValueHome);
            }
            return Command.Download(FW.Path, FW.Filename, FW.Version, FW.Region, FW.Model_Type, filename, FW.Size);
        }
    }
}