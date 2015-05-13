using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using Coding4Fun.Toolkit.Controls;

namespace weixin
{
    public partial class SelectPicturePage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;
        Stream resultImagetream;
        string fileName;
        public ToastPrompt  toast=new ToastPrompt();
        public SelectPicturePage()
        {
            InitializeComponent();
             photoChooserTask = new PhotoChooserTask();//初始化任务对象
             cameraCaptureTask = new CameraCaptureTask();
             photoChooserTask.Completed += photoChooserTask_Completed;//标识要在用户完成任务后运行的方法。
             cameraCaptureTask.Completed += OnCameraCapTureTask_Completed;
             //photoChooserTask.Show();
        }
       
         private void photoChooserTask_Completed(object sender, PhotoResult e)
           {
               if (e.TaskResult == TaskResult.OK)//选择器操作完成
               {
                     resultImagetream=e.ChosenPhoto;
                     string fileName=e.OriginalFileName;
                    while (resultImagetream.Length / 1024.00 / 1024.00 > 2)
                        resultImagetream = ImageOpreate.CompressImage(resultImagetream);
                     BitmapImage bmp = new BitmapImage();
                     bmp.SetSource(resultImagetream);//将照片数据的流赋给bmp对象
                     this.ShowTempPicture.Source = bmp;//将选择的图片显示出来
                }
           }

        private void OnCameraCapTureTask_Completed(object sender, PhotoResult e)
          {
              if (e.TaskResult == TaskResult.OK)//选择器操作完成
               {
                    resultImagetream = e.ChosenPhoto;
                    fileName=e.OriginalFileName;
                   while (resultImagetream.Length / 1024.00 / 1024.00 > 2)
                       resultImagetream = ImageOpreate.CompressImage(resultImagetream);
                     BitmapImage bmp = new BitmapImage();
                     bmp.SetSource(resultImagetream);//将照片数据的流赋给bmp对象
                     this.ShowTempPicture.Source = bmp;
                }
          }

         private void ApplicationBarIconButton_Click(object sender, EventArgs e)
         {
             photoChooserTask.Show();
         }

         private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
         {
             cameraCaptureTask.Show();
         }

         private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
         {
             string url = "https://mp.weixin.qq.com/cgi-bin/filetransfer?action=upload_material&f=json&ticket_id="+loginInfo.UniformUserName+"&ticket="+loginInfo.Ticket+"&token="+loginInfo.Token+"&lang=zh_CN";
        
             string data="-----------------------------7df3823630724"+'\n'
                                +"Content-Disposition: form-data; name=\"id\""+'\n'
                                +"WU_FILE_2"+'\n'
                              +"-----------------------------7df3823630724"+'\n'
                              +"Content-Disposition: form-data; name=\"name\""+'\n'
                              +fileName
                              +"-----------------------------7df3823630724"+'\n'
                              +"Content-Disposition: form-data; name=\"type\""+'\n'
                              +"image/jpeg"+'\n'
                              +"-----------------------------7df3823630724"+'\n'
                              +"Content-Disposition: form-data; name=\"lastModifiedDate\""
                              +DateTime.Now
                              +"Content-Disposition: form-data; name=\"size\""+'\n'
                             +resultImagetream.Length + '\n'
                            + "-----------------------------7df3823630724" + '\n'
                            + "Content-Disposition: form-data; name=\"file\"; filename=" + fileName + '\n'
                             + "Content-Type: image/jpeg" + '\n';


             PostPicture pp = new PostPicture(data,new Uri(url),this);
             pp.PostOperater();
         }
 
    }
}