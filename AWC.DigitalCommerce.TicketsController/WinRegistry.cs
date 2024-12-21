using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace AWC.DigitalCommerce.TicketsController
{
    public class WinRegistry
    {
        public const string AIDAWARE = @"SOFTWARE\AIDAware\TicketsController";

        public static string GetValueFromWinRegistryKey(string key)
        {
            try
            {
                using (RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    string value = localKey.OpenSubKey(AIDAWARE).GetValue(key).ToString();
                    return value;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return string.Empty;
            }
        }
    }
}
