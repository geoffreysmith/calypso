using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Calypso
{
    public class RegisterCulture
    {
        private const int MaxPath = 260;
        private const string CustomKey = @"SYSTEM\CurrentControlSet\Control\Nls\CustomLocale";

        [DllImport("kernel32", SetLastError = true)]
        private static extern int GetWindowsDirectory(StringBuilder lpBuffer,
            int nSize);

        public static CultureAndRegionModifiers ParseCultureAndRegion(string cultureAndRegionModifier)
        {
            // this is case sensitive!
            if (!Enum.IsDefined(typeof (CultureAndRegionModifiers), cultureAndRegionModifier))
                throw new InvalidEnumArgumentException("cultureAndRegionModifier: " + cultureAndRegionModifier);

            return
                (CultureAndRegionModifiers)
                    Enum.Parse(typeof (CultureAndRegionModifiers), cultureAndRegionModifier);
        }

        public static bool CustomCultureExists(string cultureName)
        {
            var userCultures = CultureInfo.GetCultures(CultureTypes.UserCustomCulture);

            var customCulture = userCultures.FirstOrDefault
                (x => x.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase));

            return customCulture != null;
        }

        public static void RegisterAndBuild(string cultureName,
            string cultureAndRegionModifier,
            string existingRegionIsoCode,
            string newRegionIsoCode)
        {
            CultureAndRegionModifiers cultureAndRegion;

            Enum.TryParse(cultureAndRegionModifier, true, out cultureAndRegion);

            // Create an alternate en-US culture.
            var enUs = new CultureAndRegionInfoBuilder(cultureName, cultureAndRegion);

            enUs.LoadDataFromCultureInfo(CultureInfo.CreateSpecificCulture(existingRegionIsoCode));
            enUs.LoadDataFromRegionInfo(new RegionInfo(newRegionIsoCode));

            // Use reflection to get the CultureDefinition.Compile method.
            var assem = Assembly.Load("sysglobl, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            var defType = assem.GetType("System.Globalization.CultureDefinition");
            var method = defType.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Static);
            var tempPath = @".\" + cultureName + ".nlp";
            object[] args = {enUs, tempPath};

            // Delete target file if it already exists.
            if (File.Exists(tempPath))
                File.Delete(tempPath);

            // Compile the culture definition.
            method.Invoke(null, args);
            // Copy the file.
            try
            {
                var buffer = new StringBuilder(MaxPath);
                var charsWritten = GetWindowsDirectory(buffer, MaxPath);
                var fileName = String.Format("{0}{1}Globalization{1}{2}.nlp",
                    buffer.ToString().Substring(0, charsWritten),
                    Path.DirectorySeparatorChar,
                    cultureName);
                File.Copy(tempPath, fileName, true);
                WriteToRegistry(CustomKey, cultureName);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("You must run this application as an administrator");
                Console.WriteLine("so that you can install culture definition files.");
            }
        }

        public static void UnregisterCulture(string cultureName)
        {
            try
            {
                CultureAndRegionInfoBuilder.Unregister(cultureName);
            }
            catch (Exception)
            {
                Console.WriteLine("There was a problem unregistering your culture.");
            }
        }

        public static void WriteToRegistry(string keyName, string valueName)
        {
            var key = Registry.LocalMachine.OpenSubKey(keyName, true);

            // Create the key if it does not already exist.
            if (key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(keyName);
                if (key == null) throw new NullReferenceException("Cannot create the registry key");
            }

            // Set the new name
            key.SetValue(valueName, valueName);
            key.Close();
        }
    }
}
