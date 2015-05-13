using System.ComponentModel;
using System.Collections.ObjectModel;

namespace weixin
{
    public class TalkMessageCollection : INotifyPropertyChanged
    {
        //定义ObservableCollection来绑定页面的数据
        private ObservableCollection<TalkMessageTable> _TalkMessageTables;
        public ObservableCollection<TalkMessageTable> TalkMessageTables
        {
            get
            {
                return _TalkMessageTables;
            }
            set
            {
                if (_TalkMessageTables != value)
                {
                    _TalkMessageTables = value;
                    NotifyPropertyChanged("TalkMessageTables");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        //用于通知属性的改变
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
