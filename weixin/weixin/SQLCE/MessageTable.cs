using System.Data.Linq.Mapping;
using System.ComponentModel;

namespace weixin
{
    [Table]
    public class MessageTable : INotifyPropertyChanged, INotifyPropertyChanging
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

        // talkid
        private string _TalkId;

        [Column(CanBeNull = false)]
        public string TalkId
        {
            get
            {
                return _TalkId;
            }
            set
            {
                if (_TalkId != value)
                {
                    NotifyPropertyChanging("TalkId");
                    _TalkId = value;
                    NotifyPropertyChanged("TalkId");
                }
            }
        }

      
            // ownid
            private string _ownId;

            [Column(CanBeNull = false)]
            public string ownId
            {
                get
                {
                    return _ownId;
                }
                set
                {
                    if (_ownId != value)
                    {
                        NotifyPropertyChanging("ownId");
                        _ownId = value;
                        NotifyPropertyChanged("ownId");
                    }
                }
            }


            // 定义员工名字字段
            private string _FakeId;

            [Column(CanBeNull = false)]
            public string FakeId
            {
                get
                {
                    return _FakeId;
                }
                set
                {
                    if (_FakeId != value)
                    {
                        NotifyPropertyChanging("FakeId");
                        _FakeId = value;
                        NotifyPropertyChanged("FakeId");
                    }
                }
            }

            private string _NickName;
            [Column(CanBeNull = false)]
            public string NickName
            {
                get
                {
                    return _NickName;
                }
                set
                {
                    if (_NickName != value)
                    {
                        NotifyPropertyChanging("NickName");
                        _NickName = value;
                        NotifyPropertyChanged("NickName");
                    }
                }
            }

            //private string _RemarkName;
            //[Column(CanBeNull = true)]
            //public string RemarkName
            //{
            //    get
            //    {
            //        if (_RemarkName == null)
            //            _RemarkName = "1";
            //        return _RemarkName;
            //    }
            //    set
            //    {
            //        if (_RemarkName != value)
            //        {
            //            NotifyPropertyChanging("RemarkName");
            //            _RemarkName = value;
            //            NotifyPropertyChanged("RemarkName");
            //        }
            //    }
            //}

            private string _Time;
            [Column]
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
                        NotifyPropertyChanging("Content");
                        _Content = value;
                        NotifyPropertyChanged("Content");
                    }
                }
            }

            private string _has_Reply;
            [Column(CanBeNull = false)]
            public string has_Reply
            {
                get
                {
                    return _has_Reply;
                }
                set
                {
                    if (_has_Reply != value)
                    {
                        NotifyPropertyChanging("has_Reply");
                        _has_Reply = value;
                        NotifyPropertyChanged("has_Reply");
                    }
                }
            }


            private string _is_star;
        [Column(CanBeNull = false)]
            public string is_star
            {
                get
                {
                    return _is_star;
                }
                set
                {
                    if (_is_star != value)
                    {
                        if (value == null)
                            value = "0";
                        NotifyPropertyChanging("is_star");
                        _is_star = value;
                        NotifyPropertyChanged("is_star");
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

