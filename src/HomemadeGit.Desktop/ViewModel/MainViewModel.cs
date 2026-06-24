using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Desktop.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private object? _currentPage;
        private fakeauth Fakeauth;

        public MainViewModel(fakeauth fakeauth)
        {
            Fakeauth = fakeauth;
            CurrentPage = new RegisterViewModel(fakeauth, this);
        }
    }
}