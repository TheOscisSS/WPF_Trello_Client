using DevExpress.Mvvm;
using WebSocketSharp;
using System.Diagnostics;
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

        public ICommand ComeToRegisterPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Register());
        });
        public ICommand ComeToHomePage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Home());
            _pageService.ChangeStatus(true);
        });
        public ICommand SendCredentialsToServer => new AsyncCommand(async () =>
        {
            Debug.WriteLine($"Your credentials are {EmailInputField} : {PasswordInputField}");
        });
    }
}
