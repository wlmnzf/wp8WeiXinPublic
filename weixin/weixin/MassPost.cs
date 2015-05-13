using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Threading;


namespace weixin
{
    public class MassPost
    {
        public DispatcherTimer tmr;
        string Data;
        byte[] byteArray;
        Uri uri;
        CookieContainer cc = new CookieContainer();
        string refer;
        SendGroup sendGroup;
        public MassPost(string d, Uri u, string r, SendGroup t)
        {
            Data = d;
            uri = u;
            refer = r;
            sendGroup = t;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                tmr = new DispatcherTimer();
                tmr.Tick += new EventHandler(degreeColl);
                tmr.Interval = TimeSpan.FromSeconds(1);
            });
        }
        void degreeColl(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //talkPage.stateSp.Visibility = Visibility.Collapsed;
                tmr.Stop();
            });
        }
        public void PostOperater()
        {
            try
            {
                byteArray = Encoding.UTF8.GetBytes(Data); // 转化
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);  //新建一个WebRequest对象用来请求或者响应url
                webRequest.Accept = "Accept	text/html, */*; q=0.01";
                if (loginInfo.LoginCookie != null)
                    webRequest.CookieContainer = loginInfo.LoginCookie;
                else
                    webRequest.CookieContainer = cc;                                      //保存cookie  

                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";       //请求的内容格式为application/x-www-form-urlencoded
                webRequest.ContentLength = byteArray.Length;
                webRequest.AllowAutoRedirect = true;
                webRequest.Headers["Referer"] = refer;

                webRequest.AllowWriteStreamBuffering = true;
                webRequest.AllowReadStreamBuffering = true;

                webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                IAsyncResult asyncResult = (IAsyncResult)webRequest.BeginGetRequestStream(new AsyncCallback(RequestReady), webRequest);
            }//返回用于将数据写入 Internet 资源的 Stream。
            catch (Exception err)
            {
                Global.StoErr("MassPostPostOperate", err);
                sendGroup.toast.Message = "连接超时，请检查网络";
                sendGroup.toast.Show();
            }
        }

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
                Global.StoErr("MassPostRequestReady", err);
                sendGroup.toast.Message = "连接超时，请检查网络";
                sendGroup.toast.Show();
            }
        }


        public void ResponseReady(IAsyncResult ResponseResult)
        {
            try
            {
                HttpWebRequest request = ResponseResult.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ResponseResult);
                StreamReader sr2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string text2 = sr2.ReadToEnd();
                JArray ja = new JArray(JsonConvert.DeserializeObject(text2));
                string ret = ja[0]["base_resp"]["ret"].ToString();
                string msg = ja[0]["base_resp"]["err_msg"].ToString();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (ret=="0"||msg == "ok")
                        {
                            sendGroup.toast.Message = "发送成功";
                            sendGroup.toast.Show();
                            sendGroup.SendBox.Text ="";
                        }
                        else if (ret=="64004"||msg == "not have masssend quota today!")
                        {
                            sendGroup.toast.Message = "您可群发的消息还剩0条";
                            sendGroup.toast.Show();
                        }
                        else if (ret=="-1"||msg == "sys error")
                        {
                            sendGroup.toast.Message = "发送失败，您可能未绑定安全助手";
                            sendGroup.toast.Show();
                        }
                        else if (ret == "-1" || msg == "system fail")
                        {
                            sendGroup.toast.Message = "发送失败，可能您开启了，群发消息保护";
                            sendGroup.toast.Show();
                        }
                    });
                //"{\"ret\":\"-1\", \"msg\":\"system fail\"}"
                //"{\"ret\":\"-1\", \"msg\":\"sys error\"}"
                //"{\"ret\":\"64004\", \"msg\":\"not have masssend quota today!\"}"
                //{"ret":"0", "msg":"ok"}
            }
            catch (Exception err)
            {

            }
        }


    }
}
