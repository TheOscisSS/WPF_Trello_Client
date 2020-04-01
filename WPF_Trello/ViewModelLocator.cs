using Microsoft.Extensions.DependencyInjection;
using WPF_Trello.Services;
using WPF_Trello.ViewModels;

namespace WPF_Trello
{
    public class ViewModelLocator
    {
        private static ServiceProvider _provider;

        public static void Init()
        {
            var services = new ServiceCollection();

            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddScoped<WelcomeViewModel>();
            services.AddScoped<HomeViewModel>();

            services.AddSingleton<PageService>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<EventBusService>();
            services.AddSingleton<MessageBusService>();


            _provider = services.BuildServiceProvider();

            foreach (var item in services)
            {
                _provider.GetRequiredService(item.ServiceType);
            }
        }

        public MainViewModel MainViewModel => _provider.GetRequiredService<MainViewModel>();
        public LoginViewModel LoginViewModel => _provider.GetRequiredService<LoginViewModel>();
        public RegisterViewModel RegisterViewModel => _provider.GetRequiredService<RegisterViewModel>();
        public HomeViewModel HomeViewModel => _provider.GetRequiredService<HomeViewModel>();
        public WelcomeViewModel WelcomeViewModel => _provider.GetRequiredService<WelcomeViewModel>();
    }
}
