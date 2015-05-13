using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace weixin
{
    public partial class showweb : PhoneApplicationPage
    {
        public showweb()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            show.Navigate(new Uri("web.htm", UriKind.Relative));//加载本地保存的页面
            base.OnNavigatedTo(e);
        }
    }
}