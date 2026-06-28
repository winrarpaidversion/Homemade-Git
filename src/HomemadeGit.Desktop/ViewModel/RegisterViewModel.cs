
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
        public async Task SignUp()
        {

            var regiserResult =await AuthClientService.Register(Login, Password);
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                TitleError = "Введите все поля";
                IsError = "Visible";
                return;
            }
            if (regiserResult.UserId != 0 || regiserResult != null)
            {

                MainViewModel.CurrentPage = _gitviewModelFactory(regiserResult.UserId);

            }
            else
            {

                IsError = "Visible";
                return;
            }


        }
        [RelayCommand]
        public async Task SignIn()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                TitleError = "Введите все поля";
                IsError = "Visible";
                return;
            }
            var loginResult =await AuthClientService.Login(Login, Password);
            if (loginResult.UserId != 0 || loginResult != null)
            {
                MainViewModel.CurrentPage = _gitviewModelFactory(loginResult.UserId);

            }
            else
            {
                TitleError = $"Такого пользователя  {loginResult.Login} нет:";
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