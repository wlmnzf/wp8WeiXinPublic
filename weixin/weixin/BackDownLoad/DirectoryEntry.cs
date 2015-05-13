using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace weixin
{
    public enum FileTypes
    {
        Image,
        Video,
        Music,
        Unknown
    }
    class DirectoryEntry
    {
        private DateTime DateModifiedField;
        private string EntryNameField;
        private long SizeField;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public DateTime DateModified
        {
            get
            {
                return this.DateModifiedField;
            }
            set
            {
                if (!this.DateModifiedField.Equals(value))
                {
                    this.DateModifiedField = value;
                    this.RaisePropertyChanged("DateModified");
                }
            }
        }

        public string Detail
        {
            get
            {
                string str = "";
                str = this.Size.ToString() + " Bytes. ";
                return str;
            }
        }

        public string EntryName
        {
            get
            {
                return this.EntryNameField;
            }
            set
            {
                if (!object.ReferenceEquals(this.EntryNameField, value))
                {
                    this.EntryNameField = value;
                    this.RaisePropertyChanged("EntryName");
                }
            }
        }

        public FileTypes FileType
        {
            get
            {
                switch (Path.GetExtension(this.EntryName).ToLower())
                {
                    case ".png":
                    case ".jpg":
                    case ".bmp":
                    case ".jpeg":
                    case ".gif":
                        return FileTypes.Image;

                    case ".wmv":
                        return FileTypes.Video;

                    case ".wma":
                    case ".mp3":
                        return FileTypes.Music;
                }
                return FileTypes.Unknown;
            }
        }

        public bool HasPreview
        {
            get
            {
                bool flag = false;
                if (((this.FileType != FileTypes.Image) && (this.FileType != FileTypes.Music)) && (this.FileType != FileTypes.Video))
                {
                    return flag;
                }
                return true;
            }
        }

        public long Size
        {
            get
            {
                return this.SizeField;
            }
            set
            {
                if (!this.SizeField.Equals(value))
                {
                    this.SizeField = value;
                    this.RaisePropertyChanged("Size");
                }
            }
        }

    }
}
