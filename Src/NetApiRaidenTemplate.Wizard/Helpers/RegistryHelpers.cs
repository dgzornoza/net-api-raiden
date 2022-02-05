using System;
using Microsoft.Win32;

namespace NetApiRaidenTemplate.Wizard.Helpers
{
    /// <summary>
    /// Helper for read/write in Win register
    /// </summary>
    public static class RegistryHelpers
    {
        private const string KeyName = "SOFTWARE\\NetApiRaiden";

        /// <summary>
        /// Mehod for get value from win registry
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="entryName">Entry name for read value</param>
        /// <param name="defaultValue">default value if none exists</param>
        /// <returns>Value from registry or default value if none exists</returns>
        public static T GetValue<T>(string entryName, T defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(entryName))
            {
                throw new ArgumentException("entryName");
            }

            T result = defaultValue;
            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(KeyName))
            {
                if (registryKey != null)
                {
                    object obj = registryKey.GetValue(entryName, defaultValue, RegistryValueOptions.DoNotExpandEnvironmentNames);
                    if (obj.ToString().ToLower() == "true")
                    {
                        obj = true;
                    }
                    else if (obj.ToString().ToLower() == "false")
                    {
                        obj = false;
                    }
                    else if (typeof(T).IsEnum)
                    {
                        return (T)Enum.Parse(typeof(T), obj.ToString());
                    }

                    result = (T)obj;
                }
            }

            return result;
        }

        /// <summary>
        /// method for write value in win registry
        /// </summary>
        /// <param name="entryName">Entry name for write value</param>
        /// <param name="value">Value to write in registry</param>
        public static void WriteValue(string entryName, object value)
        {
            value = value ?? throw new ArgumentNullException("value");

            if (string.IsNullOrWhiteSpace(entryName))
            {
                throw new ArgumentException("entryName");
            }

            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(KeyName))
            {
                registryKey?.SetValue(entryName, value);
            }
        }
    }
}
