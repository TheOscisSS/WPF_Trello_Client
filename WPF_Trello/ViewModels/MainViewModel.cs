using DevExpress.Mvvm;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;
using WPF_Trello.Utils;
using WPF_Trello.Messages;
using WPF_Trello.Events;
using System.Collections.ObjectModel;

namespace WPF_Trello.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly MessageBusService _messageBusService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;

        private bool _isAuthenticated;
        private PictureExample _selectedPicture;

        public Page PageSource { get; set; }
        public ObservableCollection<PictureExample> RandomPictureCollection { get; set; }
        
        
        public MainViewModel(PageService pageService, AuthenticationService authenticationService, MessageBusService messageBusService,
            EventBusService eventBusService, BoardService boardService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _messageBusService = messageBusService;
            _eventBusService = eventBusService;
            _boardService = boardService;

            IsAddBoardTrigger = false;
            NewBoardTitle = string.Empty;
            SelectedPicture = null;

            _pageService.OnPageChanged += (page) => PageSource = page;
            _authenticationService.OnStatusChanged += (status) => IsAuthenticated = status;
            _eventBusService.Subscribe<AuthorizatedEvent>(async _ =>
            {
                _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
                FirstChapterFromName = Thread.CurrentPrincipal.Identity.Name[0];
            });
            _eventBusService.Subscribe<CreatedUserEvent>(async _ =>
            {
                _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
                FirstChapterFromName = Thread.CurrentPrincipal.Identity.Name[0];
            });

            _messageBusService.Receive<NewBoardResponseMessage>(this, async message =>
            {
                if (!message.Status)
                {
                    Debug.WriteLine(message.ResponseMessage);
                }
                else
                {
                    IsAddBoardTrigger = false;
                    NewBoardTitle = string.Empty;
                    SelectedPicture = null;
                }
            });

            VerificyUserToken();
        }
        private async void GetPictureExamples()
        {
            try
            {
                RandomPictureCollection = await _boardService.GetRandomPicture();
            }
            catch (Exception ex)
            {
                RandomPictureCollection = new ObservableCollection<PictureExample>();
                Debug.WriteLine(ex.Message);
            }
        }
        private async void VerificyUserToken()
        {
            try
            {
                if (AccessToken.isExist())
                {
                    User user = await _authenticationService.GetCurrentUser();

                    _authenticationService.SetCustomPrincipal(user);

                    await _messageBusService.SendTo<WelcomeViewModel>(new TextMessage("Welcome back " + user.Username));

                    FirstChapterFromName = Thread.CurrentPrincipal.Identity.Name[0];

                    _pageService.ChangePage(new Welcome());
                }
                else
                {
                    _pageService.ChangePage(new Login());
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine(e.Message);
                await _messageBusService.SendTo<WelcomeViewModel>(new TextMessage(e.Message));
                _pageService.ChangePage(new Welcome());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void Logout()
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                _pageService.ChangePage(new Login());
            }
            AccessToken.Remove();
        }

        public ICommand ToHomePageButton => new AsyncCommand(async () =>
        {
            await _eventBusService.Publish(new GoToHomeEvent());
            _pageService.ChangePage(new Home());
        });

        public ICommand LogoutButton => new AsyncCommand(async () =>
        {
            Logout();
            _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
        });

        public ICommand ShowCreateNewBoardCommand => new AsyncCommand(async () =>
        {
            if (RandomPictureCollection == null)
            {
                GetPictureExamples();
            }
            IsAddBoardTrigger = true;
        });
        public ICommand HideCreateNewBoardCommand => new AsyncCommand(async () =>
        {
            IsAddBoardTrigger = false;
        });
        public ICommand CreateNewBoard => new AsyncCommand(async () =>
        {
            if (RandomPictureCollection == null)
            {
                GetPictureExamples();
            }
            IsAddBoardTrigger = true;
        });
        public ICommand RefreshPictureCommand => new AsyncCommand(async () =>
        {
            SelectedPicture = null;
            GetPictureExamples();
        });
        public ICommand AddNewBoardCommand => new AsyncCommand(async () =>
        {
            if (Uri.IsWellFormedUriString(BoardBackgrounPicture, UriKind.Absolute))
            {
                await _messageBusService.SendTo<HomeViewModel>(new CreateBoardMessage(NewBoardTitle, BoardBackgrounPicture));
            }
            else
            {
                Debug.WriteLine("URL has the wrong format");
                //TODO: Add error handler
            }
        });
        public char FirstChapterFromName { get; private set; }
        public bool IsAddBoardTrigger { get; private set; }
        public string BoardBackgrounPicture { get; set; }
        public string NewBoardTitle { get; set; }
        public PictureExample SelectedPicture
        {
            get => _selectedPicture;
            set
            {
                _selectedPicture = value;
                if(_selectedPicture != null)
                {
                    BoardBackgrounPicture = _selectedPicture.Regular;
                    RaisePropertiesChanged("SelectedPicture");
                    RaisePropertiesChanged("BoardBackgrounPicture");
                }
            }
        }
        public bool IsAuthenticated
        {
            get => Thread.CurrentPrincipal.Identity.IsAuthenticated;
            set => _isAuthenticated = value;
        }
    }
}
