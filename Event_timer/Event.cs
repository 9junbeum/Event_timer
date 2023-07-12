using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Event_timer
{
    internal class Event
    {
        public string Name { get; set; }//이벤트 명
        public bool[] DOTW { get; set; }//요일 day of the week
        public Time_ S_time { get; set; }//시작시간 
        public Time_ E_time { get; set; }//종료시간 
        public bool[] Event_num { get; set; }//이벤트 종류

        public Event(string name, bool[] dotw, Time_ s_time, Time_ e_time, bool[] event_num)
        {
            this.Name = name;
            this.DOTW = dotw;
            this.S_time = s_time;
            this.E_time = e_time;
            this.Event_num = event_num;
        }
    
    }
}
