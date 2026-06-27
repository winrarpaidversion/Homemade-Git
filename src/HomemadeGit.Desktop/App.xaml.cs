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
            services.AddHttpClient();
    
            services.AddSingleton<AuthClientService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<GitViewModel>();

            return services.BuildServiceProvider();
        }
    }

}
