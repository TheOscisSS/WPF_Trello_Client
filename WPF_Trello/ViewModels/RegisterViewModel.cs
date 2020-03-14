using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Trello.Pages;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public RegisterViewModel(PageService pageService)
        {
            _pageService = pageService;
        }

        public ICommand ChangePage => new AsyncCommand(async () =>
        {
            _pageService.ChangePage(new Login());
        });
    }
}
