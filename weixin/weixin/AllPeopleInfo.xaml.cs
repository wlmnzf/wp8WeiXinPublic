using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using System.ComponentModel;
using Microsoft.Phone.BackgroundTransfer;
using System.IO.IsolatedStorage;

namespace weixin
{
    public partial class AllPeopleInfo : PhoneApplicationPage
    {
        /*前台wifi请求，按页来进行进度条计算，然后存进独立储存，*/
       
       public BackgroundWorker backgroundWorker;

        public AllPeopleInfo()
        {
            InitializeComponent();
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;//屏蔽自动锁屏
        }
      
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { 
            base.OnNavigatedTo(e);
        }

        #region 进度条操作
        void usePB()
        {
            //创建一个后台处理类的对象
            backgroundWorker = new BackgroundWorker();
            //调用 RunWorkerAsync后台操作时引发此事件，即后台要处理的事情写在这个事件里面
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            //当后台操作完成事件
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            //当处理进度（ReportProgress）被激活后，进度的改变将触发ProgressChanged事件
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            //设置为可报告进度情况的后台处理
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerAsync();
        }
        //进度改变处理
        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //把进度改变的值赋值给progressBar1进度条的值
                pb.Value = e.ProgressPercentage;
            }
         );
        }
        //后台操作完成
        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //隐藏进度条
                pb.Visibility = System.Windows.Visibility.Visible;
            }
            );

        }
#endregion

        #region 进度条后台工作者启动

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //这里还要添加删除所有信息的代码
            string uri = "https://mp.weixin.qq.com/cgi-bin/contactmanage?t=user/index&pagesize=10&pageidx=0&type=0&groupid=0&token="+loginInfo.Token+"&lang=zh_CN";
            string refer = "https://mp.weixin.qq.com/cgi-bin/home?t=home/index&lang=zh_CN&token="+loginInfo.Token;
            string host = "mp.weixin.qq.com";
            Weixin getAllInfo = new Weixin(new Uri(uri), refer, host, this);
            Global.task = "GetAllPeopleInfo";
            changePB(5);
            Global.test = false;
            getAllInfo.GetOperater();
            while (!Global.test) ;
            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //  {
                   findInfo();
               //});   
        }

        #endregion

        #region 改变进度

        public void changePB(int i)
        {
            backgroundWorker.ReportProgress(i);
        }

        #endregion

        #region 下载信息

        public void findInfo()
        {
            try
            {
                string[] groupid = (string[])Sto.Info[loginInfo.UserName + "groupList"];
                string[] name = (string[])Sto.Info[loginInfo.UserName + "nameList"];
                string[] cnt = (string[])Sto.Info[loginInfo.UserName + "cntList"];
                for (int i = 0; i < groupid.Length; i++)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => { state.Text = "正在处理  " + name[i] + "：" + cnt[i] + "人"; });
                    Uri tempuri = new Uri("https://mp.weixin.qq.com/cgi-bin/contactmanage?t=user/index&pagesize=" + cnt[i] + "&pageidx=0&type=0&groupid=" + groupid[i] + "&token=" + loginInfo.Token + "&lang=zh_CN", UriKind.RelativeOrAbsolute);
                    string temprefer = "https://mp.weixin.qq.com/cgi-bin/contactmanage?t=user/index&pagesize=" + cnt[i] + "&pageidx=0&type=0&groupid=" + groupid[i] + "&token=" + loginInfo.Token + "&lang=zh_CN";
                    string temphost = "mp.weixin.qq.com";
                    Global.task = "GetPage";
                    Global.cnt = 0;
                    Sto.stoInfo<string>("groupid", groupid[i]);
                    smallHttp tempget = new smallHttp(tempuri, temprefer, temphost, this);
                    tempget.GetOperater();
                    while (Global.cnt < int.Parse(cnt[i])) ;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        pb.Value = Global.pb;
                    });
                }
                if (Global.failedFakeId.Count != 0)
                {
                    state.Text = "正在尝试重新获得失败的图片，请稍候";
                    foreach (string fakeid in Global.failedFakeId)
                    {
                        try
                        {
                            int i = Global.failedFakeId.IndexOf(fakeid);
                            string tempUri = "https://mp.weixin.qq.com/misc/getheadimg?fakeid=" + fakeid + "&token=" + loginInfo.Token + "&lang=zh_CN";
                            string tempRefer = Global.failedReferer[i];
                            smallHttp getFailedIco = new smallHttp();
                            getFailedIco.getImage(tempUri, tempRefer);
                        }
                        catch
                        {
                            state.Text = "请求"+fakeid+"失败";
                        }
                    }
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    pb.Value = 100; state.Text = "完成";
                    begin.Content = "继续";
                    //82,376,0,0
                    begin.Margin = new Thickness(82, 376, 0, 0);
                    begin.Visibility = Visibility.Visible;
                });
            }
            catch (Exception err)
            {
                Global.StoErr("NewMessage", err);
            }
        }

#endregion

        #region 按钮事件

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (begin.Content.ToString() == "开始")
            {
                state.Visibility = Visibility.Visible;
                pb.Visibility = Visibility.Visible;
                tishi.Visibility = Visibility.Collapsed;
                begin.Visibility = Visibility.Collapsed;
                usePB();
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/HomePage.xaml",UriKind.Relative));
            }
        }

        #endregion

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Sto.File.DirectoryExists(loginInfo.UserName + "AllInfo"))
                {
                    Sto.deleteFile(loginInfo.UserName + "AllInfo");
                    Sto.File.DeleteDirectory(loginInfo.UserName + "AllInfo");
                }
                MessageBox.Show("清理成功！");
            }
            catch
            {}
        }

    }

  

}