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

        
            services.AddTransient<MainViewModel>();
            services.AddTransient<SelectingViewModel>();

            return services.BuildServiceProvider();
        }
    }

}
