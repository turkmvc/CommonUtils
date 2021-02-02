using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToStringify(this DateTime datetime)
        {

            TimeSpan sp = DateTime.Now - datetime;
            if(sp.TotalSeconds < 15)
            {
                return "az önce";
            }
            else if(sp.TotalSeconds < 60)
            {
                return "{0} sn. önce".Fmt((int)sp.TotalSeconds);
            }
            else if(sp.TotalMinutes < 60)
            {
                return "{0} dk. önce".Fmt((int)sp.TotalMinutes);
            }
            else if(sp.TotalHours < 24)
            {
                return "{0} saat önce".Fmt((int)sp.TotalHours);
            }
            else if(sp.TotalDays < 30)
            {
                return "{0} gün önce".Fmt((int)sp.TotalDays);
            }
            else if(sp.TotalDays >= 365)
            {
                return "{0} yıl önce".Fmt((int) sp.TotalDays / 365);
            }
            return "";
        }
    }
}
