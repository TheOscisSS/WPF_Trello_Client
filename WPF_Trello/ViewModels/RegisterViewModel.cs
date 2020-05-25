using DevExpress.Mvvm;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Trello.Events;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly WebSocketService _webSocketService;

        public string UsernameInputField { get; set; }
        public string PasswordInputField { get; set; }
        public string ConfirmPasswordInputField { get; set; }
        public string ShowStatus { get; set; }
        public bool isAuthError => ShowStatus.Equals(string.Empty) ? false : true;

        public RegisterViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
            WebSocketService webSocketService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _webSocketService = webSocketService;

            ShowStatus = string.Empty;
        }

        private async Task Registration()
        {
            ShowStatus = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(UsernameInputField) ||
                    string.IsNullOrEmpty(PasswordInputField) ||
                    string.IsNullOrEmpty(ConfirmPasswordInputField)
                    )
                {
                    ShowStatus = "Please, fill all fields";
                    return;
                }

                if (!new Regex(@"^[a-zA-Z][a-zA-Z0-9]{3,13}$").IsMatch(UsernameInputField))
                {
                    ShowStatus = "Username can't containe special sumbols and was less then 3 characters";
                    return;
                }

                if (PasswordInputField != ConfirmPasswordInputField)
                {
                    ShowStatus = "Passwords must match";
                    return;
                }

                if (!new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})").IsMatch(PasswordInputField))
                {
                    ShowStatus = "Password is too easy";
                    return;
                }

                _eventBusService.Publish(new WaitingResponseEvent());

                User user = await _authenticationService.CreateUser(UsernameInputField, PasswordInputField, ConfirmPasswordInputField);

                _authenticationService.SetCustomPrincipal(user);
                _webSocketService.JoinIntoAccount(user);

                UsernameInputField = string.Empty;
                PasswordInputField = string.Empty;
                ConfirmPasswordInputField = string.Empty;
                ShowStatus = string.Empty;

                _pageService.ChangePage(new Home());

                await _eventBusService.Publish(new CreatedUserEvent());
            }
            catch (UnauthorizedAccessException e)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                Debug.WriteLine(e.Message);
                ShowStatus = e.Message;
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                Debug.WriteLine(ex.Message);
                ShowStatus = "Something was wrong";
            }
        }

        public ICommand RegistrationButton => new AsyncCommand(async () =>
        {
            await Registration();
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);

            _eventBusService.Publish(new ResponseReceivedEvent());
        });
        public ICommand ComeToLoginPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Login());
        });
    }
}
