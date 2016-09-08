using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dubbingApp.Utils
{
    public static class TimeConverter
    {
        public static int StringToSeconds(string time)
        {
            if (string.IsNullOrEmpty(time))
                return 0;

            int result = 0;
            int[] timeParts = time.Split(':').Select(x => Convert.ToInt32(x)).ToArray();
            if(timeParts.Length == 2)
            {
                result = (timeParts[0] * 60) + timeParts[0];
                return result;
            }

            result = (timeParts[0] * 3600) + (timeParts[1] * 60) + timeParts[2];
            return result;
        }
        public static string SecondsToTime(int seconds) 
        {
            string result = string.Empty;
            int h = (int) Math.Floor(seconds / 3600f);
            int hs = h * 3600;
            int m = (int)Math.Floor((seconds - hs) / 3600f);
            int ms = m * 60;
            int s = seconds - hs - ms;
            result = string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
            return result;
        }
    }
}