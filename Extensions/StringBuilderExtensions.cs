using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void AppendLineFormat(this StringBuilder sB, string format, params object[] arg0)
        {
            sB.AppendLine();
            sB.AppendFormat(format, arg0);
        }
    }
}
