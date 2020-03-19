using DevExpress.Mvvm;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        public Page PageSource { get; set; }
        private bool _isAuthenticated;
        public bool IsAuthenticated {
            get => Thread.CurrentPrincipal.Identity.IsAuthenticated;
            set => _isAuthenticated = value;
        }
        
        
        public MainViewModel(PageService pageService, AuthenticationService authenticationService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;

            _pageService.OnPageChanged += (page) => PageSource = page;
            _pageService.OnStatusChanged += (status) => IsAuthenticated = status;
            _pageService.ChangePage(new Login());
            _pageService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        }

        private void Logout()
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                _pageService.ChangePage(new Login());
            }
        }

        public ICommand LogoutButton => new AsyncCommand(async () =>
        {
            Logout();
            _pageService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        });
    }
}
