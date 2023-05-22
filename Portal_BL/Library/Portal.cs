using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal_BL.Library
{
    public class Portal
    {
        public DateTime? LocalTime(DateTime? value)
        {
            if (value == null) return null;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
            DateTime dtz = TimeZoneInfo.ConvertTimeFromUtc((DateTime)value, tz);
            return dtz;
        }
    }
}
