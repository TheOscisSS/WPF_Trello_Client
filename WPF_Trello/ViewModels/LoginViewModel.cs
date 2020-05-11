using DevExpress.Mvvm;
using System.Diagnostics;
using System.Windows.Input;
using WPF_Trello.Pages;
using WPF_Trello.Services;
using System;
using WPF_Trello.Models;
using System.Threading;
using WPF_Trello.Events;
using System.Threading.Tasks;

namespace WPF_Trello.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly WebSocketService _webSocketService;

        public string UsernameInputField { get; set; }
        public string PasswordInputField { get; set; }
        public string ShowStatus { get; set; }
        public bool isAuthError => ShowStatus.Equals(string.Empty) ? false : true;

        public LoginViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                WebSocketService webSocketService)
        {

            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _webSocketService = webSocketService;

            ShowStatus = string.Empty;
        }

        private async Task Login()
        {
            ShowStatus = string.Empty;
            
            try
            {
                if (string.IsNullOrEmpty(UsernameInputField) || string.IsNullOrEmpty(PasswordInputField))
                {
                    ShowStatus = "Please, fill all fields";
                    return;
                }

                User user = await _authenticationService.AuthenticateUser(UsernameInputField, PasswordInputField);

                _authenticationService.SetCustomPrincipal(user);
                _webSocketService.JoinIntoAccount(user);

                UsernameInputField = string.Empty;
                PasswordInputField = string.Empty;
                ShowStatus = string.Empty;

                _pageService.ChangePage(new Home());

                await _eventBusService.Publish(new AuthorizatedEvent());
                await _eventBusService.Publish(new GetBoardsEvent());
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine(e.Message);
                ShowStatus = e.Message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowStatus = "Something was wrong";
            }
        }

        public ICommand ComeToRegisterPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Register());
        });
        public ICommand LoginButton => new AsyncCommand(async () =>
        {
            await Login();
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);

        });
        public ICommand SendCredentialsToServer => new AsyncCommand(async () =>
        {
            Debug.WriteLine($"Your credentials are {UsernameInputField} : {PasswordInputField}");
        });
    }
}
