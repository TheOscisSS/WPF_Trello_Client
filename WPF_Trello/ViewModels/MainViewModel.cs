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
using System.Threading.Tasks;
using System.Linq;

namespace WPF_Trello.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly MessageBusService _messageBusService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;
        private readonly WebSocketService _webSocketService;

        private bool _isAuthenticated;
        private PictureExample _selectedPicture;

        public Page PageSource { get; set; }
        public ObservableCollection<PictureExample> RandomPictureCollection { get; set; }
        public ObservableCollection<UserNotification> UserNotificationCollection { get; set; }
        public UserNotification SelectedUserNotification { get; set; }
        
        public MainViewModel(PageService pageService, AuthenticationService authenticationService, MessageBusService messageBusService,
            EventBusService eventBusService, BoardService boardService, WebSocketService webSocketService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _messageBusService = messageBusService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _webSocketService = webSocketService;

            SelectedUserNotification = null;
            UserNotificationCollection = new ObservableCollection<UserNotification>();
            IsExistUnreadNotification = false;
            IsShowUserInfo = false;
            IsShowUserInfoAddIcon = false;
            IsAddBoardTrigger = false;
            IsOpenNotificationsTrigger = false;
            IsShowAllNotifications = false;
            NewBoardTitle = string.Empty;
            SelectedPicture = null;

            _pageService.OnPageChanged += (page) => PageSource = page;
            _authenticationService.OnStatusChanged += (status) => IsAuthenticated = status;
            _eventBusService.Subscribe<AuthorizatedEvent>(async _ =>
            {
                _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
                CurrentUser = _authenticationService.CurrentUser;
            });
            _eventBusService.Subscribe<CreatedUserEvent>(async _ =>
            {
                _authenticationService.ChangeStatus(Thread.CurrentPrincipal.Identity.IsAuthenticated);
                CurrentUser = _authenticationService.CurrentUser;
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
            _messageBusService.Receive<AddNewNotificationMessage>(this, async message =>
            {
                UserNotificationCollection.Add(message.UserNotification);
                CheckIsUnreadNotificationExist();
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
                    _webSocketService.JoinIntoAccount(user);
                    _authenticationService.SetCustomPrincipal(user);

                    CurrentUser = _authenticationService.CurrentUser;
                    await _messageBusService.SendTo<WelcomeViewModel>(new SendUserCredentialMessage(CurrentUser, "Welcome back " + user.Username));

                    GetUserNotificatons();

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
                await _messageBusService.SendTo<WelcomeViewModel>(new SendUserCredentialMessage(e.Message));
                _pageService.ChangePage(new Welcome());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private async void GetUserNotificatons()
        {
            try
            {
                UserNotificationCollection = await _boardService.GetAllUserNotifications();
                RaisePropertiesChanged("UserNotificationCollection");
                CheckIsUnreadNotificationExist();
            }
            catch (Exception ex)
            {
                //TODO: add error handler
                Debug.WriteLine(ex.Message);
            }
        }
        private void CheckIsUnreadNotificationExist()
        {
            var UnreadNotifications = UserNotificationCollection
                .Where(notification => notification.IsReaded == false)
                .Select(notification => notification);
            if (UnreadNotifications.Count() > 0)
            {
                IsExistUnreadNotification = true;
            }
            else
            {
                IsExistUnreadNotification = false;
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
            IsShowUserInfo = false;
            _webSocketService.LeaveFromAccount();
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
        public ICommand AddUserBackgroundPicture => new AsyncCommand(async () =>
        {
            if (Uri.IsWellFormedUriString(UserBackroundPicture, UriKind.Absolute))
            {
                CurrentUser = await _authenticationService.SetUserIcon(UserBackroundPicture);
                IsShowUserInfoAddIcon = false;
                RaisePropertiesChanged("CurrentUser");
                RaisePropertiesChanged("IsShowUserInfoAddIcon");
                //await _messageBusService.SendTo<HomeViewModel>(new CreateBoardMessage(NewBoardTitle, BoardBackgrounPicture));
            }
            else
            {
                Debug.WriteLine("URL has the wrong format");
                //TODO: Add error handler
            }
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

        public ICommand ShowNotificationsCommand => new AsyncCommand(async () =>
        {
            IsShowUserInfo = false;
            IsOpenNotificationsTrigger = true;
        });
        public ICommand HideNotificationsCommand => new AsyncCommand(async () =>
        {
            IsOpenNotificationsTrigger = false;
        });
        public ICommand ShowUserInfoCommand => new AsyncCommand(async () =>
        {

            IsShowUserInfo = true;
            IsOpenNotificationsTrigger = false;
        });
        public ICommand HideUserInfoCommand => new AsyncCommand(async () =>
        {
            IsShowUserInfo = false;
        });
        public ICommand ShowUserInfIconCommand => new AsyncCommand(async () =>
        {

            IsShowUserInfoAddIcon = true;
        });
        public ICommand HideUserInfoIconCommand => new AsyncCommand(async () =>
        {
            IsShowUserInfoAddIcon = false;
        });
        public ICommand ShowAllNotificationsCommand => new AsyncCommand(async () =>
        {

            IsShowAllNotifications = true;
        });
        public ICommand ShowOnlyUnreadNotificationsCommand => new AsyncCommand(async () =>
        {
            IsShowAllNotifications = false;
        });
        public ICommand ReadNotificationCommand => new AsyncCommand(async () =>
        {
            try
            {
                _boardService.ReadNotificationById(string.Copy(SelectedUserNotification.ID));
                SelectedUserNotification.ReadNotification();
                CheckIsUnreadNotificationExist();
            }
            catch (Exception ex)
            {
                //TODO: add error handler
                Debug.WriteLine(ex.Message);
            }
        });

        public User CurrentUser { get; private set; }
        public bool IsAddBoardTrigger { get; private set; }
        public bool IsShowUserInfo { get; private set; }
        public bool IsOpenNotificationsTrigger { get; private set; }
        public bool IsShowAllNotifications { get; private set; }
        public bool IsShowUserInfoAddIcon { get; private set; }
        public bool IsExistUnreadNotification { get; private set; }
        public string UserBackroundPicture { get; set; }
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
