﻿#pragma checksum "D:\个人资料\William\工程\weixin - 副本 - 副本\weixin\TalkPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DEED946B08D549B3D839E1BBE06EEB5E"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace weixin {
    
    
    public partial class TalkPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.StackPanel stateSp;
        
        internal System.Windows.Controls.TextBlock state;
        
        internal System.Windows.Controls.TextBlock nickname;
        
        internal System.Windows.Controls.Image bot;
        
        internal System.Windows.Controls.TextBox tb;
        
        internal System.Windows.Controls.ProgressBar pb;
        
        internal Microsoft.Phone.Controls.LongListSelector longListSelector;
        
        internal Microsoft.Phone.Shell.ApplicationBar ab;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/weixin;component/TalkPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.stateSp = ((System.Windows.Controls.StackPanel)(this.FindName("stateSp")));
            this.state = ((System.Windows.Controls.TextBlock)(this.FindName("state")));
            this.nickname = ((System.Windows.Controls.TextBlock)(this.FindName("nickname")));
            this.bot = ((System.Windows.Controls.Image)(this.FindName("bot")));
            this.tb = ((System.Windows.Controls.TextBox)(this.FindName("tb")));
            this.pb = ((System.Windows.Controls.ProgressBar)(this.FindName("pb")));
            this.longListSelector = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("longListSelector")));
            this.ab = ((Microsoft.Phone.Shell.ApplicationBar)(this.FindName("ab")));
        }
    }
}
