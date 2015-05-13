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
    public class                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      PostPicture
    {
        string Data;
        byte[] byteArray;
        Uri uri;
        CookieContainer cc = new CookieContainer();
       // string refer;
        SelectPicturePage selectPicturePage;

        public PostPicture(string d, Uri u, SelectPicturePage t)
        {
            Data = d;
            uri = u;
        //    refer = r;
            selectPicturePage = t;
        }

        public void PostOperater()
        {
            try
            {
                //byteArray = Encoding.UTF8.GetBytes(Data); // 转化
                //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);  //新建一个WebRequest对象用来请求或者响应url
                //webRequest.Accept = "*/*";
                //if (loginInfo.LoginCookie != null)
                //    webRequest.CookieContainer = loginInfo.LoginCookie;
                //else
                //    webRequest.CookieContainer = cc;                                      //保存cookie  

                //webRequest.Method = "POST";
                //webRequest.ContentType = "multipart/form-data; boundary=----------7df3823630724";       //请求的内容格式为application/x-www-form-urlencoded
                //webRequest.ContentLength = byteArray.Length;
              //  webRequest.Headers
              //  "-----------------------------7df3823630724" + '\n'
              //                  + "Content-Disposition: form-data; name=\"id\"" + '\n'
              //                  + "WU_FILE_2" + '\n'
              //                + "-----------------------------7df3823630724" + '\n'
              //                + "Content-Disposition: form-data; name=\"name\"" + '\n'
              //                + fileName
              //                + "-----------------------------7df3823630724" + '\n'
              //                + "Content-Disposition: form-data; name=\"type\"" + '\n'
              //                + "image/jpeg" + '\n'
              //                + "-----------------------------7df3823630724" + '\n'
              //                + "Content-Disposition: form-data; name=\"lastModifiedDate\""
              //                + DateTime.Now
              //                + "Content-Disposition: form-data; name=\"size\"" + '\n'
              //               + resultImagetream.Length + '\n'
              //              + "-----------------------------7df3823630724" + '\n'
              //              + "Content-Disposition: form-data; name=\"file\"; filename=" + fileName + '\n'
              //               + "Content-Type: image/jpeg" + '\n';

              //  webRequest.AllowAutoRedirect = true;
              ////  webRequest.Headers["Referer"] = refer;

              //  webRequest.AllowWriteStreamBuffering = true;
              //  webRequest.AllowReadStreamBuffering = true;

               // webRequest.UserAgent = "Shockwave Flash";
             //   IAsyncResult asyncResult = (IAsyncResult)webRequest.BeginGetRequestStream(new AsyncCallback(RequestReady), webRequest);
            }//返回用于将数据写入 Internet 资源的 Stream。
            catch (Exception err)
            {
                Global.StoErr("PostOperate", err);
                selectPicturePage.toast.Message = "连接超时，请检查网络";
                selectPicturePage.toast.Show();
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
                selectPicturePage.toast.Message = "连接超时，请检查网络";
                selectPicturePage.toast.Show();
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
                
            }
            catch (Exception err)
            {

            }
        }

    }
}
