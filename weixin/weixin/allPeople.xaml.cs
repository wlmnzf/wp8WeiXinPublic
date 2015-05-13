using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using Coding4Fun.Toolkit.Controls;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace weixin
{
    public partial class allPeople : PhoneApplicationPage
    {
        public ToastPrompt toast = new ToastPrompt();
        public List<AllPeoPle<allpeople>> Data;
        public allPeople()
        {
            InitializeComponent();
        }

        #region 进入界面事件

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Global.isFirstLaunch == 0)
            {
                Global.isFirstLaunch = 1;
                return;
            }
            pb.Visibility = Visibility.Visible;
            await Task.Run(() =>
           {
               Deployment.Current.Dispatcher.BeginInvoke(() =>
               {
                  // pb.IsIndeterminate = true;
                   try
                    {
                      string url;
                      List<allpeople> source = new List<allpeople>();
                     try
                     {
                         var aFile = new IsolatedStorageFileStream(loginInfo.UserName + "AllInfo" + "\\Info.txt", FileMode.OpenOrCreate, Sto.File);
                         StreamReader sr = new StreamReader(aFile);
                         string tempString = sr.ReadLine();
                         while (tempString != null)
                          {
                            var array = tempString.Split(':');
                            url = loginInfo.UserName + "AllInfo" + "\\ico" + array[1] + ".jpg";
                            //if (!Sto.File.FileExists(url))
                            //{
                            //  //  MessageBox.Show("飒沓");
                            //    continue;
                            //}
                            var readstream = Sto.File.OpenFile(url, FileMode.Open, FileAccess.Read);
                              BitmapImage newjpg = new BitmapImage();
                            try
                            {
                                newjpg.SetSource(readstream);
                             } 
                            catch(Exception err)
                            {
                                string l = "/design/otherico/getheadimg.png";
                                newjpg.UriSource = new Uri(l,UriKind.Relative);
                                //readstream = Sto.File.OpenFile(l, FileMode.Open, FileAccess.Read);
                                //newjpg.SetSource(readstream);
                                //toast.Message = "黑名单用户无法下载到头像";
                               // toast.Show();
                            }
                                source.Add(new allpeople(newjpg, array[3], array[5], array[7], array[1]));
                         
                           
                            readstream.Close();
                            tempString = sr.ReadLine();
                        }
                        sr.Close();
                    }
                   catch (Exception err)
                   {
                         Global.StoErr("allPeople", err);
                          toast.Message = "读取资料出错，请重试";
                          toast.Show();
                   }
                   Data = AllPeoPle<allpeople>.CreateGroups(source, (allpeople s) => { return s.l_groupid; });
                    }
                   catch (Exception err)
                   {
                       Global.StoErr("allPeople", err);
                       toast.Message = "读取资料出错，请重试";
                       toast.Show();
                   }
                    longListSelector.ItemsSource = Data;
                    pb.Visibility = Visibility.Collapsed;
                 }); 
           });
        }

        #endregion

        #region allpeople属性

        public class allpeople
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
            public string l_remarkname
            {
                get;
                set;
            }
            public string l_groupid
            {
                get;
                set;
            }
            public string l_fakeid
            {
                get;
                set;
            }

            public allpeople(BitmapImage i, string n,  string r,string g,string f)
            {
                this.l_imageSource = i;
                this.l_nickname = n;
                this.l_remarkname = r;
                this.l_groupid=g;
                this.l_fakeid = f;
            }
        }

        #endregion

        #region 跳转TalkPage

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image ico = sender as Image;
            this.NavigationService.Navigate(new Uri("/TalkPage.xaml?fakeid=" + ico.Tag.ToString(), UriKind.Relative));
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            this.NavigationService.Navigate(new Uri("/TalkPage.xaml?fakeid=" + tb.Tag.ToString(), UriKind.Relative));
        }

        #endregion

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            Global.isFirstPrepare = true;
            this.NavigationService.Navigate(new Uri("/SendGroup.xaml",UriKind.Relative));
        }

        private void longListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}