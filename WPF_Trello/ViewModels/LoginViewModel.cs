using DevExpress.Mvvm;
using WebSocketSharp;
using System.Diagnostics;
using System.Windows.Input;
using WPF_Trello.Pages;
using WPF_Trello.Services;
using System;
using WPF_Trello.Models;
using System.Threading;

namespace WPF_Trello.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;

        public string UsernameInputField { get; set; }
        public string PasswordInputField { get; set; }
        public string ShowStatus { get; set; }
        public bool isAuthError => ShowStatus.Equals(string.Empty) ? false : true;

        public LoginViewModel(PageService pageService, AuthenticationService authenticationService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            ShowStatus = string.Empty;
        }

        private void Login()
        {
            ShowStatus = string.Empty;

            try
            {
                if(UsernameInputField.IsNullOrEmpty() || PasswordInputField.IsNullOrEmpty())
                {
                    ShowStatus = "Please, fill all fields";
                    return;
                }

                User user = _authenticationService.AuthenticateUser(UsernameInputField, PasswordInputField);

                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if(customPrincipal == null)
                {
                    throw new ArgumentException("Thread principal must be set to CustomerPrincipal");
                }

                customPrincipal.Identity = new CustomIdentity(user.Username, user.Roles);
                UsernameInputField = string.Empty;
                PasswordInputField = string.Empty;
                ShowStatus = string.Empty;


                _pageService.ChangePage(new Home());

            }
            catch (UnauthorizedAccessException)
            {
                ShowStatus = "Invalid username or password";
            }
            catch(Exception ex)
            {
                ShowStatus = $"ERROR: {ex.Message}";
            }
        }

        public ICommand ComeToRegisterPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Register());
        });
        public ICommand LoginButton => new AsyncCommand(async () =>
        {
            Login();
            _pageService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        });
        public ICommand SendCredentialsToServer => new AsyncCommand(async () =>
        {
            Debug.WriteLine($"Your credentials are {UsernameInputField} : {PasswordInputField}");
        });
    }
}
