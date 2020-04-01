using DevExpress.Mvvm;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;
using WPF_Trello.Utils;
using WPF_Trello.Messages;

namespace WPF_Trello.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly MessageBusService _messageBusService;
        public Page PageSource { get; set; }
        private bool _isAuthenticated;
        public bool IsAuthenticated {
            get => Thread.CurrentPrincipal.Identity.IsAuthenticated;
            set => _isAuthenticated = value;
        }
        
        
        public MainViewModel(PageService pageService, AuthenticationService authenticationService, MessageBusService messageBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _messageBusService = messageBusService;

            _pageService.OnPageChanged += (page) => PageSource = page;
            _authenticationService.OnStatusChanged += (status) => IsAuthenticated = status;
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);

            VerificyUserToken();
        }
        private async void VerificyUserToken()
        {
            try
            {
                if (File.Exists(AccessToken.FILENAME))
                {
                    User user = await _authenticationService.GetCurrentUser();

                    _authenticationService.SetCustomPrincipal(user);

                    _pageService.ChangePage(new Welcome());
                }
                else
                {
                    _pageService.ChangePage(new Login());
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine(e.Message);
                await _messageBusService.SendTo<WelcomeViewModel>(new TextMessage(e.Message));
                _pageService.ChangePage(new Welcome());
                //_pageService.ChangePage(new Login());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void Logout()
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                _pageService.ChangePage(new Login());
            }
        }

        public ICommand LogoutButton => new AsyncCommand(async () =>
        {
            Logout();
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        });
    }
}
