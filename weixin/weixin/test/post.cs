using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace weixin
{
    public class post
    {
        HttpWebRequest webRequest;
        string strSendData;
       // Method strMethod;
        public void PostHttpWebRequest(string strData)
        {
            //strMethod = "POST";
            strSendData = strData;

            webRequest = (HttpWebRequest)WebRequest.Create(new Uri("http://mp.weixin.qq.com/cgi-bin/login?lang=zh_CN"));
            webRequest.Method = "POST";
            //webRequest.ContentType = "multipart/form-data";
            //webRequest.UseDefaultCredentials = true;

            var resBeginSend = (IAsyncResult)webRequest.BeginGetRequestStream(RequestSendCallback, webRequest);
        }

        /// <summary>
        /// GET数据
        /// </summary>
        public void GetHttpWebRequest()
        {
            //strMethod = "GET";
            webRequest = (HttpWebRequest)WebRequest.Create(new Uri("http://www.baidu.com"));
            webRequest.Method ="GET";
            // get 方法直接到Response函数
            var resBeginSend = (IAsyncResult)webRequest.BeginGetResponse(MYTestResponseCallback, webRequest);
        }

        /// <summary>
        /// Post方式返回数据响应
        /// </summary>
        /// <param name="result"></param>
        private void RequestSendCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                request.ContentType = "multipart/form-data";
                var data = Encoding.UTF8.GetBytes(strSendData);
                Stream newStream = request.EndGetRequestStream(result);
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                //=======================================================
                StreamReader sr = new StreamReader(newStream, Encoding.UTF8);
                string str = sr.ReadToEnd();
                sr.Close();
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    MessageBox.Show(str);
                });
                //==========================================================
                var resGetResp = (IAsyncResult)request.BeginGetResponse(MYTestResponseCallback, request);
            }
            catch (Exception err)
            {
                Global.StoErr("post.cs", err);
            }
        }

        public void MYTestResponseCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                var response = request.EndGetResponse(result);

                WebHeaderCollection webHeader = response.Headers;
                using (var stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                    string str = sr.ReadToEnd();
                    sr.Close();
                    //YYYDebug.Write("返回的数据信息：", str);

                    //HNetResultEventArgs ee = new HNetResultEventArgs(str);
                    //CompleteDataEventHandler(this, ee);

                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        //MessageBox.Show(str);
                        //HNetResultEventArgs ee = new HNetResultEventArgs(str);
                        //CompleteDataEventHandler(this, ee);
                        //DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(result));
                    });
                    //done.Set();
                }
            }
            catch (System.Exception e)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    string dd = e.Message;
                    //HNetResultEventArgs ee = new HNetResultEventArgs(str);
                    //CompleteDataEventHandler(this, ee);
                    //DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(result));
                });
            }
        }

    }
}
