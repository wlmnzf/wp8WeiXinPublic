using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weixin
{
    public class AllPeoPle<T> : List<T>
    {

        static string[] keys;
        /// <summary>
        /// The delegate that is used to get the key information.
        /// </summary>
        /// <param name="item">An object of type T</param>
        /// <returns>The key value to use for this object</returns>
        public delegate string GetKeyDelegate(T item);

        /// <summary>
        /// The Key of this group.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="key">The key for this group.</param>
        public AllPeoPle(string key)
        {
            Key = key;
        }


        /// <summary>
        /// Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The </param>
        /// <returns>Theitems source for a LongListSelector</returns>
        private static List<AllPeoPle<T>> CreateGroups()
        {
            if (Sto.Info.Contains(loginInfo.UserName+"nameList"))
            {
                string[] groupid = (string[])Sto.Info[loginInfo.UserName + "groupList"];
                string[] name = (string[])Sto.Info[loginInfo.UserName + "nameList"];
                string[] cnt = (string[])Sto.Info[loginInfo.UserName + "cntList"];
                keys=new string[name.Length];
                for (int i = 0; i < name.Length; i++)
                {
                    keys[i] = name[i]+"   "+cnt[i];
                }
            }

            List<AllPeoPle<T>> list = new List<AllPeoPle<T>>();

            foreach (string key in keys)
            {
                list.Add(new AllPeoPle<T>(key));
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
        public static List<AllPeoPle<T>> CreateGroups(IEnumerable<T> items, GetKeyDelegate getKey)
        {
             string[] groupid = (string[])Sto.Info[loginInfo.UserName + "groupList"];
                string[] name = (string[])Sto.Info[loginInfo.UserName + "nameList"];
                string[] cnt = (string[])Sto.Info[loginInfo.UserName + "cntList"];
            //SortedLocaleGrouping slg = new SortedLocaleGrouping(ci);
            List<AllPeoPle<T>> list = CreateGroups();
            foreach (T item in items)
            {
                for (int i = 0; i < groupid.Length; i++)
                {
                    if (groupid[i] == getKey(item))
                        list[i].Add(item);
                }
            }
          /*  foreach (AllPeoPle<T> group in list)
            {
                group.Sort((c0, c1) => { return DateTime.Compare(getKey(c1), getKey(c0)); });
            }*/
            return list;
        }
    }
}
