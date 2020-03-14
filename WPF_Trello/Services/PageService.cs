using System;
using System.Windows.Controls;
using WPF_Trello.Pages;

namespace WPF_Trello.Services
{
    public class PageService
    {

        public event Action<Page> OnPageChanged;
        public void ChangePage(Page page) => OnPageChanged?.Invoke(page);
    }
}
