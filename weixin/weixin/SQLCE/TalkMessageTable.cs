using System;
using System.Data.Linq.Mapping;
using System.ComponentModel;

namespace weixin
{
    [Table]
    public class TalkMessageTable : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // num
        private string _Num;
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Num
        {
            get
            {
                return _Num;
            }
            set
            {
                if (_Num != value)
                {
                    NotifyPropertyChanging("Num");
                    _Num = value;
                    NotifyPropertyChanged("Num");
                }
            }
        }

        private string _Username;
        [Column(CanBeNull = false)]
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                if (_Username != value)
                {
                    NotifyPropertyChanging("Username");
                    _Username = value;
                    NotifyPropertyChanged("Username");
                }
            }
        }

        private string _Tofakeid;
        [Column(CanBeNull = false)]
        public string Tofakeid
        {
            get
            {
                return _Tofakeid;
            }
            set
            {
                if (_Tofakeid != value)
                {
                    NotifyPropertyChanging("Tofakeid");
                    _Tofakeid = value;
                    NotifyPropertyChanged("Tofakeid");
                }
            }
        }


        private string _Id;
        [Column(CanBeNull = false)]
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    NotifyPropertyChanging("Id");
                    _Id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _Type;
        [Column(CanBeNull = false)]
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (_Type != value)
                {
                    NotifyPropertyChanging("Type");
                    _Type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }


        private string _Fakeid;
        [Column(CanBeNull = false)]
        public string Fakeid
        {
            get
            {
                return _Fakeid;
            }
            set
            {
                if (_Fakeid != value)
                {
                    NotifyPropertyChanging("Fakeid");
                    _Fakeid = value;
                    NotifyPropertyChanged("Fakeid");
                }
            }
        }

        private string _Nickname;
        [Column(CanBeNull = false)]
        public string Nickname
        {
            get
            {
                return _Nickname;
            }
            set
            {
                if (_Nickname != value)
                {
                    NotifyPropertyChanging("Nickname");
                    _Nickname = value;
                    NotifyPropertyChanged("Nickname");
                }
            }
        }


        private string _Time;
        [Column(CanBeNull = false)]
        public string Time
        {
            get
            {
                return _Time;
            }
            set
            {
                if (_Time != value)
                {
                    NotifyPropertyChanging("Time");
                    _Time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }


        private string _Content;
        [Column(CanBeNull = false)]
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    if(value==null)
                        value="[空消息]";
                    NotifyPropertyChanging("Content");
                    _Content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        //用来通知页面表的字段数据产生了改变
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // 用来通知数据上下文表的字段数据将要产生改变
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
