using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF_Trello.Messages;
using WPF_Trello.Models;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class BoardViewModel : BindableBase
    {
        private readonly PageService _pageService;
        private readonly AuthenticationService _authenticationService;
        private readonly EventBusService _eventBusService;
        private readonly BoardService _boardService;
        private readonly MessageBusService _messageBusService;

        public Board CurrentBoard { get; private set; }


        public BoardViewModel(PageService pageService, AuthenticationService authenticationService, EventBusService eventBusService,
                BoardService boardService, MessageBusService messageBusService)
        {
            _pageService = pageService;
            _authenticationService = authenticationService;
            _eventBusService = eventBusService;
            _boardService = boardService;
            _messageBusService = messageBusService;

            _messageBusService.Receive<BoardPreloadMessage>(this, async message =>
            {
                CurrentBoard = new Board(message.ID, message.Title, message.Description, message.Background, message.CreatedAt, message.UpdatedAt);
            });


            //BoardLists = new ObservableCollection<BoardList>
            //{
            //    new BoardList("Hello", new List<BoardCard>
            //    {
            //        new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //          new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //        new BoardCard("JS", "...", "Practics"),
            //    }),
            //    new BoardList("Second list",  new List<BoardCard>
            //    {
            //        new BoardCard("Second", "...", "Theory"),
            //        new BoardCard("HTTP", "...", "Practics"),
            //    }),
            //    new BoardList("Hello", new List<BoardCard>
            //    {
            //        new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //         new BoardCard("React", "...", "Theory"),
            //        new BoardCard("JS", "...", "Practics"),
            //        new BoardCard("JS", "...", "Practics"),
            //    }),
            //    new BoardList("Second list",  new List<BoardCard>
            //    {
            //    }),
            //};
        }
    }
}
