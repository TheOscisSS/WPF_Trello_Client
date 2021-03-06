﻿using Microsoft.Extensions.DependencyInjection;
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

            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();

            services.AddScoped<WelcomeViewModel>();
            services.AddScoped<MainViewModel>();
            services.AddScoped<HomeViewModel>();
            services.AddScoped<BoardViewModel>();

            services.AddSingleton<PageService>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<EventBusService>();
            services.AddSingleton<MessageBusService>();
            services.AddSingleton<BoardService>();
            services.AddSingleton<WebSocketService>();

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
        public BoardViewModel BoardViewModel => _provider.GetRequiredService<BoardViewModel>();
    }
}
