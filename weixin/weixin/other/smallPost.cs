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
    public class smallPost
    {
        public DispatcherTimer tmr;
        string Data;
        byte[] byteArray;
        Uri uri;
        CookieContainer cc = new CookieContainer();
        string refer;
        TalkPage talkPage;
        //SelectPicturePage selectPicturePage;
         public smallPost(string d,Uri u,string r ,TalkPage t)
        {
            Data=d;
            uri=u;
            refer=r;
            talkPage=t;
            Deployment.Current.Dispatcher.BeginInvoke(() => { 
                tmr= new DispatcherTimer();
                tmr.Tick += new EventHandler(degreeColl);
                tmr.Interval = TimeSpan.FromSeconds(1);
            });
        }

         //public smallPost(string d, Uri u, string r, SelectPicturePage t)
         //{
         //    Data = d;
         //    uri = u;
         //    refer = r;
         //    selectPicturePage = t;
         //}
         void degreeColl(object sender, EventArgs e)
         {
             Deployment.Current.Dispatcher.BeginInvoke(() =>
             {
                 talkPage.stateSp.Visibility = Visibility.Collapsed;
                 tmr.Stop();
             });
         }
        public void PostOperater()
        {
            try
            {
                byteArray = Encoding.UTF8.GetBytes(Data); // 转化
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);  //新建一个WebRequest对象用来请求或者响应url
                webRequest.Accept = "application/json, text/javascript, */*; q=0.01";
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
                Global.StoErr("PostOperate", err);
                talkPage.toast.Message = "连接超时，请检查网络";
                talkPage.toast.Show();
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
                Global.StoErr("RequestReady", err);
                talkPage.toast.Message = "连接超时，请检查网络";
                talkPage.toast.Show();
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
                //=================sendmeggage===============
                if (Global.send==true)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Global.send = false;
                        try
                        {
                            JArray ja = new JArray(JsonConvert.DeserializeObject(text2));
                           string Result = ja[0]["base_resp"]["err_msg"].ToString();
                            if(Result=="ok")
                            {
                                 talkPage.state.Text = "发送成功";
                                 tmr.Stop();
                                 tmr.Start();
                            }
                            else if (Result == "customer block")
                            {
                                talkPage.state.Text = "48小时未联系";
                                tmr.Stop();
                                tmr.Start();
                            }
                            else if (Result == "system error")
                            {
                                talkPage.state.Text = "系统错误";
                                tmr.Stop();
                                tmr.Start();
                            }
                            else
                            {
                                talkPage.state.Text = "发送失败"+Result;
                                tmr.Stop();
                                tmr.Start();
                            }
                        }
                        catch (Exception err)
                        {
                            if (talkPage != null)
                            {
                                Global.StoErr("send", err); 
                            }
                        }
                    }
                 );
                }
                //=============sendmessage====================
            }
            catch (Exception err)
            {

            }
        }


    }
}
