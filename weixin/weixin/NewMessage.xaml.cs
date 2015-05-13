using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace weixin
{
    public partial class NewMessage : PhoneApplicationPage
    {
        Weixin getMessage;
        public ToastPrompt toast = new ToastPrompt();
        public List<AlphaKeyGroup<newMessageList>> Data;
        public WeixinDataContext weixinDB; // 创建DataContext实例用于用于操作本地的数据库
        public MessageCollection messageCol = new MessageCollection();
        public NewMessage()
        {
            InitializeComponent();
            pb.Visibility = Visibility.Visible;
        }

        #region newMessageList

        public class newMessageList
        {
            public BitmapImage l_imageSource
            {
                get;
                set;
            }
            public string l_nickname
            {
                get;
                set;
            }
            public string l_content
            {
                get;
                set;
            }
            public DateTime l_time
            {
                get;
                set;
            }
             public string l_hasreply
            {
                get;
                set;
            }
             public string l_isstar
             {
                 get;
                 set;
             }
             public string l_fakeid
             {
                 get;
                 set;
             }

            public newMessageList(BitmapImage i, string n, string c, DateTime t,string h,string isstar,string f)
            {
                this.l_imageSource =i ;
                this.l_nickname = n;
                this.l_content = c;
                this.l_time =t;
                this.l_hasreply=h;
                this.l_isstar=isstar;
                this.l_fakeid = f;
            }      
        }

        #endregion

        #region longDataSet

        public void longDataSet()
        {
            try
            {
                string url;
                List<newMessageList> source = new List<newMessageList>();
                string content = "";
                for (int i = 0; i < messageCol.MessageTables.Count; i++)
                {
                    url = loginInfo.UserName + "AllInfo" + "\\ico" + messageCol.MessageTables[i].FakeId + ".jpg";
                    BitmapImage newjpg = new BitmapImage();
                    try
                    {
                        var readstream = Sto.File.OpenFile(url, FileMode.Open, FileAccess.Read);
                        newjpg.SetSource(readstream);
                        //throw(new Exception("www"));
                    }
                    catch
                    {
                        string l = "/design/otherico/getheadimg.png";
                        newjpg.UriSource = new Uri(l, UriKind.Relative);
                        //toast.Message="载入头像出错";
                        //toast.Show();
                    }
                    content = messageCol.MessageTables[i].Content;
                    source.Add(new newMessageList(newjpg,messageCol.MessageTables[i].NickName, content, TimeStamp.GetTime(messageCol.MessageTables[i].Time), messageCol.MessageTables[i].has_Reply, messageCol.MessageTables[i].is_star, messageCol.MessageTables[i].FakeId));
                }
                Data = AlphaKeyGroup<newMessageList>.CreateGroups(source, (newMessageList s) => { return s.l_time; }, (newMessageList s) => { return s.l_isstar; });
            }
            catch (Exception err)
            {
                Global.StoErr("longDataSet", err);
                toast.Message = "读取信息出错,请更新订阅者信息再试";
                toast.Show();
            }
        }

        #endregion

        #region DBDataSet

        public void DBDataSet()
        {
            try
            {
                //连接数据库并初始化DataContext实例
                weixinDB = new WeixinDataContext(WeixinDataContext.DBConnectionString);

                // 使用Linq查询语句查询EmployeeTable表的所有数据
                var messageInDB = from MessageTable message in weixinDB.Messages
                                  where message.ownId == loginInfo.UserName
                                  select message;

                // 将查询的结果返回到页面数据绑定的集合里面
                messageCol.MessageTables = new ObservableCollection<MessageTable>(messageInDB);

                //赋值给当前页面的DataContext用于数据绑定
                this.DataContext = messageCol;
            }
            catch (Exception err)
            {
                Global.StoErr("DBDataSet", err);
                if (err.Message == "Operation not permitted on IsolatedStorageFileStream")
                    toast.Message = "订阅者信息有变化，请及时刷新";
                else
                    toast.Message = "读取信息出错";
                toast.Show();
            }
        }

          #endregion

        #region  导航到事件

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
                     if (!Sto.Info.Contains(loginInfo.UserName + "LaunchTimes"))
                         Sto.stoInfo<int>(loginInfo.UserName + "LaunchTimes", 1);

                     if (Global.isFirstLaunch == 1)
                     {
                              DBDataSet();
                              //longDataSet();
                             // longListSelector.ItemsSource = Data;
                             if (int.Parse(Global.newAddMessage) != 0 || messageCol.MessageTables.Count == 0)
                              {
                                  await Task.Run(() =>
                                   {
                                       string uri = "https://mp.weixin.qq.com/cgi-bin/message?t=message/list&count=10000&day=7&token=" + loginInfo.Token + "&lang=zh_CN&filterivrmsg=1";
                                       string refer = "https://mp.weixin.qq.com/cgi-bin/message?t=message/list&count=10000&day=7&token=" + loginInfo.Token + "&lang=zh_CN";
                                       string host = "mp.weixin.qq.com";
                                       Global.task = "getMessage";
                                       getMessage = new Weixin(new Uri(uri), refer, host, this);
                                       getMessage.GetOperater();
                                   });
                            }
                           else
                           {
                               await Task.Run(() =>
                              {
                                  Deployment.Current.Dispatcher.BeginInvoke(() =>
                                  {
                                      longDataSet();
                                      longListSelector.ItemsSource = Data;
                                  });
                                  Deployment.Current.Dispatcher.BeginInvoke(() =>
                                  {
                                      pb.Visibility = Visibility.Collapsed;
                                  });
                                });
                            }
                     }
                     else
                     {
                         Global.isFirstLaunch = 1;
                         NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
                     }
            base.OnNavigatedTo(e);
        }

        #endregion

        #region 导航到TalkPage

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image ico=sender as Image;
            if(ico!=null)
            this.NavigationService.Navigate(new Uri ("/TalkPage.xaml?fakeid="+ico.Tag.ToString(),UriKind.Relative));
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if(tb!=null)
            this.NavigationService.Navigate(new Uri("/TalkPage.xaml?fakeid=" + tb.Tag.ToString(), UriKind.Relative));
        }

        private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gd = sender as Grid;
            if(gd!=null)
            this.NavigationService.Navigate(new Uri("/TalkPage.xaml?fakeid=" + gd.Tag.ToString(), UriKind.Relative));
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            if(sp!=null)
            this.NavigationService.Navigate(new Uri("/TalkPage.xaml?fakeid=" + sp.Tag.ToString(), UriKind.Relative));
        }

        #endregion

    }
}