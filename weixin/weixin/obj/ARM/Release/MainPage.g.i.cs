﻿#pragma checksum "D:\个人资料\William\工程\weixin\weixin\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E322F41412F746A02B3DD429AD386577"
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
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox acc;
        
        internal System.Windows.Controls.PasswordBox pass;
        
        internal System.Windows.Controls.Button login;
        
        internal System.Windows.Controls.Image vc;
        
        internal System.Windows.Controls.TextBlock state;
        
        internal System.Windows.Controls.ProgressBar pb;
        
        internal System.Windows.Controls.TextBox code;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/weixin;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.acc = ((System.Windows.Controls.TextBox)(this.FindName("acc")));
            this.pass = ((System.Windows.Controls.PasswordBox)(this.FindName("pass")));
            this.login = ((System.Windows.Controls.Button)(this.FindName("login")));
            this.vc = ((System.Windows.Controls.Image)(this.FindName("vc")));
            this.state = ((System.Windows.Controls.TextBlock)(this.FindName("state")));
            this.pb = ((System.Windows.Controls.ProgressBar)(this.FindName("pb")));
            this.code = ((System.Windows.Controls.TextBox)(this.FindName("code")));
        }
    }
}

