using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media.Imaging;

namespace weixin
{
    class GetIco
    {
        HomePage home;
        CookieContainer cc = new CookieContainer();//接收缓存

        public void getImage(string image_uri, string refer,HomePage h)
        {
            try
            {
                home = h;
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(image_uri);
                webRequest.Accept = "image/webp,*/*;q=0.8";
                if (loginInfo.LoginCookie != null)
                    webRequest.CookieContainer = loginInfo.LoginCookie;
                else
                    webRequest.CookieContainer = cc;                                      //保存cookie  

                webRequest.AllowWriteStreamBuffering = true;
                webRequest.AllowReadStreamBuffering = true;
                // webRequest.SetNetworkPreference(NetworkSelectionCharacteristics.Cellular);

                webRequest.Method = "GET";                                          //请求方式是POST
                webRequest.AllowAutoRedirect = true;
                //webRequest.Headers["Accept-Encoding"] = "gzip,deflate";
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
                home.toast.Message = "获取头像出错";
            }
        }

        public void ImageReady(IAsyncResult ImageResult)
        {
            HttpWebRequest request = ImageResult.AsyncState as HttpWebRequest;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ImageResult);
            using (Stream stream = response.GetResponseStream())
            {
                    try
                    {
                        if (!Sto.File.DirectoryExists(loginInfo.UserName))
                        {
                            Sto.File.CreateDirectory(loginInfo.UserName);
                        }


                        var outStream = Sto.File.OpenFile(loginInfo.UserName + "\\ico.jpg",FileMode.Create,FileAccess.Write);

                            Int32 i = 0;
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
                            stream.Close();
                            var readstream = Sto.File.OpenFile(loginInfo.UserName + "\\ico.jpg", FileMode.Open, FileAccess.Read);
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                               BitmapImage newjpg = new BitmapImage();
                                newjpg.SetSource(readstream);
                                home.ico.Source = newjpg;
                                readstream.Close();
                            });
                    }
                    catch (Exception err)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            Global.StoErr("Saveico", err);
                            home.toast.Message = "储存头像出错";
                            home.toast.Show();
                        });
                    }
            }
        }
    }
}
