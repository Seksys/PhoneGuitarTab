﻿using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using PhoneGuitarTab.Core.Navigation;

namespace PhoneGuitarTab.Core
{
    /// <summary>
    /// ViewModel 
    /// </summary>
    public class ViewModel : ViewModelBase
    {
        protected INavigationService navigationService;
 
        public ViewModel()
        {
           navigationService = Container.Resolve<INavigationService>();
        }

        private Dictionary<string, object> navigationParameters = null;
        public Dictionary<string, object> NavigationParameters
        {
            get
            {
                return navigationParameters;
            }
            set
            {
                navigationParameters = value;
                ReadNavigationParameters();
            }
        }

        protected virtual void ReadNavigationParameters()
        {
        }

        /// <summary>
        /// Saves view model state into dictionary
        /// </summary>
        /// <param name="state"></param>
        public virtual void SaveStateTo(IDictionary<string, object> state)
        {

        }

        /// <summary>
        /// Restores view model state from dictionary
        /// </summary>
        /// <param name="state"></param>
        public virtual void LoadStateFrom(IDictionary<string, object> state)
        {

        }
    }
}