using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_Trello.Pages;
using WPF_Trello.Services;
using WPF_Trello.Messages;

namespace WPF_Trello.ViewModels
{
    public class WelcomeViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly MessageBusService _messageBusService;

        public string DisplayMessage { get; set; }
        public Page PageSource { get; set; }

        public WelcomeViewModel(PageService pageService, AuthenticationService authenticationService, MessageBusService messageBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _messageBusService = messageBusService;

            _pageService.OnPageChanged += (page) => PageSource = page;

            DisplayMessage = "Welcome back";

            _messageBusService.Receive<TextMessage>(this, async message =>
            {
                DisplayMessage = message.Message;
            });
        }

        public ICommand ContinueButton => new AsyncCommand(async () =>
        {
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
            _pageService.ChangePage(new Home());
        });
    }
}
