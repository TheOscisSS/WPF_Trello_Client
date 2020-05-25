using DevExpress.Mvvm;
using GongSolutions.Wpf.DragDrop;
using Markdig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Trello.Events;
using WPF_Trello.Exceptions;
using WPF_Trello.Messages;
using WPF_Trello.Models;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class BoardViewModel : BindableBase, IDropTarget
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;
        private readonly MessageBusService _messageBusService;
        private readonly WebSocketService _webSocketService;

        public string CardDetailID { get; private set; }
        public string CardDetailTitle { get; private set; }
        public string CardDetailCurrentList { get; private set; }
        public string CardDetailDescription { get; private set; }
        public string CardDetail { get; private set; }
        public string DescriptionTextBox { get; set; }
        public string CardDetailChangedBy { get; private set; }
        public string NewListTitle { get; set; }
        public string InviteMemberName { get; set; }
        public Models.Board CurrentBoard { get; private set; }
        public ObservableCollection<User> BoardMembers { get; set; }
        public BoardList SelectedList { get; set; }
        public BoardCard SelectedCard { get; set; }
        public User SelectedMember { get; set; }
        public bool IsAddListTrigger { get; private set; }
        public bool IsMenuTrigger { get; private set; }
        public bool IsInviteMemberTrigger { get; private set; }
        public bool IsShowCardDetails { get; private set; }
        public bool IsEditingDescription { get; private set; }
        public bool IsExistConflictChanges { get; private set; }
        public bool IsShowMoreBoardDetails { get; private set; }
        public bool IsBoardOwner { get; private set; }

        public BoardViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                BoardService boardService, MessageBusService messageBusService, WebSocketService webSocketService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _messageBusService = messageBusService;
            _webSocketService = webSocketService;

            InitializeProperties();

            _eventBusService.Subscribe<LogOutEvent>(async _ =>
            {
                InitializeProperties();
            });

            _messageBusService.Receive<BoardPreloadMessage>(this, async message =>
            {
                _eventBusService.Publish(new WaitingResponseEvent());

                CurrentBoard = new Models.Board(message.ID, message.Title, message.Description, message.Background, message.CreatedAt, message.UpdatedAt);
                BoardMembers = new ObservableCollection<User>();
                IsShowMoreBoardDetails = false;
                RenderBoard(message.ID);
            });
            _messageBusService.Receive<AddNewActivityMessage>(this, async message =>
            {
                CurrentBoard.AddNewActivity(message.AddedActivity);   
            });
            _messageBusService.Receive<AddNewMemberMessage>(this, async message =>
            {
                if(_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    string alertMessage = $"User {message.InvitedMember.Username} was invited";
                    string alertStatus = AlertStatus.SUCCESS;

                    Alert alert = new Alert(alertMessage, alertStatus);
                    _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }

                BoardMembers.Add(message.InvitedMember);
            });
            _messageBusService.Receive<AddNewListMessage>(this, async message =>
            {
                if (_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    //string alertMessage = $"List {message.BoardList.Title} successfully added";
                    //string alertStatus = AlertStatus.SUCCESS;

                    //Alert alert = new Alert(alertMessage, alertStatus);
                    //_messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }

                NewListTitle = string.Empty;
                CurrentBoard.AddNewList(message.BoardList);
            });
            _messageBusService.Receive<AddNewCardMessage>(this, async message =>
            {
                if(_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    //string alertMessage = $"Card {message.BoardCard.Title} successfully added";
                    //string alertStatus = AlertStatus.SUCCESS;

                    var selectedListByID = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ListID);

                    selectedListByID.AddNewCard(message.BoardCard);
                    selectedListByID.NewCardTitle = string.Empty;

                    //Alert alert = new Alert(alertMessage, alertStatus);
                    //_messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }
                else
                {
                    var ListByID = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ListID);
                    ListByID.AddNewCard(message.BoardCard);
                }
            });
            _messageBusService.Receive<BoardDeleteMemberMessage>(this, async message =>
            {
                var deletedMemberById = BoardMembers.FirstOrDefault(member => member.ID == message.memberID);

                if (_authenticationService.CurrentUser.ID == message.senderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    string alertMessage = $"User {deletedMemberById.Username} was kicked out";
                    string alertStatus = AlertStatus.SUCCESS;

                    Alert alert = new Alert(alertMessage, alertStatus);
                    _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                }
                BoardMembers.Remove(deletedMemberById);
            });
            _messageBusService.Receive<KickOutMemberMessage>(this, async message =>
            {
                if (CurrentBoard.ID == message.BoardID)
                {
                    string alertMessage = $"You has been kicked out from {CurrentBoard.Title}";
                    string alertStatus = AlertStatus.WARNING;

                    Alert alert = new Alert(alertMessage, alertStatus);
                    _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                    await _eventBusService.Publish(new GoToHomeEvent());
                    _pageService.ChangePage(new Home());
                }
            });
            _messageBusService.Receive<DeleteBoardListMessage>(this, async message =>
            {
                var deletedBoardListById = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ListID);

                if (_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    //string alertMessage = $"List {deletedBoardListById.Title} successfully deleted";
                    //string alertStatus = AlertStatus.SUCCESS;

                    //Alert alert = new Alert(alertMessage, alertStatus);
                    //_messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }

                if (IsShowCardDetails && deletedBoardListById.Cards.Contains(SelectedCard))
                {
                    IsShowCardDetails = false;
                    CardDetailCurrentList = string.Empty;
                    CardDetailDescription = string.Empty;
                    DescriptionTextBox = string.Empty;
                    CardDetailChangedBy = string.Empty;
                    IsExistConflictChanges = false;
                    IsEditingDescription = false;
                }

                CurrentBoard.Lists.Remove(deletedBoardListById);

            });
            _messageBusService.Receive<DeleteBoardCardMessage>(this, async message =>
            {
                var boardListContailerById = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ListID);
                var deletedBoardCardById = boardListContailerById.Cards.FirstOrDefault(card => card.ID == message.CardID);

                if (_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    //string alertMessage = $"Card {deletedBoardCardById.Title} successfully deleted";
                    //string alertStatus = AlertStatus.SUCCESS;

                    //Alert alert = new Alert(alertMessage, alertStatus);
                    //_messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }

                if (IsShowCardDetails && deletedBoardCardById.ID == SelectedCard?.ID)
                {
                    IsShowCardDetails = false;
                    CardDetailCurrentList = string.Empty;
                    CardDetailDescription = string.Empty;
                    DescriptionTextBox = string.Empty;
                    CardDetailChangedBy = string.Empty;
                    IsExistConflictChanges = false;
                    IsEditingDescription = false;
                }

                boardListContailerById.Cards.Remove(deletedBoardCardById);


            });
            _messageBusService.Receive<DeleteBoardMessage>(this, async message =>
            {
                if (_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    string alertMessage = $"Board {CurrentBoard.Title} successfully deleted";
                    string alertStatus = AlertStatus.SUCCESS;

                    Alert alert = new Alert(alertMessage, alertStatus);
                    _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }
                else
                {
                    string alertMessage = $"Board {CurrentBoard.Title} was deleted by owner";
                    string alertStatus = AlertStatus.SUCCESS;

                    Alert alert = new Alert(alertMessage, alertStatus);
                    _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }

                InitializeProperties();

                await _eventBusService.Publish(new GoToHomeEvent());
                _pageService.ChangePage(new Home());
            });
            _messageBusService.Receive<AddNewUserIconMessage>(this, async message =>
            {
                if (_authenticationService.CurrentUser.ID == message.SenderID)
                {
                    _eventBusService.Publish(new ResponseReceivedEvent());

                    string alertMessage = $"Icon successfully updated";
                    string alertStatus = AlertStatus.SUCCESS;

                    Alert alert = new Alert(alertMessage, alertStatus);
                    _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                }

                var memberById = BoardMembers.FirstOrDefault(member => member.ID == message.SenderID) as User;
                memberById.SetIcon(message.Icon);
                CurrentBoard.UpdateEachActivityIcon(message.Icon, message.SenderID);

            });
            _messageBusService.Receive<MoveBoardListMessage>(this, async message =>
            {
                var movableList = CurrentBoard.Lists.ElementAt(message.From);

                if(_authenticationService.CurrentUser.ID != message.SenderID)
                {
                    CurrentBoard.Lists.Remove(movableList);
                    CurrentBoard.Lists.Insert(message.To, movableList);
                }
            });
            _messageBusService.Receive<MoveBoardCardMessage>(this, async message =>
            {
                var collection = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ListID);
               
                if (_authenticationService.CurrentUser.ID != message.SenderID)
                {
                    var movableCard = collection.Cards.ElementAt(message.From);
                    var isEditingMovableCard = SelectedCard != null && SelectedCard.ID == movableCard.ID;

                    collection.Cards.Remove(movableCard);
                    collection.Cards.Insert(message.To, movableCard);

                    if (IsShowCardDetails && isEditingMovableCard)
                    {
                        SelectedCard = movableCard;
                        CardDetailCurrentList = collection.Title;
                        RaisePropertiesChanged("SelectedCard");
                    }
                }
            });
            _messageBusService.Receive<MoveCardBetweenListsMessage>(this, async message =>
            {
                var sourceCollection = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.FromListID);
                var targetCollection = CurrentBoard.Lists.FirstOrDefault(list => list.ID == message.ToListID);

                if (_authenticationService.CurrentUser.ID != message.SenderID)
                {
                    var movableCard = sourceCollection.Cards.ElementAt(message.From);
                    var isEditingMovableCard = SelectedCard != null && SelectedCard.ID == movableCard.ID;

                    sourceCollection.Cards.Remove(movableCard);
                    targetCollection.Cards.Insert(message.To, movableCard);

                    if (IsShowCardDetails && isEditingMovableCard)
                    {
                        SelectedCard = movableCard;
                        CardDetailCurrentList = targetCollection.Title;
                        RaisePropertiesChanged("SelectedCard");
                    }
                }
            });
            _messageBusService.Receive<UpdateDescriptionMessage>(this, async message =>
            {
                if(IsShowCardDetails && SelectedCard.ID == message.CardID)
                {
                    if(_authenticationService.CurrentUser.ID != message.SenderID)
                    {
                        SelectedCard.SetDescription(message.Description);
                        CardDetailDescription = Markdown.ToHtml(SelectedCard.Description);

                        if (IsEditingDescription)
                        {
                            var sender = BoardMembers.FirstOrDefault(user => user.ID == message.SenderID);

                            string alertMessage = $"User {sender.Username} has changed description in this card";
                            string alertStatus = AlertStatus.WARNING;

                            Alert alert = new Alert(alertMessage, alertStatus, 3000);
                            _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                            CardDetailChangedBy = sender.Username;
                            IsExistConflictChanges = true;
                        }
                    }
                    else
                    {
                        _eventBusService.Publish(new ResponseReceivedEvent());

                        string alertMessage = $"Description successfully changed";
                        string alertStatus = AlertStatus.SUCCESS;

                        Alert alert = new Alert(alertMessage, alertStatus);
                        _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
                    }
                }
            });
        }

        private void InitializeProperties()
        {
            SelectedMember = null;
            SelectedCard = null;
            IsAddListTrigger = false;
            IsMenuTrigger = false;
            IsInviteMemberTrigger = false;
            IsShowCardDetails = false;
            IsEditingDescription = false;
            IsExistConflictChanges = false;
            IsShowMoreBoardDetails = false;
            NewListTitle = string.Empty;
            InviteMemberName = string.Empty;
            CardDetailID = string.Empty;
            CardDetailTitle = string.Empty;
            CardDetailCurrentList = string.Empty;
            CardDetailDescription = string.Empty;
            DescriptionTextBox = string.Empty;
            CardDetailChangedBy = string.Empty;

            IsBoardOwner = false;
        }

        private async void RenderBoard(string id)
        {
            try
            {
                IsAddListTrigger = false;
                IsInviteMemberTrigger = false;
                NewListTitle = string.Empty;
                InviteMemberName = string.Empty;
                CurrentBoard = await _boardService.GetBoardById(id);
                BoardMembers.Add(CurrentBoard.Owner);
                foreach(User member in CurrentBoard.Members)
                {
                    BoardMembers.Add(member);
                }

                IsBoardOwner = CurrentBoard.Owner.ID == _authenticationService.CurrentUser.ID;
                _eventBusService.Publish(new ResponseReceivedEvent());
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if(dropInfo.Data is BoardList)
            {
                BoardList sourceItem = dropInfo.Data as BoardList;
                BoardList targetItem = dropInfo.TargetItem as BoardList;

                if (sourceItem != null && targetItem != null)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
            else if(dropInfo.Data is BoardCard && dropInfo.TargetItem is BoardCard)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;
                BoardCard targetItem = dropInfo.TargetItem as BoardCard;

                if (sourceItem != null && targetItem != null)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
            else if(dropInfo.TargetCollection is ObservableCollection<BoardCard>)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;
                var targetCollection = dropInfo.TargetCollection as ObservableCollection<BoardCard>;

                if (sourceItem != null && targetCollection != null && targetCollection.Count == 0)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
            
        }

        public void Drop(IDropInfo dropInfo)
        {

            if(dropInfo.Data is BoardList)
            {
                BoardList sourceItem = dropInfo.Data as BoardList;
                BoardList targetItem = dropInfo.TargetItem as BoardList;

                if (sourceItem.ID != targetItem.ID)
                {
                    int sourceIndex = CurrentBoard.Lists.IndexOf(sourceItem);
                    int targetIndex = CurrentBoard.Lists.IndexOf(targetItem);

                    CurrentBoard.Lists.Remove(sourceItem);
                    if (sourceIndex < targetIndex)
                    {
                        int toIndex = dropInfo.InsertIndex - 1 < 0 ? 0 : dropInfo.InsertIndex - 1;
                        SendMoveListData(sourceIndex, toIndex, CurrentBoard.ID);
                        CurrentBoard.Lists.Insert(toIndex, sourceItem);
                    }
                    else
                    {
                        SendMoveListData(sourceIndex, dropInfo.InsertIndex, CurrentBoard.ID);
                        CurrentBoard.Lists.Insert(dropInfo.InsertIndex, sourceItem);
                    }
                }
            }
            else if (dropInfo.Data is BoardCard && dropInfo.TargetItem is BoardCard)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;
                BoardCard targetItem = dropInfo.TargetItem as BoardCard;

                var sourceCollection = dropInfo.DragInfo.SourceCollection as ObservableCollection<BoardCard>;
                var targetCollection = dropInfo.TargetCollection as ObservableCollection<BoardCard>;

                if (sourceItem.ID != targetItem.ID)
                {
                    int sourceIndex = sourceCollection.IndexOf(sourceItem);
                    int targetIndex = targetCollection.IndexOf(targetItem);

                    var sourceListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == sourceCollection);
                    var currentListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == targetCollection);

                    sourceCollection.Remove(sourceItem);

                    if (sourceCollection == targetCollection)
                    {
                       
                        if (sourceIndex < targetIndex)
                        {
                            int toIndex = dropInfo.InsertIndex - 1 < 0 ? 0 : dropInfo.InsertIndex - 1;
                            SendMoveCardData(sourceIndex, toIndex, currentListCollection.ID);
                            targetCollection.Insert(toIndex, sourceItem);
                        }
                        else
                        {
                            SendMoveCardData(sourceIndex, dropInfo.InsertIndex, currentListCollection.ID);
                            targetCollection.Insert(dropInfo.InsertIndex, sourceItem);
                        }
                    }
                    else
                    {
                        SendMoveCardToAnotherListData(sourceIndex, dropInfo.InsertIndex, sourceListCollection.ID, currentListCollection.ID);

                        targetCollection.Insert(dropInfo.InsertIndex, sourceItem);
                    }
                    
                }

                SelectedCard = null;
            }
            else if (dropInfo.TargetCollection is ObservableCollection<BoardCard>)
            {
                BoardCard sourceItem = dropInfo.Data as BoardCard;

                var sourceCollection = dropInfo.DragInfo.SourceCollection as ObservableCollection<BoardCard>;
                var targetCollection = dropInfo.TargetCollection as ObservableCollection<BoardCard>;

                var sourceListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == sourceCollection);
                var currentListCollection = CurrentBoard.Lists.FirstOrDefault(list => list.Cards == targetCollection);

                if (targetCollection.Count == 0)
                {
                    int sourceIndex = sourceCollection.IndexOf(sourceItem);

                    SendMoveCardToAnotherListData(sourceIndex, 0, sourceListCollection.ID, currentListCollection.ID);

                    sourceCollection.Remove(sourceItem);
                    targetCollection.Add(sourceItem);
                }

                SelectedCard = null;
            }
        }
        private async Task<List<string>> SendMoveListData(int from, int to, string boardID)
        {
            return await _boardService.MoveBoardList(from, to, boardID);
        }
        private async Task<List<string>> SendMoveCardData(int from, int to, string listID)
        {
            return await _boardService.MoveBoardCard(from, to, listID);
        }
        private async void SendMoveCardToAnotherListData(int from, int to, string fromListID, string toListID)
        {
            await _boardService.MoveBoardCardBetweenLists(from, to, fromListID, toListID);
        }

        public ICommand ShowAddListButton => new AsyncCommand(async () =>
        {
            IsAddListTrigger = true;
        });
        public ICommand HideAddListButton => new AsyncCommand(async () =>
        {
            IsAddListTrigger = false;
        });
        public ICommand ShowMenu => new AsyncCommand(async () =>
        {
            IsMenuTrigger = true;
        });
        public ICommand HideMenu => new AsyncCommand(async () =>
        {
            IsMenuTrigger = false;
            IsShowMoreBoardDetails = false;
        });
        public ICommand ShowAddCardButton => new AsyncCommand(async () =>
        {
            SelectedList.IsAddCardTrigger = true;
        });
        public ICommand HideAddCardButton => new AsyncCommand(async () =>
        {
            SelectedList.IsAddCardTrigger = false;
        });
        public ICommand ShowInviteButton => new AsyncCommand(async () =>
        {
            IsInviteMemberTrigger = true;
        });
        public ICommand HideInviteButton => new AsyncCommand(async () =>
        {
            IsInviteMemberTrigger = false;
        });
        public ICommand ShowListAdditionOptions => new AsyncCommand(async () =>
        {
            SelectedList.IsShowListAdditionOptions = true;
        });
        public ICommand HideListAdditionOptions => new AsyncCommand(async () =>
        {
            SelectedList.IsShowListAdditionOptions = false;
        });
        public ICommand ShowMoreBoardDetailsCommand => new AsyncCommand(async () =>
        {
            IsShowMoreBoardDetails = true;
        });
        public ICommand HideMoreBoardDetailsCommand => new AsyncCommand(async () =>
        {
            IsShowMoreBoardDetails = false;
        });
        public ICommand StartEditingDescripntionCommand => new AsyncCommand(async () =>
        {
            DescriptionTextBox = Markdown.Normalize(SelectedCard.Description);
            IsEditingDescription = true;
        });
        public ICommand SaveEditingDescriptionCommand => new AsyncCommand(async () =>
        {
            _eventBusService.Publish(new WaitingResponseEvent());

            var newCard = await _boardService.SetDescription(SelectedCard.ID, DescriptionTextBox);
            CardDetailDescription = Markdown.ToHtml(newCard.Description);
            SelectedCard.SetDescription(newCard.Description);

            IsEditingDescription = false;
            CardDetailChangedBy = string.Empty;
            IsExistConflictChanges = false;

            _eventBusService.Publish(new ResponseReceivedEvent());
        });
        public ICommand CancelEditingDescriptionCommand => new AsyncCommand(async () =>
        {
            _eventBusService.Publish(new ResponseReceivedEvent());

            IsEditingDescription = false;
            CardDetailChangedBy = string.Empty;
            IsExistConflictChanges = false;
        });
        public ICommand ShowCardDetailsCommand => new AsyncCommand(async () =>
        {
            _eventBusService.Publish(new WaitingResponseEvent());

            CardDetailCurrentList = string.Copy(SelectedList.Title);
            IsEditingDescription = false;
            IsShowCardDetails = true;
            SelectedCard.SetDescription(await _boardService.GetDescription(SelectedCard.ID));
            CardDetailDescription = Markdown.ToHtml(SelectedCard.Description);

            _eventBusService.Publish(new ResponseReceivedEvent());
        });
        public ICommand HideCardDetailsCommand => new AsyncCommand(async () =>
        {
            if (IsEditingDescription)
            {
                if (!IsExistConflictChanges)
                {
                    _eventBusService.Publish(new WaitingResponseEvent());

                    var newCard = await _boardService.SetDescription(SelectedCard.ID, DescriptionTextBox);
                    CardDetailDescription = Markdown.ToHtml(newCard.Description);
                    SelectedCard.SetDescription(newCard.Description);

                    _eventBusService.Publish(new ResponseReceivedEvent());
                }
                IsEditingDescription = false;
            }

            _eventBusService.Publish(new ResponseReceivedEvent());

            IsShowCardDetails = false;
            CardDetailCurrentList = string.Empty;
            CardDetailDescription = string.Empty;
            DescriptionTextBox = string.Empty;
            CardDetailChangedBy = string.Empty;
            IsExistConflictChanges = false;


            SelectedCard = null;
        });
        public ICommand DeleteMemberCommand => new AsyncCommand(async () =>
        {
            try
            {
                if(_authenticationService.CurrentUser.ID != SelectedMember.ID &&
                    CurrentBoard.Owner.ID != SelectedMember.ID)
                {
                    _eventBusService.Publish(new WaitingResponseEvent());

                    await _boardService.KickOutMemberFromBoardById(CurrentBoard.ID, string.Copy(SelectedMember.ID));
                }
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand AddAnotherListCommand => new AsyncCommand(async () =>
        {
            try
            {
                _eventBusService.Publish(new WaitingResponseEvent());

                BoardList newList = await _boardService.CreateNewList(CurrentBoard.ID, NewListTitle);
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand AddAnotherCardCommand => new AsyncCommand(async () =>
        {
            try
            {
                _eventBusService.Publish(new WaitingResponseEvent());

                BoardCard newCard = await _boardService.CreateNewCard(SelectedList.ID, SelectedList.NewCardTitle);
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand DeleteBoardListCommand => new AsyncCommand(async () =>
        {
            try
            {
                _eventBusService.Publish(new WaitingResponseEvent());

                await _boardService.DeleteBoardList(SelectedList.ID);
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand DeleteBoardCardCommand => new AsyncCommand(async () =>
        {
            try
            {
                _eventBusService.Publish(new WaitingResponseEvent());

                await _boardService.DeleteBoardCard(SelectedCard.ID);
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand DeleteBoardCommand => new AsyncCommand(async () =>
        {
            try
            {
                _eventBusService.Publish(new WaitingResponseEvent());

                await _boardService.DeleteBoard(CurrentBoard.ID);
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        });
        public ICommand InviteMemberCommand => new AsyncCommand(async () =>
        {
            try
            {
                _eventBusService.Publish(new WaitingResponseEvent());

                User newMember = await _boardService.InviteNewMember(CurrentBoard.ID, InviteMemberName);
                _webSocketService.NotifyInvitedMember(newMember.ID);

                InviteMemberName = string.Empty;
            }
            catch (ServerResponseException ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = ex.Message;
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));
            }
            catch (Exception ex)
            {
                _eventBusService.Publish(new ResponseReceivedEvent());

                string alertMessage = "Something was wrong";
                string alertStatus = AlertStatus.ERROR;

                Alert alert = new Alert(alertMessage, alertStatus);
                _messageBusService.SendTo<MainViewModel>(new NotifyAlertMessage(alert));

                Debug.WriteLine(ex.Message);
            }
        });
    }
}
