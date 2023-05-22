using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AppsPortal.Library
{
    public class AppSettingsKeys
    {
        public static string Domain
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["Domain"]; }
        }
        public static string MediaURL
        {
            get { return ConfigurationManager.AppSettings["MediaURL"]; }
        }
        public static string ServerAccessibility
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ServerAccessibility"]; }
        }
    }
}