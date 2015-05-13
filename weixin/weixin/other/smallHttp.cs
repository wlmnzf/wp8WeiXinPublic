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
using System.IO.IsolatedStorage;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Net.NetworkInformation;

namespace weixin
{
    class smallHttp
    {
         Uri uri;
        string refer;
        string host;
        CookieContainer cc=new CookieContainer();//接收缓存
        AllPeopleInfo allPeopleInfo;
        SendGroup sendGroup;

        #region 构造函数

        public smallHttp()
        { }
   
        public smallHttp(Uri u, string r, string h, AllPeopleInfo t)
        {
            uri = u;
            refer = r;
            host = h;
            allPeopleInfo = t;
        }
        public smallHttp(Uri u, string r, string h)
        {
            uri = u;
            refer = r;
            host = h;
        }

         public smallHttp(Uri u, string r, string h,SendGroup s)
        {
            uri = u;
            refer = r;
            host = h;
            sendGroup = s;
        }

        #endregion

        #region 请求流

        public void GetOperater ()
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                if (loginInfo.LoginCookie != null)
                    webRequest.CookieContainer = loginInfo.LoginCookie;
                else
                    webRequest.CookieContainer = cc;                                      //保存cookie  

                webRequest.Method = "GET";
               // webRequest.Headers["Accept-Encoding"] = "gzip,deflate";
                webRequest.Headers["Accept-Language"] = "zh-CN";
                webRequest.Headers["Connection"] = "keep-alive";
                webRequest.Headers["Referer"] = refer;
                webRequest.Headers["Host"] = host;
                webRequest.Headers["DNT"] = "1";
                webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                webRequest.AllowAutoRedirect = true;
                webRequest.Headers["Referer"] = refer;
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.AllowReadStreamBuffering = true;
               
                IAsyncResult ResponseResult = (IAsyncResult)webRequest.BeginGetResponse(new AsyncCallback(ResponseReady), webRequest);
            }
            catch (Exception err)
            {
                Global.StoErr("GetOperate", err);
                allPeopleInfo.state.Text = "请求超时，请检查网络状况";
            }
  }

        #endregion

        #region 获取图片

        public void getImage(string image_uri,string refer)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(image_uri);
                webRequest.Accept = "image/webp,*/*;q=0.8";
                if (loginInfo.LoginCookie != null)
                    webRequest.CookieContainer = loginInfo.LoginCookie;
                else
                    webRequest.CookieContainer = cc;                                      //保存cookie  

                webRequest.AllowWriteStreamBuffering = true;
                webRequest.AllowReadStreamBuffering = true;

                webRequest.Method = "GET";                                          
                webRequest.AllowAutoRedirect = true;
                webRequest.Headers["Accept-Encoding"] = "gzip,deflate";
                webRequest.Headers["Accept-Language"] = "zh-CN";
                webRequest.Headers["Connection"] = "keep-alive";
                webRequest.Headers["Referer"] = refer;
                webRequest.Headers["Host"] = "mp.weixin.qq.com";
                webRequest.Headers["DNT"] = "1";
                webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                IAsyncResult ImageResult = (IAsyncResult)webRequest.BeginGetResponse(new AsyncCallback(ImageReady), webRequest);
            }
            catch 
            {
                allPeopleInfo.state.Text = "连接服务器超时，请检查网络状况";
            }
        }

        public void ImageReady(IAsyncResult ImageResult)
        { 
           string fakeid="";
           string ExceptionReferer= "";
            try
            {
                HttpWebRequest request = ImageResult.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ImageResult);
                ExceptionReferer= response.Headers["Referer"];
                using (Stream stream = response.GetResponseStream())
                {
                    string tempstr = response.ResponseUri.ToString();
                    string pattern;

                    pattern = "fakeid=(?<fakeid>[\\d]*)";
                    var m = Regex.Match(tempstr, pattern);
                     fakeid = m.Groups["fakeid"].Value;
                    if (!Sto.File.DirectoryExists(loginInfo.UserName + "AllInfo"))
                    {
                        Sto.File.CreateDirectory(loginInfo.UserName + "AllInfo");
                    }
                    var outStream = Sto.File.OpenFile(loginInfo.UserName + "AllInfo" + "\\ico" + fakeid + ".jpg", FileMode.Create, FileAccess.Write);

                    Int32 i = 0;
                    //循环inStream，将内容写进outStream
                    while (true)
                    {
                        i = stream.ReadByte();
                        if (i != -1)
                        {
                            outStream.WriteByte((Byte)i);
                        }
                        else
                        {
                            break;
                        }
                    }
                    outStream.Close();
                    Global.cnt++;
                    Deployment.Current.Dispatcher.BeginInvoke(() => { Global.pb += Global.perPerson; allPeopleInfo.pb.Value = Global.pb; });
                }
            }
            catch(Exception err)
            {
                Global.StoErr("SmallPost", err);
                Global.failedFakeId.Add(fakeid);
                Global.failedReferer.Add(ExceptionReferer);
                if (Sto.File.FileExists(loginInfo.UserName + "AllInfo" + "\\ico" + fakeid + ".jpg"))
                {
                    Sto.File.DeleteFile(loginInfo.UserName + "AllInfo" + "\\ico" + fakeid + ".jpg");
                }
                Deployment.Current.Dispatcher.BeginInvoke(() => { allPeopleInfo.state.Text = "连接服务器超时，请检查网络状况,再次登陆时请点击新增人数，再次同步";});
            }
        }

        #endregion

        #region 接收流

        public void ResponseReady(IAsyncResult ResponseResult)
        {
            try
            {
                HttpWebRequest request = ResponseResult.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ResponseResult);
                StreamReader sr2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string text2 = sr2.ReadToEnd();

                if (Global.task == "GetPage")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (!Sto.File.DirectoryExists(loginInfo.UserName + "AllInfo"))
                        {
                            Sto.File.CreateDirectory(loginInfo.UserName + "AllInfo");
                        }
                        string pattern;
                        pattern = "\"id\":(?<id>[\\d]*),\"nick_name\":\"(?<nickname>[^\"]*)\",\"remark_name\":\"(?<remarkname>[^\"]*)\",\"group_id\":(?<groupid>[\\d]*)";
                        var ms = Regex.Matches(text2, pattern);

                        string temppagesize = response.ResponseUri.ToString();
                        pattern = "pagesize=(?<pagesize>[\\d]*)";
                        var m = Regex.Match(temppagesize, pattern);
                        temppagesize = m.Groups["pagesize"].Value;
                        Global.tempSize = temppagesize;

                        var aFile = new IsolatedStorageFileStream(loginInfo.UserName + "AllInfo" + "\\Info.txt", FileMode.Append, Sto.File);
                        StreamWriter sw = new StreamWriter(aFile);
                        foreach (Match match in ms)
                        {
                            try
                            {
                                if (!Sto.File.FileExists(loginInfo.UserName + "AllInfo" + "\\ico" + match.Groups["id"].Value + ".jpg"))
                                {
                                    sw.WriteLine("fakeid:" + match.Groups["id"].Value + ":nickname:" + match.Groups["nickname"] + ":remarkname:" + match.Groups["remarkname"].Value + ":grupid:" + match.Groups["groupid"].Value);
                                    string tempUri = "https://mp.weixin.qq.com/misc/getheadimg?fakeid=" + match.Groups["id"].Value + "&token=" + loginInfo.Token + "&lang=zh_CN";
                                    string tempRefer = "https://mp.weixin.qq.com/cgi-bin/contactmanage?t=user/index&pagesize=" + temppagesize + "&pageidx=0&type=0&groupid=" + match.Groups["groupid"].Value + "&token=" + loginInfo.Token + "&lang=zh_CN";
                                    Global.jpgName = "allico";
                                    getImage(tempUri, tempRefer);
                                }
                                else
                                {
                                    Global.cnt++;
                                    Global.pb += Global.perPerson; allPeopleInfo.pb.Value = Global.pb;
                                }
                            }
                            catch (Exception err)
                            {
                                Global.StoErr("GetPage", err);
                                allPeopleInfo.state.Text = "写入资料失败";
                                sw.Close();
                            }
                        }
                        sw.Close();
                    }
                 );
              }

                else if (Global.task == "SendGroup")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        string temp;
                        string pattern;
                        pattern = " data:{(?<data>[^}]*)},";
                        var m = Regex.Match(text2,pattern);
                        temp = m.Groups["data"].Value.ToString();

                        pattern = "ticket:\"(?<ticket>[^\"]*)\"";
                         m = Regex.Match(temp,pattern);
                        loginInfo.Ticket = m.Groups["ticket"].Value.ToString();

                        pattern = " user_name:\"(?<username>[^\"]*)\"";
                        m = Regex.Match(temp,pattern);
                        loginInfo.UniformUserName = m.Groups["username"].Value.ToString();

                        pattern = "wx.cgiData = {(?<info>[\\S\\s]*)seajs.use";   //可以匹配更多信息
                         m = Regex.Match(text2, pattern);
                        temp = m.Groups["info"].Value.ToString();

                        pattern = "operation_seq: \"(?<seq>[\\d]*)\"";
                         m = Regex.Match(temp, pattern);
                        loginInfo.Seq = m.Groups["seq"].Value.ToString();

                        pattern = "\"id\":(?<id>[\\d]*),\"name\":\"(?<name>[^\"]*)\",\"cnt\":(?<cnt>[\\d]*)";
                        var ms = Regex.Matches(temp,pattern);
                        Global.groupsInfo = new Dictionary<string, string>();
                        Global.groupsInfo.Add("全部","-1");
                        foreach(Match a in ms)
                        {
                            Global.groupsInfo.Add(a.Groups["name"].Value.ToString() + " " + a.Groups["cnt"].Value.ToString()+"人",a.Groups["id"].Value.ToString());
                        }
                        sendGroup.listPicker.ItemsSource = Global.groupsInfo.Keys;
                        sendGroup.listPicker.SelectedItem = sendGroup.listPicker.Items[0];
                        sendGroup.pb.Visibility = Visibility.Collapsed;
                        sendGroup.PreparingText.Visibility = Visibility.Collapsed;
                        sendGroup.SendBox.IsEnabled = true;
                        Global.isFirstPrepare = false;
                    }
                    );
                }

          }
            catch(Exception err)
            {
                Global.StoErr("smallHttp-GetInfo", err);
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                   {
                       allPeopleInfo.state.Text = "写入资料失败";
                   });
            }
        }

        #endregion
    }
}
