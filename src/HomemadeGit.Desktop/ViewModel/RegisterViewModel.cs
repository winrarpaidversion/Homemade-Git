
﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Desktop.ViewModel
{
    public partial class RegisterViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isCheck = false;
        private fakeauth Fakeauth;
        private MainViewModel MainViewModel;
        public RegisterViewModel(fakeauth fakeauth, MainViewModel mainViewModel)
        {
            Fakeauth = fakeauth;
            MainViewModel = mainViewModel;
        }
        [RelayCommand]
        public void SignUp()
        {
            MainViewModel.CurrentPage =new  GitViewModel();
        }
        [RelayCommand]
        public void SignIn()
        {
            MainViewModel.CurrentPage = new GitViewModel();
        }


        [RelayCommand]
        private void GoToSignUp() => IsCheck = true;

        [RelayCommand]
        private void GoToSignIn() => IsCheck = false;


    }
}