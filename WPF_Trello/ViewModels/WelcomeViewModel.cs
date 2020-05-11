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
using WPF_Trello.Utils;
using WPF_Trello.Events;

namespace WPF_Trello.ViewModels
{
    public class WelcomeViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly MessageBusService _messageBusService;
        private readonly EventBusService _eventBusService;
        private readonly WebSocketService _webSocketService;

        public string DisplayMessage { get; set; }
        public Page PageSource { get; set; }

        public WelcomeViewModel(PageService pageService, AuthenticationService authenticationService, MessageBusService messageBusService,
            EventBusService eventBusService, WebSocketService webSocketService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _messageBusService = messageBusService;
            _eventBusService = eventBusService;
            _webSocketService = webSocketService;

            _pageService.OnPageChanged += (page) => PageSource = page;

            _messageBusService.Receive<TextMessage>(this, async message =>
            {
                DisplayMessage = message.Message;
            });
        }

        public ICommand ContinueButton => new AsyncCommand(async () =>
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                AccessToken.Remove();
                _pageService.ChangePage(new Login());
                return;
            }

            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);

            _pageService.ChangePage(new Home());
            //_pageService.ChangePage(new Pages.Board());

            await _eventBusService.Publish(new GetBoardsEvent());
        });
    }
}
