namespace WPF_Trello.Models
{
    class AnonymousIdentity : CustomIdentity
    {
        public AnonymousIdentity() : base(string.Empty, new string[] { }) { }
    }
}
