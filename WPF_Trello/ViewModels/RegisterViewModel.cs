using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public string UsernameInputField { get; set; }
        public string PasswordInputField { get; set; }
        public string ConfirmPasswordInputField { get; set; }
        public string ShowStatus { get; set; }
        public bool isAuthError => ShowStatus.Equals(string.Empty) ? false : true;

        public RegisterViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;

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

                User user = await _authenticationService.CreateUser(UsernameInputField, PasswordInputField);

                _authenticationService.SetCustomPrincipal(user);

                UsernameInputField = string.Empty;
                PasswordInputField = string.Empty;
                ConfirmPasswordInputField = string.Empty;
                ShowStatus = string.Empty;

                _pageService.ChangePage(new Home());

                await _eventBusService.Publish(new CreatedUserEvent());
            }
            catch (UnauthorizedAccessException e)
            {
                ShowStatus = e.Message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowStatus = "Something was wrong";
            }
        }

        public ICommand RegistrationButton => new AsyncCommand(async () =>
        {
            await Registration();
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        });
        public ICommand ComeToLoginPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Login());
        });
    }
}
