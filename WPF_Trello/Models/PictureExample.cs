namespace WPF_Trello.Models
{
    public class PictureExample
    {
        public PictureExample(string regular, string small, string thumb)
        {
            Regular = regular;
            Small = small;
            Thumb = thumb;
        }

        public string Regular { get; private set; }
       public string Small { get; private set; }
       public string Thumb { get; private set; }
    }
}
