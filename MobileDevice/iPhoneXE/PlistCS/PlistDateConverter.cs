using System;

namespace PlistCS
{
    // Token: 0x02000021 RID: 33
    public static class PlistDateConverter
    {
        // Token: 0x06000130 RID: 304 RVA: 0x00008484 File Offset: 0x00006684
        public static DateTime ConvertFromAppleTimeStamp(double timestamp)
        {
            DateTime dateTime = new DateTime(2001, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(timestamp);
        }

        // Token: 0x06000131 RID: 305 RVA: 0x000084B0 File Offset: 0x000066B0
        public static double ConvertToAppleTimeStamp(DateTime date)
        {
            DateTime d = new DateTime(2001, 1, 1, 0, 0, 0, 0);
            return Math.Floor((date - d).TotalSeconds);
        }

        // Token: 0x0600012E RID: 302 RVA: 0x00008454 File Offset: 0x00006654
        public static long GetAppleTime(long unixTime)
        {
            return unixTime - PlistDateConverter.timeDifference;
        }

        // Token: 0x0600012F RID: 303 RVA: 0x0000846C File Offset: 0x0000666C
        public static long GetUnixTime(long appleTime)
        {
            return appleTime + PlistDateConverter.timeDifference;
        }

        // Token: 0x0400009A RID: 154
        public static long timeDifference = 978307200L;
    }
}
