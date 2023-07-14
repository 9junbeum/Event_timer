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
        string event_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Event_timer_settings";//저장 폴더
        string music_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Music_list_settings";//저장 폴더
        

        public bool is_SaveFile_Exist()
        {
            if (File.Exists(event_path))
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
        public bool is_SaveFile_Exist_()
        {
            if (File.Exists(music_path))
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
                    File.WriteAllText(event_path, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public void SAVE_(ObservableCollection<Music> m)
        {
            if (m != null)
            {
                try
                {
                    List<Music> MusicList = new List<Music>();
                    foreach (Music ml in m)
                    {
                        MusicList.Add(new Music(ml.FileName, ml.FilePath, ml.Duration, ml.Size));
                    }

                    string json = JsonConvert.SerializeObject(MusicList, Formatting.Indented);
                    File.WriteAllText(music_path, json);
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
                string json_e = File.ReadAllText(event_path);

                JArray jArray_e = JArray.Parse(json_e);

                foreach (JObject jObject in jArray_e)
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

        public ObservableCollection<Music> LOAD_()
        {
            ObservableCollection<Music> musics = new ObservableCollection<Music>();

            try
            {
                string json_m = File.ReadAllText(music_path);

                JArray jArray_m = JArray.Parse(json_m);

                foreach (JObject jObject in jArray_m)
                {
                    string filename = (string)jObject["FileName"];
                    string filepath = (string)jObject["FilePath"];
                    TimeSpan duration = jObject["Duration"].ToObject<TimeSpan>();
                    long size = jObject["Size"].ToObject<long>();

                    Music m = new Music(filename, filepath, duration, size);
                    musics.Add(m);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return musics;
        }
    }
}
