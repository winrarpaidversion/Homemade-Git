using CommunityToolkit.Mvvm.ComponentModel;
using HomemadeGit.Core.Services;
using HomemadeGit.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Desktop.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private object? _currentPage;
        private AuthClientService AuthClientService;

        public MainViewModel(AuthClientService authClientService)
        {
            AuthClientService = authClientService;
            CurrentPage = new RegisterViewModel(AuthClientService, this);
        }
    }
}