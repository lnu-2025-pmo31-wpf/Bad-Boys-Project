using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using BadBoys.BLL.Services;
using BadBoys.DAL;
using BadBoys.Presentation.WPF.ViewModels;

namespace BadBoys.Presentation.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public App()
        {
            var services = new ServiceCollection();

            // DAL
            services.AddDbContext<AppDbContext>();

            // BLL
            services.AddSingleton<MediaService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<MediaListViewModel>();

            // Views
            services.AddTransient<Views.MediaListPage>();
            services.AddTransient<MainWindow>();

            Services = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var wnd = Services.GetRequiredService<MainWindow>();
            wnd.Show();
        }
    }
}
