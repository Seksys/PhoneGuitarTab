﻿using System;
using PhoneGuitarTab.UI.ViewModel;


namespace PhoneGuitarTab.UI
{
    public partial class TextTabView : Core.ViewPage
    {
        public TextTabView()
        {
            InitializeComponent();
            tabWebBrowser.Loaded += delegate
                {
                    tabWebBrowser.NavigateToString((DataContext as TextTabViewModel).TabContent);
                };
        }
    }
}