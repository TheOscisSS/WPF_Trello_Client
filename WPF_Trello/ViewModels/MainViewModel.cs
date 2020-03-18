using DevExpress.Mvvm;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        public Page PageSource { get; set; }
        public bool IsAuthenticated
        {
            get => Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }
        public MainViewModel(PageService pageService, AuthenticationService authenticationService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;

            _pageService.OnPageChanged += (page) => PageSource = page;
            //_pageService.OnStatusChanged += (trigger) => IsAuth = trigger; 
            _pageService.ChangePage(new Login());
            //_pageService.ChangeStatus(false);
        }
        public ICommand ComeToLoginPage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Login());
            _pageService.ChangeStatus(false);
        });
    }
}
