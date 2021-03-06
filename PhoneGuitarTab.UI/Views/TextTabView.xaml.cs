﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.UI.ViewModels;


namespace PhoneGuitarTab.UI
{
    public partial class TextTabView : ViewPage
    {
        private string TextTabUri = "/Html/texttab.html";
        public TextTabViewModel viewModel;
        private bool isBrowserReady;

        public TextTabView()
        {
            InitializeComponent();
            slider.Browser = tabWebBrowser;
            viewModel = DataContext as TextTabViewModel;
            viewModel.Browser = tabWebBrowser;

            tabWebBrowser.LoadCompleted += viewModel.Browser_LoadCompleted;
            tabWebBrowser.ScriptNotify += viewModel.Browser_ScriptNotify;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
            viewModel.AudioUrlRetrieved += AudioUrlRetrievedHandler;
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AutoScrollToggled")
            {
                this.ToggleSlider();
            }
        }

        /// <summary>
        ///     Event to Stop / Resume the scrolling when tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabWebBrowser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (slider.Visibility == Visibility.Visible)
            {
                if (slider.isScrolling)
                    slider.stopAutoScroll(sender);
                else
                    slider.invokeAutoScroll(sender);
            }

            if (!(ApplicationBar.Mode == ApplicationBarMode.Minimized))
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                //Set the adcontrol margin depending on the app bar mode
                AdControl.Margin = new Thickness(0,0,0,30);
            }
                
        }

        private void tabWebBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value.StartsWith("onReady"))
            {
                var viewModel = DataContext as TextTabViewModel;

                if (viewModel.TabContent != null)
                {
                    tabWebBrowser.InvokeScript("pullTabContent", viewModel.TabContent, 
                        viewModel.IsNightMode.ToString());
                }
                isBrowserReady = true;

                //Trigger Audio Stream Url retrieval - after the tab content is being pulled.
                viewModel.GetOnlineAudioStreamUrl();

            }
        }

        private void tabWebBrowser_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            tabWebBrowser.Navigate(new Uri(TextTabUri, UriKind.Relative));
        }

        private void ToggleSlider()
        {
            //Control Autoscroll behaviour
            if (slider.Visibility == Visibility.Visible)
            {
                slider.Visibility = Visibility.Collapsed;
                slider.stopAutoScroll(slider.Browser);
            }
            else
            {
                slider.Visibility = Visibility.Visible;
                slider.invokeAutoScroll(slider.Browser);
            }
            if (!(ApplicationBar.Mode == ApplicationBarMode.Minimized))
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);                            
            var viewModel = DataContext as TextTabViewModel;
            viewModel.PopUpMessageService.IncreaseTabViewCount();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
            if (isBrowserReady)
            {
                var viewModel = DataContext as TextTabViewModel;
                viewModel.StopAudioPlayer();
            }
        }

        private void root_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            //Control the appbar visibility (in landscape mode it opens the buttons, reading tabs become harder)
            if ((Orientation == PageOrientation.LandscapeLeft) ||
                (Orientation == PageOrientation.LandscapeRight))
            {
                ApplicationBar.IsVisible = false;
                //Set the adcontrol margins depending on the orientation
                AdControl.Margin = new Thickness(0, 0, 0, 0);
                AdControl.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                ApplicationBar.IsVisible = true;

                //Set the adcontrol margins depending on the orientation
                AdControl.Margin = new Thickness(0, 0, 0, 30);
                AdControl.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void AudioUrlRetrievedHandler(string audioUrl)
        {
            try
            {
                if (isBrowserReady)
                    Dispatcher.BeginInvoke(() => tabWebBrowser.InvokeScript("setAudioUrl", audioUrl));
            }
            catch (Exception)
            {
                //TODO - do some exception handling here  - sometimes the above JS function sucks.               
            }
        }

        private void TextTabView_OnBackKeyPress(object sender, CancelEventArgs e)
        {
            //If a chord pop-up is displayed do not navigate back to the main app menu, stay in the page and close the chord pop-up.
            if (Convert.ToBoolean(tabWebBrowser.InvokeScript("getChordPopUpVisibility")))
                {
                    e.Cancel = true;
                    tabWebBrowser.InvokeScript("closeChordPopUp");

                }              
        }
    }
}