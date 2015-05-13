using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;

namespace weixin
{
    class IsolatedStorageHelper
    {
        public static DirectoryEntry[] GetDirectoryEntries(string directoryPath)
        {
            List<DirectoryEntry> list = new List<DirectoryEntry>();
            try
            {
                DirectoryEntry entry;
                string[] directoryNames;
                IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();

                if (string.IsNullOrEmpty(directoryPath))
                {
                    directoryNames = userStoreForApplication.GetDirectoryNames();
                }
                else
                {
                    directoryNames = userStoreForApplication.GetDirectoryNames(Path.Combine(directoryPath, "*"));
                }
                Array.Sort<string>(directoryNames);
                foreach (string str2 in directoryNames)
                {
                    entry = new DirectoryEntry
                    {
                        EntryName = Path.GetFileName(str2),
                        Size = 0L
                    };
                    try
                    {
                        entry.DateModified = Directory.GetLastWriteTime(str2);
                    }
                    catch
                    {
                        entry.DateModified = DateTime.Now;
                    }
                    list.Add(entry);
                }
                string[] fileNames = userStoreForApplication.GetFileNames(Path.Combine(directoryPath, "*"));
                Array.Sort<string>(fileNames);
                foreach (string str3 in fileNames)
                {
                    entry = new DirectoryEntry
                    {
                        EntryName = Path.GetFileName(str3)
                    };
                    try
                    {
                        entry.Size = GetFileSize(Path.Combine(directoryPath, str3));
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        entry.DateModified = DateTime.Now;
                        entry.Size = 0L;
                    }
                    list.Add(entry);
                }
            }
            catch (Exception)
            {
            }
            return list.ToArray();
        }

        public static long GetFileSize(string fileName)
        {
            long length = 0L;
            try
            {
                using (BinaryReader reader = new BinaryReader(IsolatedStorageFile.GetUserStoreForApplication().OpenFile(fileName, FileMode.Open, FileAccess.Read)))
                {
                    length = reader.BaseStream.Length;
                }
            }
            catch (Exception)
            {
            }
            return length;
        }

    }
}
