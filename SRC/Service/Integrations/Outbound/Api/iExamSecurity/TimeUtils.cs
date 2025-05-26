using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iExamSecurity.Util
{
    public static class TimeUtils
    {
        public static long GetUnixTimstamp(DateTime date)
        {
            DateTime zero = new DateTime(1970, 1, 1);
            TimeSpan span = date.Subtract(zero);

            return (long)span.TotalMilliseconds;
        }

        public static long GetUnixTimstamp()
        {
            return GetUnixTimstamp(DateTime.UtcNow);
        }

    }
}