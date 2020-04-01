using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;

        public string UsernameInputField { get; set; }
        public string PasswordInputField { get; set; }
        public string ConfirmPasswordInputField { get; set; }
        public string ShowStatus { get; set; }
        public bool isAuthError => ShowStatus.Equals(string.Empty) ? false : true;

        public RegisterViewModel(PageService pageService, AuthenticationService authenticationService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            ShowStatus = string.Empty;
        }

        private void Registration()
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

                //User user = _authenticationService.CreateNewUser(UsernameInputField, PasswordInputField);

                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                {
                    throw new ArgumentException("Thread principal must be set to CustomerPrincipal");
                }

                //customPrincipal.Identity = new CustomIdentity(user.Username, user.Roles);
                //UsernameInputField = string.Empty;
                //PasswordInputField = string.Empty;
                //ShowStatus = string.Empty;

                //_pageService.ChangePage(new Home());
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowStatus = ex.Message;
            }
            catch (Exception ex)
            {
                ShowStatus = $"ERROR: {ex.Message}";
            }
        }

        public ICommand RegistrationButton => new DelegateCommand(() =>
        {
            Registration();
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        });
        public ICommand ComeToLoginPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Login());
        });
    }
}
