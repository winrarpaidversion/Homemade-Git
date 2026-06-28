
using CommunityToolkit.Mvvm.ComponentModel;
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
        [ObservableProperty]
        private string _isError = "Hidden";
        [ObservableProperty]
        private string _titleError;

        private readonly Func<int, GitViewModel> _gitviewModelFactory;

        public RegisterViewModel(AuthClientService authClientService, MainViewModel mainViewModel, Func<int, GitViewModel> gitviewModelFactory)
        {

            AuthClientService = authClientService;
            MainViewModel = mainViewModel;
            _gitviewModelFactory = gitviewModelFactory;
        }
        [RelayCommand]
        public void SignUp()
        {

            var regiserResult = AuthClientService.Register(Login, Password);
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                TitleError = "Введите все поля";
                IsError = "Visible";
                return;
            }
            if (regiserResult.Id != 0)
            {

                MainViewModel.CurrentPage = _gitviewModelFactory(regiserResult.Id);

            }
            else
            {

                IsError = "Visible";
                return;
            }


        }
        [RelayCommand]
        public void SignIn()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                TitleError = "Введите все поля";
                IsError = "Visible";
                return;
            }
            var loginResult = AuthClientService.Login(Login, Password);
            if (loginResult.Id != 0)
            {
                MainViewModel.CurrentPage = _gitviewModelFactory(loginResult.Id);

            }
            else
            {
                TitleError = $"Такого пользователя нет: {loginResult.Exception}";
                IsError = "Visible";

                return;
            } 

        }


        [RelayCommand]
        private void GoToSignUp() => IsCheck = true;

        [RelayCommand]
        private void GoToSignIn() => IsCheck = false;


    }
}