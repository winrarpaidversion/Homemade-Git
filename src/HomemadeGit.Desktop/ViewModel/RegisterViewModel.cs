
﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomemadeGit.Core.Services;
using HomemadeGit.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Desktop.ViewModel
{
    public partial class RegisterViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _login;
        [ObservableProperty]
        private string _password;
        [ObservableProperty]
        private bool _isCheck = false;
        private AuthClientService AuthClientService;
        private MainViewModel MainViewModel;
        public RegisterViewModel(AuthClientService authClientService, MainViewModel mainViewModel)
        {
            AuthClientService = authClientService;
            MainViewModel = mainViewModel;
        }
        [RelayCommand]
        public void SignUp()
        {
            var regiserResult = AuthClientService.Register(Login, Password);
            if(regiserResult != null)
            {
                MainViewModel.CurrentPage = new GitViewModel();
            }
          
        }
        [RelayCommand]
        public void SignIn()
        {
            var loginResult = AuthClientService.Login(Login, Password);
            if (loginResult != null)
            {
                MainViewModel.CurrentPage = new GitViewModel();
            }

        }


        [RelayCommand]
        private void GoToSignUp() => IsCheck = true;

        [RelayCommand]
        private void GoToSignIn() => IsCheck = false;


    }
}