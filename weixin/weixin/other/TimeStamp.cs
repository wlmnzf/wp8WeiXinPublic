using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weixin
{
   public static  class TimeStamp
    {
        /// <summary>
        /// 将Unix时间戳格式转换为c# DateTime时间格式
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns>DateTime </returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime Start = new DateTime(1970, 1, 1);
            long lTime =long.Parse( timeStamp +"0000000");
            TimeSpan toNow = new TimeSpan(lTime);
             DateTime dtStart=Start.Add(toNow);
             return dtStart.AddHours(8);
        }
        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            int intResult = 0;
            System.DateTime startTime =new System.DateTime(1970, 1, 1);
            intResult = (int)(time.ToUniversalTime() - startTime).TotalSeconds;
            return intResult;
        }


    }
}
