using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_timer
{
    public class Music
    {
        public bool Is_Playing {get; set;}
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public TimeSpan Duration { get; set; }
        public long Size { get; set; }

        public Music() { }
        public Music(string fileName, string filePath, TimeSpan duration, long size)
        {
            Is_Playing = false;
            FileName = fileName;
            FilePath = filePath;
            Duration = duration;
            Size = size;
        }
    }
}
