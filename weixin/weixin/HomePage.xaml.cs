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
using System.IO;
using System.Windows.Resources;
using System.Windows.Threading;

namespace weixin
{
    public partial class HomePage : PhoneApplicationPage
    {
        public ToastPrompt toast = new ToastPrompt();
        public DispatcherTimer tmr = new DispatcherTimer();      

        public HomePage()
        {
            InitializeComponent();
            tmr.Tick += new EventHandler(NoticeCollapsed);
            tmr.Interval = TimeSpan.FromSeconds(1);
        }

        private void NoticeCollapsed(object sender, EventArgs e)
        {
            Notice.Visibility = Visibility.Collapsed;
            tmr.Stop();
        }

      protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            #region  GetHome

            pb.Visibility = Visibility.Visible;
            //Global.homeOK = false;
            string uri = "https://mp.weixin.qq.com"+Global.returnUrl;
            string refer = "https://mp.weixin.qq.com/";
            string host = "mp.weixin.qq.com";
            Weixin getHomeInfo = new Weixin(new Uri(uri), refer, host, this);
            Global.task = "GetHome";
            getHomeInfo.GetOperater();
            base.OnNavigatedTo(e);

            #endregion
        }

      void ShowAllPeopleInfo()
      {
          MessageBox.Show("检测到你有新的订阅者，您必须更新订阅者信息以便程序正常工作");
          Global.hasNewPeople = false;
          this.NavigationService.Navigate(new Uri("/AllPeopleInfo.xaml", UriKind.Relative));
      }
            #region 按钮事件

      private void newMessage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Global.hasNewPeople == true)
            {
                ShowAllPeopleInfo();
                return;
            }
            Global.isFirstLaunch = 1;
            if (Global.isFirstLoad == true && Global.homeOK == false)
            {
                Notice.Visibility = Visibility.Visible;
                tmr.Start();
                return;
            }
            this.NavigationService.Navigate(new Uri("/NewMessage.xaml", UriKind.Relative));
        }

        private void AllPeoPle(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Global.isFirstLoad == true && Global.homeOK == false)
            {
                Notice.Visibility = Visibility.Visible;
                tmr.Start();
                return;
            }
            this.NavigationService.Navigate(new Uri("/allPeople.xaml", UriKind.Relative));
        }

        private void newPeople(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Global.hasNewPeople == true)
                ShowAllPeopleInfo();
            if (Global.isFirstLoad == true && Global.homeOK == false)
            {
                Notice.Visibility = Visibility.Visible;
                tmr.Start();
                return;
            }
            this.NavigationService.Navigate(new Uri("/AllPeopleInfo.xaml", UriKind.Relative));
        }

        #endregion
    }
}