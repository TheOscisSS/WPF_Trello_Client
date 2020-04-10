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
using WPF_Trello.Events;

namespace WPF_Trello.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly MessageBusService _messageBusService;
        private readonly EventBusService _eventBusService;

        public Page PageSource { get; set; }
        private bool _isAuthenticated;
        public char FirstChapterFromName { get; private set; }
        public bool IsAuthenticated {
            get => Thread.CurrentPrincipal.Identity.IsAuthenticated;
            set => _isAuthenticated = value;
        }
        
        public MainViewModel(PageService pageService, AuthenticationService authenticationService, MessageBusService messageBusService,
            EventBusService eventBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _messageBusService = messageBusService;
            _eventBusService = eventBusService;

            _pageService.OnPageChanged += (page) => PageSource = page;
            _authenticationService.OnStatusChanged += (status) => IsAuthenticated = status;
            _eventBusService.Subscribe<AuthorizatedEvent>(async _ =>
            {
                _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
                FirstChapterFromName = Thread.CurrentPrincipal.Identity.Name[0];
            });
            _eventBusService.Subscribe<CreatedUserEvent>(async _ =>
            {
                _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
                FirstChapterFromName = Thread.CurrentPrincipal.Identity.Name[0];
            });

            VerificyUserToken();
        }
        private async void VerificyUserToken()
        {
            try
            {
                if (AccessToken.isExist())
                {
                    User user = await _authenticationService.GetCurrentUser();

                    _authenticationService.SetCustomPrincipal(user);

                    await _messageBusService.SendTo<WelcomeViewModel>(new TextMessage("Welcome back " + user.Username));

                    FirstChapterFromName = Thread.CurrentPrincipal.Identity.Name[0];

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
            AccessToken.Remove();
        }

        public ICommand ToHomePageButton => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Home());
        });

        public ICommand LogoutButton => new AsyncCommand(async () =>
        {
            Logout();
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        });
    }
}
