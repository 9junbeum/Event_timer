using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Event_timer
{
    internal class Save
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Event_timer_settings";//저장 폴더
        JObject jobj = new JObject();//Json 객체


        List<string> name__ = new List<string>();
        List<bool[]> dotw__ = new List<bool[]>();
        List<Time_> stime__ = new List<Time_>();
        List<Time_> etime__ = new List<Time_>();
        List<bool[]> event_num__ = new List<bool[]>();



        public bool is_SaveFile_Exist()
        {
            if (File.Exists(path))
            {
                //파일이 있으면
                return true;
            }
            else
            {
                //없으면
                return false;
            }
        }
        public void SAVE(ObservableCollection<Event> e)
        {
            if (e != null)
            {
                try
                {
                    List<Event> eventDataList = new List<Event>();
                    foreach (Event ev in e)
                    {
                        eventDataList.Add(new Event(ev.Name, ev.DOTW, ev.S_time, ev.E_time, ev.Event_num));
                    }

                    string json = JsonConvert.SerializeObject(eventDataList, Formatting.Indented);
                    File.WriteAllText(path, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public ObservableCollection<Event> LOAD()
        {
            ObservableCollection<Event> events = new ObservableCollection<Event>();

            try
            {
                string json = File.ReadAllText(path);
                JArray jArray = JArray.Parse(json);

                foreach (JObject jObject in jArray)
                {
                    string name = (string)jObject["Name"];
                    bool[] dotw = jObject["DOTW"].ToObject<bool[]>();
                    Time_ stime = jObject["S_time"].ToObject<Time_>();
                    Time_ etime = jObject["E_time"].ToObject<Time_>();
                    bool[] eventNum = jObject["Event_num"].ToObject<bool[]>();

                    Event e = new Event(name, dotw, stime, etime, eventNum);
                    events.Add(e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return events;
        }
    }
}
