using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using HomemadeGit.Desktop.ViewsModel;
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
            services.AddSingleton<fakeauth>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<GitViewModel>();

            return services.BuildServiceProvider();
        }
    }

}
