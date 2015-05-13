using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;

namespace weixin
{
    public static class showhtml
    {
        public static void SaveStringToIsoStore(string strWebContent)
        {
            //获取本地应用程序存储对象
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            //清除之前保存的网页
            if (isoStore.FileExists("web.htm") == true)
            {
                isoStore.DeleteFile("web.htm");
            }
            StreamResourceInfo sr = new StreamResourceInfo(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(strWebContent)), "html/text");//转化为流
            using (BinaryReader br = new BinaryReader(sr.Stream))
            {
                byte[] data = br.ReadBytes((int)sr.Stream.Length);
                //保存文件到本地存储
                using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile("web.htm")))
                {
                    bw.Write(data);
                    bw.Close();
                }
            }
        }



    }
}
