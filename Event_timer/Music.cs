using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_timer
{
    public class Music :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool is_playing;
        public bool Is_Playing
        {
            get { return is_playing; }
            set
            {
                if (is_playing != value)
                {
                    is_playing = value;
                    OnPropertyChanged(nameof(Is_Playing));
                }
            }
        }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public TimeSpan Duration { get; set; }
        public long Size { get; set; }

        public Music() { }
        public Music(bool is_playing, string fileName, string filePath, TimeSpan duration, long size)
        {
            Is_Playing = is_playing;
            FileName = fileName;
            FilePath = filePath;
            Duration = duration;
            Size = size;
        }
    }
}
