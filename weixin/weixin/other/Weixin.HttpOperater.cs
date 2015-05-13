using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;


namespace weixin
{
    public class Weixin 
    {
        Uri uri;
        string refer;
        string Data;
        string host;
        byte[] byteArray;
        CookieContainer cc=new CookieContainer();//接收缓存
        MainPage that;
        HomePage home;
        NewMessage newMessage;
        AllPeopleInfo allPeopleInfo;
        TalkPage talkPage;
        SendGroup sendGroup;

        #region 构造函数

        public Weixin(string d,Uri u,string r ,TalkPage t)
        {
            Data=d;
            uri=u;
            refer=r;
            talkPage=t;    
        }
        public Weixin(string d, Uri u, string r,MainPage t)
        {
            Data = d;
            uri = u;
            refer = r;
             that = t;
        }
        public Weixin( Uri u, string r,string h,HomePage t)
        {
            uri = u;
            refer = r;
            host = h;
            home = t;
        }
        public Weixin(Uri u, string r, string h, TalkPage t)
        {
            uri = u;
            refer = r;
            host = h;
            talkPage = t;
        }
        public Weixin(Uri u, string r, string h, NewMessage t)
        {
            uri = u;
            refer = r;
            host = h;
            newMessage = t;
        }
        public Weixin(Uri u, string r, string h, AllPeopleInfo t)
        {
            uri = u;
            refer = r;
            host = h;
            allPeopleInfo = t;
        }
        public Weixin(Uri u, string r, string h)
        {
            uri = u;
            refer = r;
            host = h;
        }

        public Weixin(Uri u, string r, string h,SendGroup s)
        {
            uri = u;
            refer = r;
            host = h;
            sendGroup = s;
        }

        #endregion

        #region 显示cookie

        public string showCookie(CookieContainer ccTemp,Uri uriTemp)
        {
             string t="";
             var cks = ccTemp.GetCookies(uriTemp);
             foreach (Cookie ck in cks)
             {
                 t+= ck.Name + " :" + ck.Value + "\n";
             }
             return t;
        }

        #endregion

        #region  POST提交

        public void PostOperater()
        {     
            try
            {
                byteArray = Encoding.UTF8.GetBytes(Data); // 转化
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);  //新建一个WebRequest对象用来请求或者响应url
                webRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                if(loginInfo.LoginCookie!=null)
                    webRequest.CookieContainer = loginInfo.LoginCookie;  
                else
                    webRequest.CookieContainer = cc;                                      //保存cookie  

                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";       //请求的内容格式为application/x-www-form-urlencoded
                webRequest.ContentLength = byteArray.Length;
                webRequest.AllowAutoRedirect = true;
                webRequest.Headers["Referer"] =refer;

                webRequest.AllowWriteStreamBuffering = true;
                webRequest.AllowReadStreamBuffering = true;
                webRequest.SetNetworkPreference(NetworkSelectionCharacteristics.Cellular);

               webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                IAsyncResult asyncResult = (IAsyncResult)webRequest.BeginGetRequestStream(new AsyncCallback(RequestReady), webRequest);
            }
            catch (Exception err)
            {
                Global.StoErr("PostOperate", err);
                that.toast.Message="连接超时，请检查网络";
                that.toast.Show();
            }
        }

        #endregion

        #region  准备POST请求 

        public void RequestReady(IAsyncResult asyncResult)
        {
            try
            {
                HttpWebRequest webRequest = asyncResult.AsyncState as HttpWebRequest;
                using (Stream stream = webRequest.EndGetRequestStream(asyncResult))
                {
                    stream.Write(byteArray, 0, byteArray.Length);
                    stream.Close();
                }
                IAsyncResult ResponseResult = (IAsyncResult)webRequest.BeginGetResponse(new AsyncCallback(ResponseReady), webRequest);
            }
            catch (Exception err)
            {
                Global.StoErr("RequestReady", err);
                that.toast.Message = "连接超时，请检查网络";
                that.toast.Show();
            }
        }

        #endregion

        #region GET请求

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
                webRequest.Headers["Accept-Language"] = "zh-CN";
                webRequest.Headers["Connection"] = "keep-alive";
                webRequest.Headers["Referer"] = refer;
                webRequest.Headers["Host"] = host;
                webRequest.Headers["DNT"] = "1";
                webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                webRequest.AllowAutoRedirect = true;

                webRequest.AllowWriteStreamBuffering = true;
                webRequest.AllowReadStreamBuffering = true;
                webRequest.Headers["Referer"] = refer;
                IAsyncResult ResponseResult = (IAsyncResult)webRequest.BeginGetResponse(new AsyncCallback(ResponseReady), webRequest);
            }
            catch (Exception err)
            {
                Global.StoErr("getOperater", err);
                home.toast.Message = "连接出错请检查网络";
                home.toast.Show();
            }
        }

        #endregion

        #region 请求验证码流

        public void getVCode(string code_uri)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(code_uri);
                webRequest.Accept = "image/webp,*/*;q=0.8";
                if (loginInfo.LoginCookie != null)
                    webRequest.CookieContainer = loginInfo.LoginCookie;
                else
                    webRequest.CookieContainer = cc;                                      //保存cookie  

                webRequest.AllowWriteStreamBuffering = true;
                webRequest.AllowReadStreamBuffering = true;

                webRequest.Method = "GET";                                          //请求方式是POST
                webRequest.AllowAutoRedirect = true;
                webRequest.Headers["Accept-Encoding"] = "gzip,deflate";
                webRequest.Headers["Accept-Language"] = "zh-CN";
                webRequest.Headers["Connection"] = "keep-alive";
                webRequest.Headers["Referer"] = "https://mp.weixin.qq.com/";
                webRequest.Headers["Host"] = "mp.weixin.qq.com";
                webRequest.Headers["DNT"] = "1";
                webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                IAsyncResult CodeResult = (IAsyncResult)webRequest.BeginGetResponse(new AsyncCallback(CodeReady), webRequest);
            }
            catch (Exception err)
            {
                Global.StoErr("GetVCode", err);
                that.toast.Message =  "获取验证码超时，请检查网络";
                that.toast.Show();
            }
      }

        #endregion

        #region 接收验证码流

        public void CodeReady(IAsyncResult CodeResult)
        {
            BitmapImage newjpg=null;
            try
            {
                string u = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString();
                HttpWebRequest request = CodeResult.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(CodeResult);
                using (Stream stream = response.GetResponseStream())
                {
                    if (!Sto.File.DirectoryExists("VC"))
                    {
                        Sto.File.CreateDirectory("VC");
                    }
                   // var isoFileStream = new IsolatedStorageFileStream
                    var outStream = Sto.File.OpenFile("VC\\VCode" + u + ".jpg", FileMode.Create, FileAccess.Write);
                   // FileStream outStream = new FileStream("VC\\VCode" + u + ".jpg", FileMode.Create);
                    /*Int32 i = 0;
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
                    }*/
                    stream.CopyTo(outStream);
                   
                    outStream.Close();
                    var readstream = Sto.File.OpenFile("VC\\VCode" + u + ".jpg", FileMode.Open, FileAccess.Read);
                    Deployment.Current.Dispatcher.BeginInvoke(() => {  newjpg=new BitmapImage(); newjpg.SetSource(readstream); });
                    //关闭文件
                }
                // Deployment.Current.Dispatcher.BeginInvoke(() => { t.Text = text2; });
                Deployment.Current.Dispatcher.BeginInvoke(() => { that.vc.Source = newjpg; that.showVCode(); });
            }
            catch (Exception err)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { that.state.Text = err.Message+"获取验证码超时，请检查网络"; });
                //that.toast.Show();
            }
      }

        #endregion

        #region 输入验证码不正确

        public void VCodeWrong(string i)
        {
            that.pb.Visibility = Visibility.Collapsed;
            if (i == "-8") that.state.Text = "请输入验证码";
            else that.state.Text = "验证码错误，请重新输入";

            Global.hasVCode = 1;
            if(loginInfo.LoginCookie==null)
                 loginInfo.LoginCookie = cc;

            string code_url = "https://mp.weixin.qq.com/cgi-bin/verifycode?username=" + that.acc.Text + "&r=" + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            getVCode(code_url);
            return;
        }

        #endregion

        #region 登陆成功

        public void loginOK(JArray ja)
        {
            Global.returnUrl = ja[0]["redirect_url"].ToString();
            string partten = "token=(?<token>[\\d]*)";
            var m = Regex.Match(Global.returnUrl,partten);
            string token = m.Groups["token"].Value.ToString();

             if (string.IsNullOrEmpty(token))
             {
                 that.state.Text = "检测不到Token,可能您开通了微信保护功能";
                 return;
             }

               that.hideVCode();

               if (!Sto.Info.Contains(that.acc.Text + "LaunchTimes"))
                   Sto.Info[that.acc.Text + "LaunchTimes"] = 1;

               if (Sto.File.DirectoryExists(that.acc.Text) && (Convert.ToInt32(Sto.Info[that.acc.Text + "LaunchTimes"]) == 15 || Sto.Info["hasErr"].ToString() == "1"||Global.isFirstSetupOrUpdate==1))
               {
                   Sto.Info[that.acc.Text + "LaunchTimes"] = 1;
                   Sto.deleteFile(that.acc.Text);
                   that.toggle.IsChecked = false;
               }

               that.code.Text = ""; //清空 验证码框

               if (!Sto.Info.Contains("acc")||Sto.Info["acc"].ToString()!=that.acc.Text)  //记住所有
               {
                   Sto.stoInfo<string>("acc",that.acc.Text);
                   Sto.stoInfo<string>("pass",that.pass.Password);
               }
               Sto.Info.Save();

            if(loginInfo.LoginCookie==null)
                loginInfo.LoginCookie = cc;

                loginInfo.CreateDate = DateTime.Now;
                loginInfo.Token = token;
                loginInfo.Err = null;

                that.pb.Visibility = Visibility.Collapsed;
                that.state.Visibility = Visibility.Collapsed;

                Global.isFirstLoad = true;
                if (Sto.File.DirectoryExists(loginInfo.UserName + "AllInfo"))
                    that.NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
                else
                    that.NavigationService.Navigate(new Uri("/AllPeopleInfo.xaml", UriKind.Relative));
        }

        #endregion

        #region 计算页数

        public int computePage(int i)
        {
            if (i % 10 != 0)
            {
                i = i / 10 + 1;
            }
            else
            {
                i = i / 10;
            }
            return i;
        }

        #endregion

        #region 接受网络流

        public void ResponseReady(IAsyncResult ResponseResult)
        {
            try
            {
                HttpWebRequest request = ResponseResult.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ResponseResult);
                StreamReader sr2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string text2 = sr2.ReadToEnd();

                #region Login

                if (Global.task == "Login")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            JArray ja = new JArray(JsonConvert.DeserializeObject(text2));
                            loginInfo.Err = ja[0]["base_resp"]["err_msg"].ToString();
                            string n = ja[0]["base_resp"]["ret"].ToString();
                            string i = "";
                            switch (n)
                            {
                                case "-1":
                                    i = "系统错误，请稍候再试。";
                                    break;
                                case "-2":
                                    i = "帐号或密码错误。";
                                    break;
                                case "-23":
                                    i = "您输入的帐号或者密码不正确，请重新输入。";
                                    break;
                                case "-21":
                                    i = "不存在该帐户。";
                                    break;
                                case "-7":
                                    i = "您目前处于访问受限状态。";
                                    break;
                                case "-8":
                                    { i = "请输入图中的验证码"; VCodeWrong("-8"); }
                                    break;
                                case "-27":
                                    { i = "您输入的验证码不正确，请重新输入"; VCodeWrong("-27"); }
                                    break;
                                case "-26":
                                    i = "该公众会议号已经过期，无法再登录使用。";
                                    break;
                                case "0":
                                    { i = "成功登录，正在跳转..."; loginOK(ja); }
                                    return;
                                case "-25":
                                    i = "海外帐号请在公众平台海外版登录";
                                    break;
                                default:
                                    i = "未知的返回。";
                                    break;
                            }
                            that.state.Text = i;
                            Global.task = "";
                        }
                        catch (Exception err)
                        {
                            if (that != null)
                            {
                                Global.StoErr("Login", err);
                                that.state.Text = "登陆发生未知错误，请重试";
                            }
                        }
                    }
                 );
                }

                #endregion

                #region GetHome

                else if (Global.task == "GetHome")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            string temp = text2;
                            string pattern;
                            if (Sto.File.FileExists(loginInfo.UserName + "\\Info.txt") && Sto.File.FileExists(loginInfo.UserName + "\\ico.jpg"))
                            {
                                try
                                {
                                    //读取本地资料
                                    var aFile = new IsolatedStorageFileStream(loginInfo.UserName + "\\Info.txt", FileMode.Open, Sto.File);
                                    StreamReader sr = new StreamReader(aFile);
                                    string strLine = sr.ReadLine();
                                    while (strLine != null)
                                    {
                                        if (strLine.Split(':')[0] == "type")
                                            loginInfo.Type = strLine.Split(':')[1];
                                        else
                                            loginInfo.NickName = strLine.Split(':')[1];
                                        strLine = sr.ReadLine();
                                    }
                                    sr.Close();
                                }
                                catch (Exception err)
                                {
                                    Global.StoErr("ExsistsFile", err);
                                    home.toast.Message = "读取文本资料出错";
                                    home.toast.Show();
                                }

                                //读取本地图片
                                try
                                {
                                    var readstream = Sto.File.OpenFile(loginInfo.UserName + "\\ico.jpg", FileMode.Open, FileAccess.Read);
                                    BitmapImage jpg = new BitmapImage();
                                    jpg.SetSource(readstream);
                                    home.ico.Source = jpg;
                                    readstream.Close();
                                }
                                catch (Exception err)
                                {
                                    Global.StoErr("ExsistsFile", err);
                                    home.toast.Message = "读取头像出错";
                                    home.toast.Show();
                                }

                                home.nickname.Text = loginInfo.NickName;
                                home.type.Text = loginInfo.Type;

                                if (Sto.Info.Contains(loginInfo.UserName + "LaunchTimes"))
                                    Sto.Info[loginInfo.UserName + "LaunchTimes"] = Convert.ToInt32(Sto.Info[loginInfo.UserName + "LaunchTimes"]) + 1;
                                else
                                    Sto.Info[loginInfo.UserName + "LaunchTimes"] = 1;
                                Sto.Info.Save();
                            }
                            else
                            {
                                if (!Sto.File.DirectoryExists(loginInfo.UserName))
                                    Sto.File.CreateDirectory(loginInfo.UserName);
                                Match m;

                                try
                                {
                                    pattern = "fakeid=(\\d*)";
                                    m = Regex.Match(temp, pattern);
                                    loginInfo.Fakeid = m.Groups[1].Value.ToString();
                                    //无法检测到fakeid
                                    if (string.IsNullOrEmpty(loginInfo.Fakeid))
                                    { home.toast.Message = "无法查找到到您的Fakeid，可能是登陆超时"; home.toast.Show(); return; }
                                    //获取头像
                                    string tempUri = "https://mp.weixin.qq.com/misc/getheadimg?fakeid=" + loginInfo.Fakeid + "&token=" + loginInfo.Token + "&lang=zh_CN";
                                    string tempRefer = "https://mp.weixin.qq.com/cgi-bin/home?t=home/index&lang=zh_CN&token=" + loginInfo.Token;
                                    GetIco getIco = new GetIco();
                                    getIco.getImage(tempUri, tempRefer, home);
                                }
                                catch (Exception err)
                                {
                                    Global.StoErr("CreateIco", err);
                                    if (Sto.File.FileExists(loginInfo.UserName + "\\ico.jpg"))
                                        Sto.File.DeleteFile(loginInfo.UserName + "\\ico.jpg");
                                    home.toast.Message = "创建头像出错,请刷新";
                                    home.toast.Show();
                                }

                                pattern = "nickname\">(\\S+)</a>";
                                m = Regex.Match(temp, pattern);
                                home.nickname.Text = loginInfo.NickName = m.Groups[1].Value;

                                pattern = "type icon_subscribe_label\">(\\S+)</a>";
                                m = Regex.Match(temp, pattern);
                                home.type.Text = loginInfo.Type = m.Groups[1].Value;

                                try
                                {
                                    var aFile = new IsolatedStorageFileStream(loginInfo.UserName + "\\Info.txt", FileMode.OpenOrCreate, Sto.File);
                                    StreamWriter sw = new StreamWriter(aFile);
                                    sw.WriteLine("type:" + loginInfo.Type);
                                    sw.WriteLine("nickname:" + loginInfo.NickName);
                                    sw.Close();
                                }
                                catch (Exception err)
                                {
                                    Global.StoErr("CreateInf", err);
                                    if (Sto.File.FileExists(loginInfo.UserName + "\\Info.txt"))
                                        Sto.File.DeleteFile(loginInfo.UserName + "\\Info.txt");
                                    if (home != null)
                                    {
                                        //home.t.Text = err.Message;
                                        home.toast.Message = "写入资料出错";
                                        home.toast.Show();
                                    }
                                    if (that != null)
                                    {
                                        that.state.Text = "写入资料出错";
                                    }
                                    if (newMessage != null)
                                    {
                                        newMessage.toast.Message = "写入资料出错";
                                        newMessage.toast.Show();
                                    }
                                }
                            }

                            try
                            {
                                pattern = "<em class=\"number\">(\\d+)</em>";
                                var ms = Regex.Matches(temp, pattern);
                                int i = 1;
                                foreach (Match match in ms)
                                {
                                    if (i == 1)
                                        Global.newAddMessage = home.talk.Text = match.Groups[1].Value.ToString();
                                    if (i == 2)
                                    {
                                        home.newperson.Text = match.Groups[1].Value.ToString();
                                        if (int.Parse(home.newperson.Text) > 0)
                                            Global.hasNewPeople = true;
                                            //home.NavigationService.Navigate(new Uri("AllPeopleInfo.xaml",UriKind.Relative));
                                    }
                                    if (i == 3)
                                        home.allpeople.Text = match.Groups[1].Value.ToString();
                                    i++;
                                }
                                home.pb.Visibility = Visibility.Collapsed;
                                Global.homeOK = true;
                                Global.isFirstLoad = false;
                            }
                            catch (Exception err)
                            {
                                Global.StoErr("RefreshPeopleNum", err);
                                if (home != null)
                                {
                                    //home.t.Text = err.Message;
                                    home.toast.Message = "写入资料出错";
                                    home.toast.Show();
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            Global.StoErr("GetHome", err);
                            if (home != null)
                            {
                                home.toast.Message = "处理个人资料出错";
                                home.toast.Show();
                            }
                        }
                    }
                 );
                }
                #endregion

                #region GetMessage

                else if (Global.task == "getMessage")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        string temp = text2;
                        string pattern;
                        try
                        {
                            if (!Sto.Info.Contains(loginInfo.UserName + "All_Messages_Num"))
                                Sto.stoInfo<int>(loginInfo.UserName + "All_Messages_Num", 0);

                            pattern = "\"msg_item\":((\\S|\\s)*).msg_item,";
                            var m = Regex.Match(temp, pattern);
                            temp = m.Groups[1].Value.ToString();

                            pattern = "\"id\":(?<id>[\\d]*),\"type\":[\\d]*,\"fakeid\":\"(?<fakeid>[\\d]*)\",\"nick_name\":\"(?<nickname>[^\"]*)\",\"date_time\":(?<time>[\\d]*),\"content\":\"(?<content>[^\"]*)\",\"source\":\"[^\"]*\",(|\"is_starred_msg\":(?<isstar>[\\d]*),)\"msg_status\":[\\d]*,(|\"remark_name\":\"(?<remarkname>[^\"]*)\",)\"has_reply\":(?<hasreply>[\\d]*),\"refuse_reason\":\"[^\"]*\",";
                            //pattern = @""id":(?<id>[\S]*),"type":\S*,"fakeid":"(?<fakeid>[\S]*)","nick_name":"(?<nickname>[\S]*)","date_time":(?<time>[\S]*),"content":"(?<content>[\S]*)","source":"\S*","msg_status":\S*,"has_reply":(?<hasreply>[\S]*),"refuse_reason":"\S*","multi_item":\[\S*\],"to_uin":\S*";
                            var ms = Regex.Matches(temp, pattern);

                            if (newMessage.messageCol.MessageTables.Count == 0)
                                Sto.Info[loginInfo.UserName + "All_Messages_Num"] = Convert.ToInt32(Sto.Info[loginInfo.UserName + "All_Messages_Num"]) + ms.Count;
                            else
                                Sto.Info[loginInfo.UserName + "All_Messages_Num"] = Convert.ToInt32(Sto.Info[loginInfo.UserName + "All_Messages_Num"]) + int.Parse(Global.newAddMessage);

                            int messagesNum = Convert.ToInt32(Sto.Info[loginInfo.UserName + "All_Messages_Num"]);
                            int i = messagesNum - int.Parse(Global.newAddMessage);
                            Sto.Info.Save();

                            foreach (Match match in ms)
                            {
                                try
                                {
                                    string showName = match.Groups["nickname"].Value;
                                    if (!String.IsNullOrEmpty(match.Groups["remarkname"].Value.ToString()))
                                        showName = match.Groups["remarkname"].Value.ToString();
                                    //创建一条表的数据
                                    MessageTable newmessage = new MessageTable { Num = loginInfo.UserName + ":" + match.Groups["id"].Value, TalkId = match.Groups["id"].Value, ownId = loginInfo.UserName, FakeId = match.Groups["fakeid"].Value, NickName = showName, Time = match.Groups["time"].Value, Content = match.Groups["content"].Value, has_Reply = match.Groups["hasreply"].Value, is_star = match.Groups["isstar"].Value };
                                    //添加绑定集合的数据
                                    newMessage.messageCol.MessageTables.Add(newmessage);
                                    //插入数据库
                                    newMessage.weixinDB.Messages.InsertOnSubmit(newmessage);
                                    messagesNum--;
                                    if (newMessage.messageCol.MessageTables.Count != 0 && messagesNum == i)
                                        break;

                                    try
                                    {
                                        //Global.personMessageShouldRefresh = int.Parse(Global.newAddMessage);
                                        newMessage.weixinDB.SubmitChanges(); //保存数据库的改变
                                    }
                                    catch (Exception err)
                                    {
                                        Random random=new Random();
                                        Global.StoErr("SaveDataBase", err);
                                        showName = match.Groups["nickname"].Value;
                                        if (!String.IsNullOrEmpty(match.Groups["remarkname"].Value.ToString()))
                                            showName = match.Groups["remarkname"].Value.ToString();
                                        //创建一条表的数据
                                        newmessage = new MessageTable { Num = loginInfo.UserName + ":" + match.Groups["id"].Value+ ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString(), TalkId = match.Groups["id"].Value, ownId = loginInfo.UserName, FakeId = match.Groups["fakeid"].Value, NickName = showName, Time = match.Groups["time"].Value, Content = match.Groups["content"].Value, has_Reply = match.Groups["hasreply"].Value, is_star = match.Groups["isstar"].Value };
                                        //添加绑定集合的数据
                                        newMessage.messageCol.MessageTables.Add(newmessage);
                                        //插入数据库
                                        newMessage.weixinDB.Messages.InsertOnSubmit(newmessage);
                                        messagesNum--;
                                        if (newMessage.messageCol.MessageTables.Count != 0 && messagesNum == i)
                                            break;
                                        try
                                        {
                                            newMessage.weixinDB.SubmitChanges(); //保存数据库的改变
                                        }
                                        catch
                                        {}
                                       // newMessage.toast.Message = "保存数据库失败";
                                       // newMessage.toast.Show();
                                    }

                                }
                                catch (Exception err)
                                {
                                    Global.StoErr("ModifyDataBase", err);
                                    //newMessage.t.Text = err.Message;
                                    newMessage.toast.Message = "无法修改数据库";
                                    newMessage.toast.Show();
                                }
                            }
                            Global.newAddMessage = "0";
                            newMessage.longDataSet();
                            newMessage.longListSelector.ItemsSource = newMessage.Data;
                            newMessage.pb.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception err)
                        {
                            Global.StoErr("GetMessage", err);
                            if (newMessage != null)
                            {
                                if (err.Message == "Operation not permitted on IsolatedStorageFileStream")
                                    newMessage.toast.Message = "订阅者信息有变化，请及时刷新";
                                else
                                    newMessage.toast.Message = "读取信息出错";
                                newMessage.toast.Show();
                            }
                        }
                    }
               );
                }

                #endregion

                #region GetAllPeopleInfo

                else if (Global.task == "GetAllPeopleInfo")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                       {
                           try
                           {
                               string temp = text2;
                               string pattern;
                               List<string> gr = new List<string>();
                               List<string> cn = new List<string>();
                               List<string> na = new List<string>();

                               pattern = "\"id\":(?<id>[\\d]*),\"name\":\"(?<name>[^\"]*)\",\"cnt\":(?<cnt>[\\d]*)}";
                               var ms = Regex.Matches(temp, pattern);
                               foreach (Match match in ms)
                               {
                                   if (int.Parse(match.Groups["cnt"].Value.ToString()) != 0)
                                   {
                                       gr.Add(match.Groups["id"].Value);
                                       na.Add(match.Groups["name"].Value);
                                       cn.Add(match.Groups["cnt"].Value);
                                   }
                               }

                               string[] groupid = gr.ToArray();
                               string[] name = na.ToArray();
                               string[] cnt = cn.ToArray();

                               int peopleNum = 0;
                               for (int i = 0; i < groupid.Length; i++)
                               {
                                   peopleNum += int.Parse(cnt[i]);
                               }

                               Global.perPerson = 90.000 / peopleNum;

                               Sto.stoInfo<Array>(loginInfo.UserName + "groupList", groupid);
                               Sto.stoInfo<Array>(loginInfo.UserName + "nameList", name);
                               Sto.stoInfo<Array>(loginInfo.UserName + "cntList", cnt);

                               Global.pb = 10;
                               Global.test = true;
                           }
                           catch (Exception err)
                           {
                               Global.StoErr("GetAllPeopleInfo_GET", err);
                               allPeopleInfo.state.Text = err.Message;
                           }
                       }
                    );
                }

                #endregion

                #region RefreshPersonMessage

                else if (Global.task == "RefreshPersonMessage")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                         {
                             try
                             {
                                 string pattern;
                                 pattern = "\"id\":(?<id>[\\d]*),\"type\":(?<type>[\\d]*),\"fakeid\":\"(?<fakeid>[\\d]*)\",\"nick_name\":\"(?<nickname>[^\"]*)\",\"date_time\":(?<time>[\\d]*),(\"content\":\"(?<content>[^\"]*)\"|)";
                                 var ms = Regex.Matches(text2, pattern);

                                 if (ms.Count != 0)
                                 {
                                     int i = 0; long tempMax = -1;
                                     while (long.Parse(ms[i].Groups["id"].Value.ToString()) > long.Parse(Sto.Info[loginInfo.UserName + "MaxTalkId" + talkPage.fakeid].ToString()))
                                     {
                                         if (i == 0) tempMax = long.Parse(ms[i].Groups["id"].Value.ToString());
                                         //创建一条表的数据
                                         TalkMessageTable newmessage = new TalkMessageTable { Num = loginInfo.UserName + ":" + talkPage.fakeid + ":" + ms[i].Groups["id"].Value.ToString(), Username = loginInfo.UserName, Tofakeid = talkPage.fakeid, Id = ms[i].Groups["id"].Value.ToString(), Type = ms[i].Groups["type"].Value.ToString(), Fakeid = ms[i].Groups["fakeid"].Value.ToString(), Nickname = ms[i].Groups["nickname"].Value.ToString(), Time = ms[i].Groups["time"].Value.ToString(), Content = ms[i].Groups["content"].Value.ToString() };
                                         //添加绑定集合的数据
                                         talkPage.messageCol.TalkMessageTables.Add(newmessage);
                                         //插入数据库
                                         talkPage.weixinDB.TalkMessages.InsertOnSubmit(newmessage);
                                         i++;
                                         if (i == ms.Count)
                                             break;
                                     }
                                     talkPage.weixinDB.SubmitChanges(); //保存数据库的改变
                                     if (tempMax > long.Parse(Sto.Info[loginInfo.UserName + "MaxTalkId" + talkPage.fakeid].ToString()))
                                         Sto.stoInfo<long>(loginInfo.UserName + "MaxTalkId" + talkPage.fakeid, tempMax);
                                 }
                                 if (talkPage.messageCol.TalkMessageTables.Count != 0)
                                 {
                                     talkPage.pm = new PersonMessages(talkPage, talkPage.fakeid);
                                     talkPage.longListSelector.ItemsSource = talkPage.pm.getData();
                                     talkPage.longListSelector.ScrollTo(talkPage.longListSelector.ItemsSource[1]);
                                 }
                                 talkPage.pb.Visibility = Visibility.Collapsed;
                                 talkPage.tmr.Start();
                             }
                             catch (Exception err)
                             {
                                 Global.StoErr("refreshPersonMessage", err);
                                 if (talkPage != null)
                                 {
                                     talkPage.toast.Message = "刷新错误，请重试";
                                     talkPage.toast.Show();
                                 }
                             }
                         }
                  );
                }

                #endregion

                #region RefreshSpan

                else if (Global.task == "RefreshSpan")
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            string pattern;
                            List<PersonMessage> source = new List<PersonMessage>();
                            pattern = "\"id\":(?<id>[\\d]*),\"type\":(?<type>[\\d]*),\"fakeid\":\"(?<fakeid>[\\d]*)\",\"nick_name\":\"(?<nickname>[^\"]*)\",\"date_time\":(?<time>[\\d]*),(\"content\":\"(?<content>[^\"]*)\"|)";
                            var ms = Regex.Matches(text2, pattern);
                            if (ms.Count != 0)
                            {
                                if (long.Parse(ms[0].Groups["id"].Value.ToString()) <= long.Parse(Sto.Info[loginInfo.UserName + "MaxTalkId" + talkPage.fakeid].ToString())) return;

                                int i = 0; long tempMax = -1;
                                //var aFile = Sto.File.OpenFile(loginInfo.UserName + "\\PersonalMessages\\message" + talkPage.fakeid + ".txt", FileMode.Append, FileAccess.Write);
                                //StreamWriter sw = new StreamWriter(aFile);
                                while (long.Parse(ms[i].Groups["id"].Value.ToString()) > long.Parse(Sto.Info[loginInfo.UserName + "MaxTalkId" + talkPage.fakeid].ToString()))
                                {
                                    if (i == 0) tempMax = long.Parse(ms[i].Groups["id"].Value.ToString());
                                    //创建一条表的数据
                                    TalkMessageTable newmessage = new TalkMessageTable { Num = loginInfo.UserName + ":" + talkPage.fakeid + ":" + ms[i].Groups["id"].Value.ToString(), Username = loginInfo.UserName, Tofakeid = talkPage.fakeid, Id = ms[i].Groups["id"].Value.ToString(), Type = ms[i].Groups["type"].Value.ToString(), Fakeid = ms[i].Groups["fakeid"].Value.ToString(), Nickname = ms[i].Groups["nickname"].Value.ToString(), Time = ms[i].Groups["time"].Value.ToString(), Content = ms[i].Groups["content"].Value.ToString() };
                                    //添加绑定集合的数据
                                    talkPage.messageCol.TalkMessageTables.Add(newmessage);
                                    //插入数据库
                                    talkPage.weixinDB.TalkMessages.InsertOnSubmit(newmessage);

                                    i++;
                                    if (i == ms.Count)
                                        break;
                                }
                                try
                                {
                                    talkPage.weixinDB.SubmitChanges(); //保存数据库的改变
                                }
                                catch (Exception err)
                                {
                                    Global.StoErr("保存数据库", err);
                                    talkPage.toast.Message = "保存数据库失败";

                                }
                                if (tempMax > long.Parse(Sto.Info[loginInfo.UserName + "MaxTalkId" + talkPage.fakeid].ToString()))
                                    Sto.stoInfo<long>(loginInfo.UserName + "MaxTalkId" + talkPage.fakeid, tempMax);
                            }
                            else
                                return;

                            talkPage.pm = new PersonMessages(talkPage, talkPage.fakeid);
                            talkPage.longListSelector.ItemsSource = talkPage.pm.getData();//用 Add优化性能
                            talkPage.longListSelector.ScrollTo(talkPage.longListSelector.ItemsSource[1]);
                        }
                        catch (Exception err)
                        {
                            Global.StoErr("refreshPersonMessage", err);
                            if (talkPage != null)
                            {
                                talkPage.toast.Message = "刷新错误";
                                talkPage.toast.Show();
                            }
                        }
                    }
                  );
                }

                #endregion

                #region SendGroup
                //else if (Global.task == "SendGroup")
                //{
                //    Deployment.Current.Dispatcher.BeginInvoke(() =>
                //    {
                //       string temp;
                //       string pattern;
                //      //  pattern = "wx.cgiData = {(?<info>[\\S\\s]*)seajs.use";   //可以匹配更多信息
                //      pattern = "operation_seq: \"(?<seq>[\\d]*)\"";
                //      var m = Regex.Match(text2, pattern);
                //      loginInfo.Seq = m.Groups["seq"].Value.ToString();

                //      // sendGroup.pb.Visibility = Visibility.Collapsed;

                //        sendGroup.pb.Visibility = Visibility.Collapsed;
                //        sendGroup.PreparingText.Visibility = Visibility.Collapsed;
                //        sendGroup.SendBox.IsEnabled = true;
                //    }
                //    );
                //}
                #endregion

                #region GetStarMessage
                //===========getstarmessage============
                /*  else if (Global.getStarMessage == true)
                  {
                      string temp = text2;
                          string pattern;
                      
                              if(!Sto.Info.Contains("All_Messages_Num"))
                                  Sto.stoInfo<int>("All_Messages_Num",0);
                              pattern = "\"msg_item\":((\\S|\\s)*).msg_item,";
                             var m= Regex.Match(temp, pattern);
                             temp= m.Groups[1].Value.ToString();

                             pattern = "\"id\":(?<id>[\\d]*),\"type\":[\\d]*,\"fakeid\":\"(?<fakeid>[\\d]*)\",\"nick_name\":\"(?<nickname>[^\"]*)\",\"date_time\":(?<time>[\\d]*),\"content\":\"(?<content>[^\"]*)\",\"source\":\"[^\"]*\",(|\"is_starred_msg\":(?<isstar>[\\d]*),)\"msg_status\":[\\d]*,\"has_reply\":(?<hasreply>[\\d]*),\"refuse_reason\":\"[^\"]*\",";
                             //pattern = @""id":(?<id>[\S]*),"type":\S*,"fakeid":"(?<fakeid>[\S]*)","nick_name":"(?<nickname>[\S]*)","date_time":(?<time>[\S]*),"content":"(?<content>[\S]*)","source":"\S*","msg_status":\S*,"has_reply":(?<hasreply>[\S]*),"refuse_reason":"\S*","multi_item":\[\S*\],"to_uin":\S*";
                              var ms = Regex.Matches(temp,pattern);



                      Global.getStarMessage = false;
                  }*/

                //===========getstarmessage============
                #endregion

              
            }
            catch (Exception err)
            {
                #region 异常处理
                Global.StoErr("HttpOperater", err);
                if(home!=null)
                    Deployment.Current.Dispatcher.BeginInvoke(() => { home.toast.Message = "获取资料出错，请检查网络状况"; home.toast.Show(); });
                else if(that!=null)
                   Deployment.Current.Dispatcher.BeginInvoke(() => { that.state.Text = "登陆超时，请检查网络状况"; });
                else if(newMessage!=null)
                    Deployment.Current.Dispatcher.BeginInvoke(() => { newMessage.toast.Message = "获取资料出错，请检查网络状况"; newMessage.toast.Show(); });
                else if (allPeopleInfo != null)
                    Deployment.Current.Dispatcher.BeginInvoke(() => { allPeopleInfo.state.Text = "登陆超时，请检查网络状况"; });
                #endregion
            }
        }

        #endregion
    }
}
