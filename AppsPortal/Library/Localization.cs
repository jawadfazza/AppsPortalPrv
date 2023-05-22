using AppsPortal.Library;
using System.Collections.Generic;

namespace AppsPortal.Extensions
{
    public class Localization
    {
        private static string UserTimePref = "hh:mm:ss tt";

        private static Dictionary<string, string> DateFormat = new Dictionary<string, string>
            {
            //Formats Below Should Match 100% the used format of MomentJS ('LL')
                {"en-US", "MMMM dd, yyyy" },
                {"en-GB", "dd MMMM yyyy"},
                {"fr-FR", "dd MMMM yyyy"},
                {"ar-IQ", "dd MMMM yyyy"},
                {"ar-SY", "dd MMMM yyyy"},
                {"ar-TN", "dd MMMM yyyy"},
            };

        public static string Date()
        {
            return DateFormat[Languages.CurrentCulture()];
        }

        public static string LongDate()
        {
            //Long Date means the same date format with Day Name
            return string.Format("DDDD, {1}", Date());
        }

        public static string DateTime()
        {
            return string.Format("{0} {1}", Date(), UserTimePref);
        }

        public static string LongDateTime()
        {
            return string.Format("dddd, {0} {1}", Date(), UserTimePref);
        }

        public static string NumberFormat(string input)
        {
            switch (Languages.CurrentCulture())
            {
                case "ar-IQ": return Numbers.EnglishToArabic(input);
                case "ar-SY": return Numbers.EnglishToArabic(input);

                //case "XY": return Numbers.XY(input);
                default: return input;
            }
        }
    }
}