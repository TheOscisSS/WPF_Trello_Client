using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly PageService _pageService;
        public string UsernameInputField { get; set; }
        public string PasswordInputField { get; set; }
        public string ConfirmPasswordInputField { get; set; }
        public RegisterViewModel(PageService pageService)
        {
            _pageService = pageService;
        }

        public ICommand ComeToLoginPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Login());
        });
        public ICommand SendCredentialsToServer => new DelegateCommand(() =>
        {
            Debug.WriteLine($"Your credentials are {UsernameInputField} : {PasswordInputField} - {ConfirmPasswordInputField}");


        });
    }
}
