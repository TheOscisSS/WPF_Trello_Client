using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class ListOfBoardsViewModel
    {
        private readonly PageService _pageService;

        public ListOfBoardsViewModel(PageService pageService)
        {
            _pageService = pageService;
        }
    }
}
