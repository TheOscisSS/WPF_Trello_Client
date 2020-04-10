using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Services;

namespace WPF_Trello.ViewModels
{
    class AuthenticationViewModel
    {
        private readonly AuthenticationService _authenticationService;
        private readonly EventService _eventService;

        public AuthenticationViewModel(
            AuthenticationService authenticationService,
            EventService eventService)
        {
            _authenticationService = authenticationService;
            _eventService = eventService;
        }
    }
}
