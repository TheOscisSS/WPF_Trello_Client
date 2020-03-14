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
    public class LoginViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public string EmailInputField { get; set; }
        public string PasswordInputField { get; set; }

        public LoginViewModel(PageService pageService)
        {
            _pageService = pageService;
        }

        public ICommand ChangePage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Register());
        });

        public ICommand SendCredentialsToServer => new DelegateCommand(() =>
        {
            Debug.WriteLine($"Your email is {EmailInputField}");
        });
    }
}
