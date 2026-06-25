using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Desktop.ViewModel
{

    public partial class GitViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _path;
        private DialogService DialogService;
        public GitViewModel()
        {
            DialogService = new DialogService();
        }
        [RelayCommand]
        private void PushCommit()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Description)) return;

        }
        [RelayCommand]
        private void NewRepository()
        {

            DialogService.SelectFolder();


        }
        }

    
}

