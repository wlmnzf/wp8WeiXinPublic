using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace weixin
{
    public static class Sto
    {
        public static IsolatedStorageSettings Info=IsolatedStorageSettings.ApplicationSettings;
        public static IsolatedStorageFile File = IsolatedStorageFile.GetUserStoreForApplication();
        public static void stoInfo<T>(string save_key, T save_val)//保存独立储存的键值对
        {

            if (Info.Contains(save_key))
            {
                Info[save_key] = save_val;
            }
            else
            {
                Info.Add(save_key, save_val);
            }
            Info.Save();
        }
        public static void deleteFile(string filename)
        {
          
            string[] dir = File.GetDirectoryNames(filename+"\\*");
            string[] file = File.GetFileNames(filename+"\\*");
            if(dir.Length==0&&file.Length==0)
                File.DeleteDirectory(filename);
            if (file.Length != 0)
            {
                for (int i = 0; i < file.Length; i++)
                {
                    if (File.FileExists(filename + "\\" + file[i]))
                    File.DeleteFile(filename+"\\"+file[i]);
                }
            }
            if (dir.Length != 0)
            {
                for (int i = 0; i < dir.Length; i++)
                {
                    deleteFile(filename+"\\"+dir[i]);
                    if (File.DirectoryExists(filename + "\\" + dir[i]))
                    File.DeleteDirectory(filename + "\\" + dir[i]);
                }
            }
        }

    }
}
