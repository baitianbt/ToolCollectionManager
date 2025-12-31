using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using ToolCollectionManager.Data;
using ToolCollectionManager.ViewModels;
using ToolCollectionManager.Services;

namespace ToolCollectionManager
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; private set; }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            // Initialize Database
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            
            // Services
            services.AddTransient<ISoftwareService, SoftwareService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            // Views
            services.AddTransient<MainWindow>();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
        }
    }
}
