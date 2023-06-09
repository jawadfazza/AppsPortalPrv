﻿
using System.Collections.Generic;
using System.Threading;


namespace AppsPortal.Extensions
{
    public class Languages
    {
        private static readonly List<string> _cultures = new List<string> {
            #region Microsoft Cultures List
            "af-ZA",  // Afrikaans - South Africa
            "sq-AL",  // Albanian - Albania
            "ar-DZ",  // Arabic - Algeria
            "ar-BH",  // Arabic - Bahrain
            "ar-EG",  // Arabic - Egypt
            "ar-IQ",  // Arabic - Iraq
            "ar-JO",  // Arabic - Jordan
            "ar-KW",  // Arabic - Kuwait
            "ar-LB",  // Arabic - Lebanon
            "ar-LY",  // Arabic - Libya
            "ar-MA",  // Arabic - Morocco
            "ar-OM",  // Arabic - Oman
            "ar-QA",  // Arabic - Qatar
            "ar-SA",  // Arabic - Saudi Arabia
            "ar-SY",  // Arabic - Syria
            "ar-TN",  // Arabic - Tunisia
            "ar-AE",  // Arabic - United Arab Emirates
            "ar-YE",  // Arabic - Yemen
            "hy-AM",  // Armenian - Armenia
            "Cy-az-AZ",  // Azeri (Cyrillic) - Azerbaijan
            "Lt-az-AZ",  // Azeri (Latin) - Azerbaijan
            "eu-ES",  // Basque - Basque
            "be-BY",  // Belarusian - Belarus
            "bg-BG",  // Bulgarian - Bulgaria
            "ca-ES",  // Catalan - Catalan
            "zh-CN",  // Chinese - China
            "zh-HK",  // Chinese - Hong Kong SAR
            "zh-MO",  // Chinese - Macau SAR
            "zh-SG",  // Chinese - Singapore
            "zh-TW",  // Chinese - Taiwan
            "zh-CHS",  // Chinese (Simplified)
            "zh-CHT",  // Chinese (Traditional)
            "hr-HR",  // Croatian - Croatia
            "cs-CZ",  // Czech - Czech Republic
            "da-DK",  // Danish - Denmark
            "div-MV",  // Dhivehi - Maldives
            "nl-BE",  // Dutch - Belgium
            "nl-NL",  // Dutch - The Netherlands
            "en-AU",  // English - Australia
            "en-BZ",  // English - Belize
            "en-CA",  // English - Canada
            "en-CB",  // English - Caribbean
            "en-IE",  // English - Ireland
            "en-JM",  // English - Jamaica
            "en-NZ",  // English - New Zealand
            "en-PH",  // English - Philippines
            "en-ZA",  // English - South Africa
            "en-TT",  // English - Trinidad and Tobago
            "en-GB",  // English - United Kingdom
            "en-US",  // English - United States
            "en-ZW",  // English - Zimbabwe
            "et-EE",  // Estonian - Estonia
            "fo-FO",  // Faroese - Faroe Islands
            "fa-IR",  // Farsi - Iran
            "fi-FI",  // Finnish - Finland
            "fr-BE",  // French - Belgium
            "fr-CA",  // French - Canada
            "fr-FR",  // French - France
            "fr-LU",  // French - Luxembourg
            "fr-MC",  // French - Monaco
            "fr-CH",  // French - Switzerland
            "gl-ES",  // Galician - Galician
            "ka-GE",  // Georgian - Georgia
            "de-AT",  // German - Austria
            "de-DE",  // German - Germany
            "de-LI",  // German - Liechtenstein
            "de-LU",  // German - Luxembourg
            "de-CH",  // German - Switzerland
            "el-GR",  // Greek - Greece
            "gu-IN",  // Gujarati - India
            "he-IL",  // Hebrew - Israel
            "hi-IN",  // Hindi - India
            "hu-HU",  // Hungarian - Hungary
            "is-IS",  // Icelandic - Iceland
            "id-ID",  // Indonesian - Indonesia
            "it-IT",  // Italian - Italy
            "it-CH",  // Italian - Switzerland
            "ja-JP",  // Japanese - Japan
            "kn-IN",  // Kannada - India
            "kk-KZ",  // Kazakh - Kazakhstan
            "kok-IN",  // Konkani - India
            "ko-KR",  // Korean - Korea
            "ky-KZ",  // Kyrgyz - Kazakhstan
            "lv-LV",  // Latvian - Latvia
            "lt-LT",  // Lithuanian - Lithuania
            "mk-MK",  // Macedonian (FYROM)
            "ms-BN",  // Malay - Brunei
            "ms-MY",  // Malay - Malaysia
            "mr-IN",  // Marathi - India
            "mn-MN",  // Mongolian - Mongolia
            "nb-NO",  // Norwegian (Bokmål) - Norway
            "nn-NO",  // Norwegian (Nynorsk) - Norway
            "pl-PL",  // Polish - Poland
            "pt-BR",  // Portuguese - Brazil
            "pt-PT",  // Portuguese - Portugal
            "pa-IN",  // Punjabi - India
            "ro-RO",  // Romanian - Romania
            "ru-RU",  // Russian - Russia
            "sa-IN",  // Sanskrit - India
            "Cy-sr-SP",  // Serbian (Cyrillic) - Serbia
            "Lt-sr-SP",  // Serbian (Latin) - Serbia
            "sk-SK",  // Slovak - Slovakia
            "sl-SI",  // Slovenian - Slovenia
            "es-AR",  // Spanish - Argentina
            "es-BO",  // Spanish - Bolivia
            "es-CL",  // Spanish - Chile
            "es-CO",  // Spanish - Colombia
            "es-CR",  // Spanish - Costa Rica
            "es-DO",  // Spanish - Dominican Republic
            "es-EC",  // Spanish - Ecuador
            "es-SV",  // Spanish - El Salvador
            "es-GT",  // Spanish - Guatemala
            "es-HN",  // Spanish - Honduras
            "es-MX",  // Spanish - Mexico
            "es-NI",  // Spanish - Nicaragua
            "es-PA",  // Spanish - Panama
            "es-PY",  // Spanish - Paraguay
            "es-PE",  // Spanish - Peru
            "es-PR",  // Spanish - Puerto Rico
            "es-ES",  // Spanish - Spain
            "es-UY",  // Spanish - Uruguay
            "es-VE",  // Spanish - Venezuela
            "sw-KE",  // Swahili - Kenya
            "sv-FI",  // Swedish - Finland
            "sv-SE",  // Swedish - Sweden
            "syr-SY",  // Syriac - Syria
            "ta-IN",  // Tamil - India
            "tt-RU",  // Tatar - Russia
            "te-IN",  // Telugu - India
            "th-TH",  // Thai - Thailand
            "tr-TR",  // Turkish - Turkey
            "uk-UA",  // Ukrainian - Ukraine
            "ur-PK",  // Urdu - Pakistan
            "Cy-uz-UZ",  // Uzbek (Cyrillic) - Uzbekistan
            "Lt-uz-UZ",  // Uzbek (Latin) - Uzbekistan
            "vi-VN"  // Vietnamese - Vietnam
            #endregion  
        };


        public static string CurrentLanguage()
        {
            if (!Thread.CurrentThread.CurrentCulture.Name.Contains("-"))
            {
                return Thread.CurrentThread.CurrentCulture.Name.ToUpper();
            }
            else
            {
                return Thread.CurrentThread.CurrentCulture.Name.Split('-')[0].ToUpper(); // Read first part only. E.g. "en", "es"
            }
        }

        public static string CurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        public static bool IsRighToLeft()
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
        }

    }
}