using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Coding4Fun.Toolkit.Controls;
using System.Text.RegularExpressions;
using Microsoft.Phone.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System.Collections;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Info;


namespace weixin
{
    public partial class TalkPage : PhoneApplicationPage
    {
        public string fakeid = "";
        public ToastPrompt toast = new ToastPrompt();
        public PersonMessages pm;
        public  DispatcherTimer tmr=new DispatcherTimer();      
        CookieContainer cc = new CookieContainer();//接收缓存
        public WeixinDataContext weixinDB; // 创建DataContext实例用于用于操作本地的数据库
        public TalkMessageCollection messageCol = new TalkMessageCollection();
        public TalkPage()
        {
            InitializeComponent();
            ab = this.ApplicationBar as ApplicationBar;   
            tmr.Tick+=new EventHandler(refreshPM);
            tmr.Interval = TimeSpan.FromSeconds(2);
        }

        #region 计算最大TalkId

        public long ComputeMax(string url)
        {
            long max=0;
            var read = Sto.File.OpenFile(url,FileMode.Open,FileAccess.Read);
            var readStream = new StreamReader(read);
            string temp = readStream.ReadLine();
            if (temp == null)
            {
               return max;
            }
            max = long.Parse( temp.Split(':')[1]);
            temp = readStream.ReadLine();
            while(temp!=null)
            {
                if (long.Parse(temp.Split(':')[1]) > max)
                    max = long.Parse(temp.Split(':')[1]);    
                temp = readStream.ReadLine();
            }
            readStream.Close();
            return max;
        }

        #endregion

        #region 离开事件

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ab.IsVisible = false;
            Global.isFirstLaunch = 1;
            tmr.Stop();
        }

        #endregion

        #region 后退键

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ab.IsVisible = false;
            base.OnBackKeyPress(e);
        }

        #endregion

        #region DBDataSet(fakeid)

        public void DBDataSet( string fakeid)
        {
            try
            {
                //连接数据库并初始化DataContext实例
                weixinDB = new WeixinDataContext(WeixinDataContext.DBConnectionString);

                // 使用Linq查询语句查询EmployeeTable表的所有数据
                var messageInDB = from TalkMessageTable message in weixinDB.TalkMessages
                                  where message.Username == loginInfo.UserName&&message.Tofakeid==fakeid
                                  select message;

                // 将查询的结果返回到页面数据绑定的集合里面
                messageCol.TalkMessageTables = new ObservableCollection<TalkMessageTable>(messageInDB);

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

        #region 导航到界面事件

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                ab.IsVisible = false;
                tmr.Stop();
                pb.Visibility = Visibility.Visible;
                NavigationContext.QueryString.TryGetValue("fakeid", out fakeid);
                Global.tofakeid = fakeid;

                DBDataSet(Global.tofakeid);
          
                var aFile = new IsolatedStorageFileStream(loginInfo.UserName + "AllInfo" + "\\Info.txt", FileMode.OpenOrCreate, Sto.File);
                StreamReader sr = new StreamReader(aFile);
                string tempString = sr.ReadLine();
                while (tempString != null)//显示昵称
                {
                    var array = tempString.Split(':');
                    if (array[1] == fakeid)
                    {
                        if (array[5] != "")
                            nickname.Text = array[5];
                        else
                            nickname.Text = array[3];
                        break;
                    }
                    tempString = sr.ReadLine();
                }
                sr.Close();

                //  if (messageCol.TalkMessageTables.Count != 0)
                //   {
                //pm = new PersonMessages(this, fakeid);
                //longListSelector.ItemsSource = pm.getData();
                //longListSelector.ScrollTo(longListSelector.ItemsSource[1]);
                //    }

                if (!Sto.Info.Contains(loginInfo.UserName + "MaxTalkId" + fakeid))
                    Sto.stoInfo<long>(loginInfo.UserName + "MaxTalkId"+fakeid, 0);

                string uri = "https://mp.weixin.qq.com/cgi-bin/singlesendpage?tofakeid=" + fakeid + "&t=message/send&action=index&token=" + loginInfo.Token + "&lang=zh_CN";
                string refer = "https://mp.weixin.qq.com/cgi-bin/message?t=message/list&count=20&day=7&token=" + loginInfo.Token + "&lang=zh_CN";
                string host = "mp.weixin.qq.com";
                Weixin personmessage = new Weixin(new Uri(uri), refer, host, this);
                Global.task = "RefreshPersonMessage";
                personmessage.GetOperater();
            }
            catch (Exception err)
            {
                Global.StoErr("TalkPage_OnNaviTo", err);
                toast.Message = "读取信息出错";
                toast.Show();
            }
 }

        #endregion

        private void refreshPM(object sender, EventArgs e)
        {
            string uri = "https://mp.weixin.qq.com/cgi-bin/singlesendpage?tofakeid=" + fakeid + "&t=message/send&action=index&token=" + loginInfo.Token + "&lang=zh_CN";
            string refer = "https://mp.weixin.qq.com/cgi-bin/message?t=message/list&count=20&day=7&token=" + loginInfo.Token + "&lang=zh_CN";
            string host = "mp.weixin.qq.com";
            Weixin personmessage = new Weixin(new Uri(uri), refer, host, this);
            Global.task = "RefreshSpan";
            Global.homeOK = false;
            personmessage.GetOperater();
        }

        private void Item_Loaded(object sender, RoutedEventArgs e)
        {
               Grid item=sender as Grid;
               item.ColumnDefinitions[1].Width = new GridLength(Convert.ToDouble(Application.Current.Host.Content.ActualWidth)-200);
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            state.Text = "正在发送";
            stateSp.Visibility = Visibility.Visible;
            string message = tb.Text;
            if (message == "")
            {
                state.Text = "发送消息为空";
                return;
            }
            await Task.Run(() =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => {  tb.Text = "";   }); 
                Random rd = new Random();
                string random = string.Format("{0:N17}", rd.NextDouble().ToString());
                string sendUrl = "https://mp.weixin.qq.com/cgi-bin/singlesend?t=ajax-response&f=json&token=" + loginInfo.Token + "&lang=zh_CN";
                string sendData = "token=" + loginInfo.Token + "&lang=zh_CN&f=json&ajax=1&random=" + random + "&type=1&content=" + message + "&tofakeid=" + fakeid + "&imgcode=";
                string sendRefer = "https://mp.weixin.qq.com/cgi-bin/singlesendpage?tofakeid=" + fakeid + "&t=message/send&action=index&token=" + loginInfo.Token + "&lang=zh_CN";
                smallPost sendInfo = new smallPost(sendData, new Uri(sendUrl), sendRefer, this);
                Global.send = true;  
                sendInfo.PostOperater();
            });
        }

        #region 隐藏ab

        private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ab.IsVisible = true;
        }

        private void longListSelector_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ab.IsVisible = false;
        }

        private void nickname_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ab.IsVisible = false;
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ab.IsVisible = false;
        }

        private void tb_MouseMove(object sender, MouseEventArgs e)
        {
            ab.IsVisible = false;
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        { 
            ab.IsVisible = false;
        }

        #endregion

        private void longListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ApplicationBarIconButton_Click
        }

    }
   
}