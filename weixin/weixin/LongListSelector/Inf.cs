using System.Collections.Generic;
using System.Globalization;
using Microsoft.Phone.Globalization;
using System;


namespace weixin
{
    public class AlphaKeyGroup<T> : List<T>
    {
          static string[] keys = new string[5];
          static DateTime today, yesterday, theDayBeforeYesterday;
         
        /// <summary>
        /// The delegate that is used to get the key information.
        /// </summary>
        /// <param name="item">An object of type T</param>
        /// <returns>The key value to use for this object</returns>
        public delegate DateTime GetKeyDelegate(T item);
         public delegate string GetStar(T item);
        /// <summary>
        /// The Key of this group.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="key">The key for this group.</param>
        public AlphaKeyGroup(string key)
        {    
            Key = key;
             today=new  DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,0,0,0);
             yesterday = today.AddDays(-1);
            theDayBeforeYesterday = today.AddDays(-2);
           //todayStamp = TimeStamp.ConvertDateTimeInt(today);
          // yesterdayStamp = TimeStamp.ConvertDateTimeInt(today.AddDays(-1));
           //theDayBeforeYesterdayStamp = TimeStamp.ConvertDateTimeInt(today.AddDays(-2));
        }
  

        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The </param>
        /// <returns>Theitems source for a LongListSelector</returns>
        private static List<AlphaKeyGroup<T>> CreateGroups()
        {
            keys[0] = "星标"; keys[1] = "今天"; keys[2] = "昨天"; keys[3] = "前天"; keys[4] = "更早"; 
            List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();

            foreach (string key in keys)
            {
                list.Add(new AlphaKeyGroup<T>(key));
            }

            return list;
        }
        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, GetKeyDelegate getKey,GetStar getStar)
        {
            //SortedLocaleGrouping slg = new SortedLocaleGrouping(ci);
            List<AlphaKeyGroup<T>> list = CreateGroups();
            DateTime messageTime;
            string isstar;
            foreach (T item in items)
            {
                isstar = getStar(item);
                messageTime = getKey(item);
                if (isstar == "1")
                {
                    list[0].Add(item);
                }
                
                if (messageTime >=today)
                {
                    list[1].Add(item);
                }
                else if (messageTime >= yesterday)
                {
                    list[2].Add(item);
                }
                else if (messageTime >= theDayBeforeYesterday)
                {
                    list[3].Add(item);
                }
                else
                {
                    list[4].Add(item);
                }
            }
            foreach (AlphaKeyGroup<T> group in list)
            {
                group.Sort((c0, c1) => { return DateTime.Compare(getKey(c1), getKey(c0)); });
            }
            return list;
        }
    }
}
