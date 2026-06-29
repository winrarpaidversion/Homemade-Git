
using HomemadeGit.Desktop.Services;
using HomemadeGit.Desktop.ViewModel;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
namespace HomemadeGit.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<AuthClientService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<RepositoryClientService>();
            services.AddSingleton<CommitClientService>();
            services.AddSingleton<BranchClientService>();
            services.AddSingleton<Func<int, GitViewModel>>(sp => id =>
              ActivatorUtilities.CreateInstance<GitViewModel>(sp, id));
            services.AddHttpClient();
            services.AddSingleton<MainWindow>();
   


            return services.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Достаем сервисы из контейнера
            var mainViewModel = Services.GetRequiredService<MainViewModel>();
            var registerViewModel = Services.GetRequiredService<RegisterViewModel>();
            var mainWindow = Services.GetRequiredService<MainWindow>();

            // 2. Связываем их между собой (разорвав ту самую циклическую зависимость)
            mainViewModel.CurrentPage = registerViewModel;
            mainWindow.DataContext = mainViewModel;

            // 3. Показываем окно
            mainWindow.Show();
        }
    }

}
