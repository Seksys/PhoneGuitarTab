﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.Arts;
using PhoneGuitarTab.Search.Suggestions;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class GenreGroupsViewModel : DataContextViewModel
    {
        #region  Fields

        private readonly IGenreBrowser _genreBrowser;
        private Group _currentGenre;
      
        private ObservableCollection<Group> _groups;

        private bool isLoading;


        #endregion  Fields

        #region Constructors

        [Dependency]
        public GenreGroupsViewModel(IGenreBrowser genreBrowser, IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            _genreBrowser = genreBrowser;
            CreateCommands();
            RegisterEvents();
        }

        #endregion Constructors

        #region Properties

        public Group CurrentGenre
        {
            get { return _currentGenre; }
            set
            {
                _currentGenre = value;
                RaisePropertyChanged("CurrentGenre");
                DataBind();
            }
        }
        
        public ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                RaisePropertyChanged("Groups");
            }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                RaisePropertyChanged("IsLoading");
              
            }
        }      

        #endregion Properties

        #region Override members
      
        protected override void DataBind()
        {
            this.Groups = new ObservableCollection<Group>(); 
        }

        protected override void ReadNavigationParameters()
        {
            if (NavigationParameters == null)
                return;
            CurrentGenre = (Group) NavigationParameters["genregroups"];
        }

        #endregion Override members

        #region Commands
      
        public ExecuteCommand Search { get; set; }
                    
        #endregion Commands

        #region Command handlers

        private void DoSearch()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                IsLoading = true;
                this.Groups.Clear();
                _genreBrowser.Run(CurrentGenre.Name);
            }
        }


        #endregion Command handlers

        #region Event handlers
        void SuggestionSearchCompleted(object sender)
        {
            var artistsByGenre = sender as IGenreBrowser;

            foreach (var band in artistsByGenre.Results)
            {             
               var group = new Group { Name = band.BandName, ExtraLargeImageUrl = band.ExtraLargeImageUrl };
               this.Groups.Add(group);
            }
            this.IsLoading = false;
        }
        #endregion Event handlers

        #region Helper methods

        private void CreateCommands()
        {
            Search = new ExecuteCommand(DoSearch);                             
        }
     
        private void RegisterEvents()
        {
            _genreBrowser.SuggestionSearchCompleted += (s, e) => SuggestionSearchCompleted(s);
        }

       
       

        #endregion Helper methods
    }
}