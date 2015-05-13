using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using weixin.Resources;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Windows.Media;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Tasks;



namespace weixin
{
    public partial class MainPage : PhoneApplicationPage
    {
        Weixin Login;
        public ToastPrompt toast = new ToastPrompt();

        public MainPage()
        {
            InitializeComponent();
        }

           #region   MD5加密
        public static string GetMd5Str32(string str) //MD5摘要算法
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            // Convert the input string to a byte array and compute the hash.  
            char[] temp = str.ToCharArray();
            byte[] buf = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                buf[i] = (byte)temp[i];
            }
            byte[] data = md5Hasher.ComputeHash(buf);
            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data   
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.  
            return sBuilder.ToString();
        }
        #endregion   MD5加密
      
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            #region 异常日志
            if (!Sto.Info.Contains("Err"))
              {
                   Sto.stoInfo<string>("Err", "问题日志："+'\n');
                   Sto.stoInfo<int>("hasErr",0);
              }

            if(!Sto.Info.Contains("ShouldReply"))
                  Sto.stoInfo<int>("ShouldReply",0);

            //触发异常日志的条件：开关为开或者登陆次数达到30次
            //选择是否发送异常日志
            if (Sto.Info.Contains("hasErr") && Sto.Info["hasErr"].ToString() == "1" && (Sto.Info["ReplyBar"].ToString() == "1" || Sto.Info["ShouldReply"].ToString() == "1"))
              {
                  MessageBoxResult RESULT = MessageBox.Show("检测到你有未处理的异常，是否发送给开发者", "提示", MessageBoxButton.OKCancel);
                 
                if (RESULT == MessageBoxResult.OK)
                  {
                        EmailComposeTask email = new EmailComposeTask();
                        email.To = "wlmnzf@hotmail.com";
                        email.Subject = "异常反馈";
                        email.Body = "机型：" + '\n' + "系统版本：" + '\n' + Sto.Info["Err"];
                        email.Show();
                        Global.DelErr();
                        Sto.stoInfo<int>("ShouldReply",0);
                  }
                  else
                  {
                       //Sto.stoInfo<int>("hasErr", 0);
                      Sto.stoInfo<int>("ShouldReply", 0);
                      Sto.stoInfo<int>("ReplyLaunchTime", 0);
                  }
             }
           else
           {
                //如果没开开关同时计数次数不达标，计数+1，如果计数达标则设标记为1
                  if (Sto.Info.Contains("hasErr") && Sto.Info["hasErr"].ToString() == "1")
                  {
                      if (!Sto.Info.Contains("ReplyLaunchTime"))
                          Sto.stoInfo<int>("ReplyLaunchTime", 0);

                      if (Sto.Info["ReplyLaunchTime"].ToString() == "100")
                      {
                          Sto.stoInfo<int>("ReplyLaunchTime", 0);
                          Sto.stoInfo<int>("ShouldReply", 1);
                      }
                      else
                      {
                          Sto.Info["ReplyLaunchTime"] = Convert.ToInt32(Sto.Info["ReplyLaunchTime"]) + 1;
                          Sto.stoInfo<int>("ShouldReply", 0);
                          Sto.Info.Save();
                      }
                  }
           }
        #endregion 异常日记

            #region  填写帐号密码
            if (Sto.Info.Contains("acc"))
              {
                  acc.Text = Sto.Info["acc"].ToString();
                  pass.Password= Sto.Info["pass"].ToString();
              }
           #endregion 填写帐号密码
 
            #region 更新说明
            if (Sto.Info.Contains("All_Messages_Num")||Sto.File.DirectoryExists(acc.Text+"\\PersonalMessages")||Sto.Info.Contains(acc.Text + "isFirstLogin") && Sto.Info[acc.Text + "isFirstLogin"].ToString()=="0")
              {
                  MessageBox.Show("检测到你尚存在上个版本遗留的问题,我们将清理您的账户信息以便修复");
                  Clear.IsChecked = true;
              }

              if (!Sto.Info.Contains("Vertion")||Sto.Info["Vertion"].ToString()!="4.1")
              {
                  Global.isFirstSetupOrUpdate = 1;
                  var result=MessageBox.Show("说明："+'\n'+"本次更新："+'\n'+"因wp8和wp8.1 api好大一部分都换了，8.1版本就顺便重新做了，做的略慢，不过最近开始进入状态了，新出一个未完成版本吧，各位赶紧来提新功能"+'\n'+"点击确定进入下载","提示",MessageBoxButton.OKCancel);
                  if (result == MessageBoxResult.OK)
                  {
                      await Windows.System.Launcher.LaunchUriAsync(new Uri("zune:navigate?appid=86136201-8889-459e-9103-41c09031ec09"));
                  }
                  Sto.stoInfo<string>("Vertion", "4.1");
              }
              #endregion 更新说明

              base.OnNavigatedTo(e);
            }

            #region  创建文件（暂时不用）
        public void CreateFile(string username)
        {
            if (!Sto.Info.Contains(username))
            for (int i = 0;i<Sto.Info.Values.Count ; i++)
            { 
                int flag=1;
                foreach (string a in Sto.Info.Values)
                    if (a == "PersonalFiles" + i.ToString())
                    { flag = 0; break; }

                if (flag == 1)
                    Sto.stoInfo<string>(username,"PersonalFiles"+i);
            }
        }
        #endregion  创建文件（暂时不用）

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region 清理资料
            if (Clear.IsChecked == true)
            {
                string[] dir = Sto.File.GetDirectoryNames();
                if (dir.Length != 0)
                {
                    for (int i = 0; i < dir.Length; i++)
                    {
                        if (dir[i].Contains(acc.Text))
                         {
                             Sto.deleteFile(dir[i]);
                             Sto.File.DeleteDirectory(dir[i]);
                         }
                    }
                }

                if (Sto.File.FileExists("Weixin.sdf"))
                {
                    try
                    {
                        Sto.File.DeleteFile("Weixin.sdf");
                        using (WeixinDataContext db = new WeixinDataContext(WeixinDataContext.DBConnectionString))
                        {
                            if (db.DatabaseExists() == false)
                            {
                                //创建一个数据库
                                db.CreateDatabase();
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        state.Text = "数据库无法清理,请重新打开该软件,再勾选反馈中的清理重试";
                    }
                }

                if (Sto.Info.Keys.Count != 0)
                {
                    string[] ks = new string[Sto.Info.Keys.Count];
                    int j= 0;
                    foreach (var key in Sto.Info.Keys)
                    {
                        ks[j] = key.ToString();
                        j++;
                    }

                    for (int i = 0; i < ks.Length;i++)
                    {
                        if (ks[i].Contains(acc.Text))
                           Sto.Info.Remove(ks[i]);
                    }
                    if (Sto.Info.Contains("ReplyBar"))
                        Sto.Info.Remove("ReplyBar");

                     if (Sto.Info.Contains("All_Messages_Num"))
                        Sto.Info.Remove("All_Messages_Num");
                }
            }
            #endregion

            #region 保存toggle开关状态
            if (toggle.IsChecked == true)
                Sto.stoInfo<int>("ReplyBar", 1);
            else
                Sto.stoInfo<int>("ReplyBar", 0);
            #endregion 保存toggle开关状态

            #region 判断验证码是否为空
            if (Global.hasVCode == 1 && code.Text == "")
            {
                state.Text = "请输入验证码";
                MessageBox.Show("验证码不能为空");
                return;
            }
            #endregion

            #region  清空验证码缓存
            if (Global.launchTimes == 0 && Sto.File.DirectoryExists("VC")&&Global.hasVCode==0)
            {
                try
                {
                    Sto.deleteFile("VC");
                    if (Sto.File.DirectoryExists("VC"))
                        Sto.File.DeleteDirectory("VC");
                    Global.launchTimes++;
                }
                catch (Exception err)
                {
                    Global.StoErr("MainPage", err);
                }
            }
            #endregion 清空验证码缓存

            #region  登陆配置
            state.Text = "正在登录，请稍候";
            state.Visibility = Visibility.Visible;
            pb.Visibility = Visibility.Visible;
            loginInfo.UserName = acc.Text;
            string tmpPassword = pass.Password;
            if (pass.Password.Length > 16) tmpPassword = pass.Password.Substring(0, 16);
            string password = MD5.GetMd5String(tmpPassword);
            //string password = GetMd5Str32(pass.Password).ToUpper();
            string postdata = "username=" + acc.Text +"&pwd=" + password+ "&imgcode="+code.Text+"&f=json";
            string url = "https://mp.weixin.qq.com/cgi-bin/login?lang=zh_CN ";//请求登录的URL
            string refer="https://mp.weixin.qq.com/";
             Login=new Weixin(postdata,new Uri(url),refer,this);
            Global.task = "Login";
            Login.PostOperater();
            #endregion 登陆配置
        }
        
           #region  帐号密码验证码显示效果
        private void acc_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //acc.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 191, 19, 2));//#BF131313
            acc.Foreground =new SolidColorBrush(Colors.Black);
        }

        private void acc_LostFocus(object sender, RoutedEventArgs e)
        {
            acc.Foreground = new SolidColorBrush(Colors.White);
        }

        private void pass_LostFocus(object sender, RoutedEventArgs e)
        {
            pass.Foreground = new SolidColorBrush(Colors.White);
        }

        private void pass_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            pass.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void code_LostFocus(object sender, RoutedEventArgs e)
        {
            code.Foreground = new SolidColorBrush(Colors.White);
        }

        private void code_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            code.Foreground = new SolidColorBrush(Colors.Black);
        }

        #endregion 帐号密码验证码显示效果

           #region 显示或隐藏验证码的动画
        public void showVCode()
        {
            for (int i = 298; i <= 400; i++)
            {
                login.Margin = new Thickness(57, i, 0, 0);
            }
            vc.Visibility = Visibility.Visible;
            code.Visibility = Visibility.Visible;
            Global.hasVCode = 1;
        }

        public void hideVCode()
        {
            vc.Visibility = Visibility.Collapsed;
            code.Visibility = Visibility.Collapsed;
            for (int i = 400; i >= 298; i--)
            {
                login.Margin = new Thickness(57, i, 0, 0);
            }
            Global.hasVCode = 0;
        }
        #endregion 显示或隐藏验证码的动画

           #region 刷新验证码
        private void vc_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {  
           //刷新验证码
            Login.VCodeWrong("-8");
        }
        #endregion 刷新验证码

          #region 反馈按钮事件
        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //sp指的是反馈设置的面板
            sp.Visibility=Visibility.Visible;
            sp.Margin = new Thickness(-92, -240, -68, 0);
            for (int i = -240; i <= 86; i += 1)
            {
                sp.Margin = new Thickness(-92, i, -68, 0);
            }
        }
        #endregion 反馈按钮事件

          #region 异常反馈事件
        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "wlmnzf@hotmail.com";
            email.Subject = "异常反馈";
            email.Body = "机型：" + '\n' + "系统版本：" + '\n' + Sto.Info["Err"];
            try
            {
                email.Show();
            }
            catch
            {
                Sto.stoInfo<string>("Err", "");
                email.Show();
            }
            Global.DelErr();
        }
        #endregion 异常反馈事件

          #region 意见反馈事件
        private void TextBlock1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "wlmnzf@hotmail.com";
            email.Subject = "建议";
            email.Body = "机型：" + '\n' + "系统版本：" + '\n' +"建议：";
            email.Show();
        }
        #endregion 意见反馈事件

          #region 棒形按钮开关事件
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (toggle.IsChecked==true)
                Sto.stoInfo<int>("ReplyBar", 1);
            else
                Sto.stoInfo<int>("ReplyBar", 0);
           
            sp.Visibility = Visibility.Collapsed;
        }
        #endregion 棒形按钮开关时事件

          #region 反馈面板load事件
        private void sp_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Sto.Info.Contains("ReplyBar"))
                Sto.stoInfo<int>("ReplyBar",0);
            if (Sto.Info["ReplyBar"].ToString() == "1")
                toggle.IsChecked = true;
            else
               toggle.IsChecked = false;
        }
        #endregion 反馈面板load事件
    }
}
