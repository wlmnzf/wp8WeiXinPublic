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
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
namespace weixin
{
    public partial class SendGroup : PhoneApplicationPage
    {
        public ToastPrompt toast = new ToastPrompt();

        public SendGroup()
        {
            InitializeComponent();
            string[] sendType = { "文字" };
            listPickerType.ItemsSource=sendType;
            listPickerType.SelectedItem = listPickerType.Items[0];
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Global.isFirstPrepare == true)
            {
                string uri = "https://mp.weixin.qq.com/cgi-bin/masssendpage?t=mass/send&token=" + loginInfo.Token + "&lang=zh_CN";
                string refer = "https://mp.weixin.qq.com/cgi-bin/home?t=home/index&lang=zh_CN&token=" + loginInfo.Token;
                string host = "mp.weixin.qq.com";
                smallHttp sendGroupPost = new smallHttp(new Uri(uri), refer, host, this);
                Global.task = "SendGroup";
                sendGroupPost.GetOperater();
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Global.isFirstLaunch = 0;
        }
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (listPickerType.SelectedItem.ToString() == "文字")
            {
                if (SendBox.Text.Length > 600)
                {
                    this.toast.Message = "字数超过限制！";
                    this.toast.Show();
                    return;
                }
                if (SendBox.Text.Length == 0)
                {
                    this.toast.Message = "内容不能为空！";
                    this.toast.Show();
                    return;
                }
                Random rd = new Random();
                string random = string.Format("{0:N17}", rd.NextDouble().ToString());
                if (String.IsNullOrEmpty(loginInfo.Seq))
                {
                    toast.Message = "operation_seq尚未获得，请稍候";
                    toast.Show();
                    return;
                }
                //性别：0（全部），1（男），2（女）
                //groupid
                //国家：(中文)
                string postdata = "token=" + loginInfo.Token + "&lang=zh_CN&f=json&ajax=1&random=" + random + "&type=1&content=" + SendBox.Text + "&cardlimit=1&sex=0&groupid=" + Global.groupsInfo[listPicker.SelectedItem.ToString()] + "&synctxweibo=" + 0 + "&country=&province=&city=&imgcode=&operation_seq=" + loginInfo.Seq;
                string url = "https://mp.weixin.qq.com/cgi-bin/masssend?t=ajax-response&token=" + loginInfo.Token + "&lang=zh_CN";//请求登录的URL
                string refer = "https://mp.weixin.qq.com/cgi-bin/masssendpage?t=mass/send&token=" + loginInfo.Token + "&lang=zh_CN";
                MassPost sendGroup = new MassPost(postdata, new Uri(url), refer, this);
                //Global.task = "Login";
                sendGroup.PostOperater();
            }
        }




        private void SendBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SendBox.Text.Length > 600)
            {
                this.toast.Message = "字数超过限制。";
                this.toast.Show();
            }
            textCount.Text = SendBox.Text.Length + "/600";
        }

        private void Send_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SelectPicturePage.xaml", UriKind.Relative));
        }

        private void listPickerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPickerType.SelectedItem.ToString() == "图片")
            {
                SendBox.Visibility = Visibility.Collapsed;
                PicturePanel.Visibility = Visibility.Visible;
                SelectPicture.Visibility = Visibility.Visible;
                textCount.Visibility = Visibility.Collapsed;
                //tishi.Visibility = Visibility.Visible;
            }
            else if (listPickerType.SelectedItem.ToString() == "文字")
            {
                SendBox.Visibility = Visibility.Visible;
                PicturePanel.Visibility = Visibility.Collapsed;
                SelectPicture.Visibility = Visibility.Collapsed;
                textCount.Visibility = Visibility.Visible;
               // tishi.Visibility = Visibility.Collapsed;
            }
        }
    }
}