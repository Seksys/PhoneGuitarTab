﻿using System;
using System.IO;
using System.Text;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class TextTabViewModel : TabViewModelBase
    {
        public string Style { get; set; }
        private readonly TextTabAdapter _textTabAdapter;

        [Dependency]
        public TextTabViewModel(IDataContextService database, RatingService ratingService, MessageHub hub)
            : base(database, ratingService, hub)
        {
            _textTabAdapter = new TextTabAdapter();
        }

        protected override void ReadNavigationParameters()
        {
            try
            {
                base.ReadNavigationParameters();

                using (var stream = FileSystem.OpenFile(Tablature.Path, FileMode.Open))
                {
                    string document = _textTabAdapter.Adapt(new StreamReader(stream).ReadToEnd());

                    if (Tablature.TabType.Name == "chords")
                    {
                        document = document.Replace("[ch]", "<span>");
                        document = document.Replace("[/ch]", "</span>");
                    }

                    TabContent = document;
                }
            }
            catch (Exception ex)
            {
                Dialog.Show(ex.Message);
                throw ex;
            }
        }
    }
}