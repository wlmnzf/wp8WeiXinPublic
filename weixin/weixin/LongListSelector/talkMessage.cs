using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace weixin
{
    public class talkMessage<T> : List<T>
    {
        public string Key { get; private set; }

        public talkMessage(string key)
        {
            Key = key;
        }

        public static List<talkMessage<T>> CreateGroups(IEnumerable<T> items, int count)
        {
            List<talkMessage<T>> list = new List<talkMessage<T>>();
            list.Add(new talkMessage<T>("所有信息"));
            list.Add(new talkMessage<T>("倒数5个"));
            int i = 0;
            foreach (T item in items)
            {
                  i++;
                  if (count > 5 && count - i < 5)
                      list[1].Add(item);
                  else
                     list[0].Add(item);
            }
         
            return list;
        }
    }


    public class PersonMessage
    {
        public BitmapImage l_imagesource { get; set; }
        public string l_fakeid { get; set; }
        public string l_content { get; set; }
        public DateTime l_time { get; set; }


        public PersonMessage(BitmapImage i, string f, string c, DateTime t)
        {
            this.l_imagesource = i;
            this.l_fakeid = f;
            this.l_content = c;
            this.l_time = t;
        }
    }

    public class PersonMessages
    {
        public List<talkMessage<PersonMessage>> Data;
        public PersonMessages(TalkPage talkPage, string fakeid)
        {
            List<PersonMessage> source = new List<PersonMessage>();
            IsolatedStorageFileStream readstream=null;
            for (int i = 0; i < talkPage.messageCol.TalkMessageTables.Count; i++)
            {
                BitmapImage newjpg = new BitmapImage();
                if (talkPage.messageCol.TalkMessageTables[i].Fakeid== fakeid)
                {
                    try
                    {
                        string url = loginInfo.UserName + "AllInfo" + "\\ico" + fakeid + ".jpg";
                        readstream = Sto.File.OpenFile(url, FileMode.Open, FileAccess.Read);
                        newjpg.SetSource(readstream);
                    }
                    catch
                    {
                        string l = "/design/otherico/getheadimg.png";
                        newjpg.UriSource = new Uri(l, UriKind.Relative);
                    }
                }
                else
                {
                    try
                    {
                        string url = loginInfo.UserName + "\\ico.jpg";
                        readstream = Sto.File.OpenFile(url, FileMode.Open, FileAccess.Read);
                        newjpg.SetSource(readstream);
                    }
                    catch 
                    {
                        string l = "/design/otherico/getheadimg.png";
                        newjpg.UriSource = new Uri(l, UriKind.Relative);
                    }
                }

                

                string talkfakeid = talkPage.messageCol.TalkMessageTables[i].Fakeid;
                string time = talkPage.messageCol.TalkMessageTables[i].Time;
                string content = "";
                if (talkPage.messageCol.TalkMessageTables[i].Type == "3")
                {
                    content = "【音频】";
                }
                else if (talkPage.messageCol.TalkMessageTables[i].Type == "15")
                {
                    content = "【视频】";
                }
                else if (talkPage.messageCol.TalkMessageTables[i].Type== "2")
                {
                    content = "【图片】";
                }
                else if (talkPage.messageCol.TalkMessageTables[i].Type == "6")
                {
                    content = "【图文消息】";
                }
                else
                {
                        content = talkPage.messageCol.TalkMessageTables[i].Content;
                }
               source.Add(new PersonMessage(newjpg, talkfakeid, content, TimeStamp.GetTime(time)));
            }
            source.Sort((c0, c1) => { return DateTime.Compare(c0.l_time,c1.l_time); });
            Data = talkMessage<PersonMessage>.CreateGroups(source, source.Count);
          }
      
        public List<talkMessage<PersonMessage>> getData()
        {
            return Data;
        }
    }

}
