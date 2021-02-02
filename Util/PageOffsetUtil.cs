using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Util
{
    public static class PageOffsetUtil
    {
        public static int GetTotalPage(int totalitem, int rowscount)
        {
            return (int)Math.Ceiling((double)totalitem / (double)rowscount);
        }
        public static int GetOffset(int currentoffset, int rowcount, int itemscount)
        {
            if (currentoffset >= itemscount)
            {
                var kalan = itemscount % rowcount;
                if (kalan == 0) kalan = rowcount;
                currentoffset = itemscount - kalan;
                if (currentoffset < 0) currentoffset = 0;
            }
            return currentoffset;
        }
    }
}
