using Caliburn.Micro;

namespace VkTunes.AudioList
{
    public class AudioRecordViewModel : PropertyChangedBase
    {
        private string title;

        public int Id { get; set; }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public string Duration { get; set; }
    }
}