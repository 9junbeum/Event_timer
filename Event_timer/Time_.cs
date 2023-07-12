using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_timer
{
    public class Time_
    {
        public int Hour;
        public int Minute;
        public int Second;
        public char AM_PM; //A and P 

        public Time_()
        {
            this.Hour = -1;
            this.Minute = -1;
            this.Second = -1;
            this.AM_PM= 'n';
        }
        public Time_(int hour, int minute, int second, char aM_PM)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
            AM_PM = aM_PM;
        }//생성자

        public bool equals(Time_ other)
        {
            if(this.Hour == other.Hour)
            {

                if (this.Minute == other.Minute)
                {

                    if (this.Second == other.Second)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool equals_now(DateTime now)
        {
            if (this.Hour == int.Parse(now.ToString("hh")))//현재랑 시간이 같고,
            {
                if (this.Minute == now.Minute)//분 도 같고
                {
                    if (this.Second == now.Second)//초 도 같고
                    {
                        if(this.AM_PM =='a')
                        {
                            //오전일 때
                            if (now.ToString("tt").Equals("오전"))
                            {
                                return true;                                
                            }
                        }
                        else if(this.AM_PM == 'p')
                        {
                            //오후일 때
                            if (now.ToString("tt").Equals("오후"))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            //오전/오후 정의가 안되어있음.
                            return false;
                        }
                    }
                }
            }
            return false;
        }

    }
}
