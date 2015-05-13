using System.ComponentModel;
using System.Collections.ObjectModel;

namespace weixin
{
    public class MessageCollection : INotifyPropertyChanged
    {
        //定义ObservableCollection来绑定页面的数据
        private ObservableCollection<MessageTable> _MessageTables;
        public ObservableCollection<MessageTable> MessageTables
        {
            get
            {
                return _MessageTables;
            }
            set
            {
                if (_MessageTables != value)
                {
                    _MessageTables = value;
                    NotifyPropertyChanged("MessageTables");
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
