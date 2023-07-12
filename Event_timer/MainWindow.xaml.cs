﻿using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Event_timer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serial_port = new SerialPort();//시리얼 포트 정보
        static ObservableCollection<Event> Events = new ObservableCollection<Event>();//이벤트 저장
        Save settings = new Save(); //설정 저장
        Thread check_serial; //시리얼 연결 체크용 Thread
        bool exitFlag = false;//프로그램 종료 시 Thread 중단용도
        //시계
        private System.Timers.Timer timer;
        private Stopwatch stopwatch;
        private TimeSpan interval = TimeSpan.FromSeconds(1);
        //UDP 통신
        private UDP udp;
        private static string udp_ip = "192.168.21.255";

        public MainWindow()
        {
            InitializeComponent();
            Initialize_Serial_Port();
            load_settings();
            StartTimer();
            Event_ListView.ItemsSource = Events;
            udp = new UDP();
            udp.data_recieved += new UDP.Data_Recieved(udp_Data_Received);
        }

        private void StartTimer()
        {
            timer = new System.Timers.Timer(interval.TotalMilliseconds);
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
            timer.Start();

            stopwatch = Stopwatch.StartNew();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan elapsed = stopwatch.Elapsed;
            stopwatch.Restart();
            DateTime now = DateTime.Now;
            Current_Time.Dispatcher.Invoke(() =>
            {
                Current_Time.Text = now.ToString();
            });

            //1~2ms 의 오차 발생 확인 코드
            //Dispatcher.Invoke(() =>
            //{
            //    status_box.AppendText(now.Millisecond.ToString() + "\n");
            //});

            while (elapsed >= interval)
            {
                elapsed -= interval;
                Task.Run(() => Check_event());
            }
        }

        private void Check_event()
        {
            //매 초 발생
            DateTime now = DateTime.Now;
            int day_of_week = (int)now.DayOfWeek; //오늘의 요일을 숫자로 변경

            foreach(Event e in Events) //이벤트
            {
                if (e.DOTW[day_of_week] == true)//오늘이 이벤트 요일 이라면,
                {
                    if(e.S_time.equals_now(now))//시작시간이 현재랑 같은지 확인
                    {
                        Dispatcher.Invoke(() =>
                        {
                            status_box.AppendText("이벤트 발생\n");
                            
                            string data = "";

                            for (int i = 0; i < e.Event_num.Length; i++)
                            {
                                if (e.Event_num[i] == true)
                                {
                                    data += (i + 1).ToString() + "1";
                                }
                            }
                            Task.Run(() =>
                            {
                                serial_port.WriteLine(data);
                            });
                        });
                    }
                    else if(e.E_time.equals_now(now))//종료시간이 현재랑 같은지 확인
                    {
                        Dispatcher.Invoke(() =>
                        {
                            status_box.AppendText("이벤트 종료\n");
                            
                            string data = "";

                            for (int i = 0; i < e.Event_num.Length; i++)
                            {
                                if (e.Event_num[i] == true)
                                {
                                    data += (i + 1).ToString() + "0";
                                }
                            }

                            Task.Run(() =>
                            {
                                serial_port.WriteLine(data);
                            });
                        });
                    }
                }
            }
        }

        private void Initialize_Serial_Port()
        {
            //시리얼 포트 설정
            string[] ports = SerialPort.GetPortNames();
            Serial_combo.ItemsSource = ports; //연결된 port 이름을 저장.
        }

        private void Close_Btn_Click(object sender, RoutedEventArgs e)
        {
            //시스템 종료 버튼
            exitFlag = false;
            udp.Kill_Thread_UDP();
            Thread.Sleep(200);
            this.Close();
        }
        
        private void Serial_connect_Btn_Click(object sender, RoutedEventArgs e)
        {
            //상단의 시리얼통신 연결 버튼 클릭 시

            if (!serial_port.IsOpen)//포트가 닫혀있으면,
            {
                try
                {
                    if (Serial_combo.SelectedIndex == -1) //시리얼이 선택되지 않았을 때.
                    {
                        System.Windows.MessageBox.Show("시리얼 포트를 선택해주세요");
                        return;
                    }
                    else
                    {
                        //시리얼 통신 기본 설정.
                        serial_port.PortName = Serial_combo.Text.ToString();
                        serial_port.BaudRate = 9600;
                        serial_port.DataBits = 8;
                        serial_port.StopBits = StopBits.One;
                        serial_port.Parity = Parity.None;
                        serial_port.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                        check_serial = new Thread(new ThreadStart(serial_is_open));
                        exitFlag = true;
                        check_serial.Start();
                        //포트 연결
                        serial_port.Open();
                        //연결 성공 시
                        if (serial_port.IsOpen)
                        {
                            status_box.AppendText("시리얼 포트에 성공적으로 연결되었습니다.\n");

                            Serial_combo.IsEnabled = false;
                            Serial_connect_Btn.IsEnabled = false;
                            Serial_connect_Btn.Content = "연결됨";
                            ListView_Grid.IsEnabled = true;
                            Manual_Relay_Grid.IsEnabled = true;
                            Event_Grid.IsEnabled = true;
                        }
                        else
                        {
                            //연결 실패
                            status_box.AppendText("시리얼 포트에 정상적으로 연결되지 않았습니다.\n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    status_box.AppendText(ex.ToString() + "\n");
                }
            }
        }

        private void serial_is_open()
        {
            Thread.Sleep(2000); //연결 후 2초 뒤 부터 검사

            while (exitFlag)
            {
                if (!serial_port.IsOpen)
                {
                    //연결 안됨
                    MessageBox.Show("시리얼 포트 연결이 끊겼습니다. 다시 연결해주세요.");
                    Dispatcher.Invoke(() =>
                    {
                        Serial_combo.IsEnabled = true;
                        Serial_connect_Btn.IsEnabled = true;
                        Serial_connect_Btn.Content = "연결";

                        ListView_Grid.IsEnabled = false;
                        Manual_Relay_Grid.IsEnabled = false;
                        Event_Grid.IsEnabled = false;
                    });
                    return;
                }
                Thread.Sleep(100);
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //시리얼통신 데이터 도착 시 발생.
            string receivedData = serial_port.ReadExisting();
            Dispatcher.Invoke(() =>
            {
                status_box.AppendText(receivedData + "\n");//UI 스레드에서 UI 요소에 접근.
            });
        }

        private void Add_event_Btn_Click(object sender, RoutedEventArgs e)
        {
            //새로운 이벤트 등록
            Event new_evt;
            string name = "";
            bool[] arr_dotw = new bool[7];
            Time_ s_time = new Time_();
            Time_ e_time = new Time_();
            bool[] arr_event = new bool[8];

            try
            {
                if(Event_Name.Text != "")
                {
                    name = Event_Name.Text;
                }
                else
                {
                    MessageBox.Show("이벤트 이름이 제대로 설정되지 않았습니다.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("이벤트 이름 설정 중 오류가 발생하였습니다.\n" + ex);
                return;
            }//name
            try
            {
                CheckBox[] checkbox = new CheckBox[7];
                checkbox[0] = SUN;
                checkbox[1] = MON;
                checkbox[2] = TUE;
                checkbox[3] = WED;
                checkbox[4] = THU;
                checkbox[5] = FRI;
                checkbox[6] = SAT;
                for (int i = 0; i < checkbox.Length; i++)
                {
                    arr_dotw[i] = checkbox[i].IsChecked == true ? true : false;
                }

                int count = 0;
                for (int i=0; i< arr_dotw.Length;i++)
                {
                    if (arr_dotw[i] == true)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    //모든 요일이 미 선택
                    MessageBox.Show("요일이 선택되지 않아 이벤트를 등록할 수 없습니다.");
                    return;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("이벤트 요일 설정 중 오류가 발생하였습니다.\n" + ex);
                return;
            }//dotw
            try
            {
                if(s_time_picker.Is_time_exist())
                {
                    if (e_time_picker.Is_time_exist())
                    {
                        if(IsEarlierThan(s_time_picker.Get_Time(), e_time_picker.Get_Time()))
                        {
                            s_time = s_time_picker.Get_Time();
                            e_time = e_time_picker.Get_Time();
                        }
                        else
                        {
                            //시간 형식 오류
                            MessageBox.Show("설정한 시간 순서에 오류가 있습니다. \n왼쪽 시간이 오른쪽 시간보다 느리거나 같습니다.");
                            return;
                        }
                    }
                    else
                    {
                        //시간 미 입력
                        MessageBox.Show("시간을 설정하지 않아 이벤트를 등록할 수 없습니다.");
                        return;
                    }
                }
                else
                {
                    //시간 미 입력
                    MessageBox.Show("시간을 설정하지 않아 이벤트를 등록할 수 없습니다.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("이벤트 발생(종료) 시간 설정 중 오류가 발생하였습니다.");
                return;
            }//s_time, e_time
            try
            {
                CheckBox[] checkbox_ = new CheckBox[8];
                checkbox_[0] = event_1;
                checkbox_[1] = event_2;
                checkbox_[2] = event_3;
                checkbox_[3] = event_4;
                checkbox_[4] = event_5;
                checkbox_[5] = event_6;
                checkbox_[6] = event_7;
                checkbox_[7] = event_8;
                for (int i = 0; i < checkbox_.Length; i++)
                {
                    arr_event[i] = checkbox_[i].IsChecked == true ? true : false;
                }

                int count = 0;
                for (int i = 0; i < arr_event.Length; i++)
                {
                    if (arr_event[i] == true)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    //모든 이벤트 미 선택
                    MessageBox.Show("발생 이벤트(릴레이)가 선택되지 않아 이벤트를 등록할 수 없습니다.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("이벤트 릴레이 설정 중 오류가 발생하였습니다.");
                return;
            }//event

            new_evt = new Event(name, arr_dotw, s_time, e_time, arr_event);//이벤트를 만들어
            Events.Add(new_evt);//담기
            Clear_event_set();//선택된 모든 이벤트 내용 Clear


        }

        private bool IsEarlierThan(Time_ t1, Time_ t2)
        {
            //t1이 t2보다 빠른 시간인지 확인
            if (t1.AM_PM == 'a' && t2.AM_PM == 'p')
            {
                return true; // 현재 시간이 오전이고 비교 대상이 오후인 경우
            }
            else if (t1.AM_PM == 'p' && t2.AM_PM == 'a')
            {
                return false; // 현재 시간이 오후이고 비교 대상이 오전인 경우
            }
            else if (t1.AM_PM == t2.AM_PM) // AM_PM이 동일한 경우
            {
                if (t1.Hour < t2.Hour)
                {
                    return true;
                }
                else if (t1.Hour == t2.Hour)
                {
                    if (t1.Minute < t2.Minute)
                    {
                        return true;
                    }
                    else if (t1.Minute == t2.Minute)
                    {
                        if (t1.Second < t2.Second)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void Clear_event_set()
        {
            //선택된 모든 이벤트 내용 Clear
            Event_Name.Clear();
            check_ED.IsChecked = false;
            check_EW.IsChecked = false;
            check_O.IsChecked = false;
            MON.IsChecked = false;
            TUE.IsChecked = false;
            WED.IsChecked = false;
            THU.IsChecked = false;
            FRI.IsChecked = false;
            SAT.IsChecked = false;
            SUN.IsChecked = false;
            s_time_picker.Clear();
            e_time_picker.Clear();
            event_1.IsChecked = false;
            event_2.IsChecked = false;
            event_3.IsChecked = false;
            event_4.IsChecked = false;
            event_5.IsChecked = false;
            event_6.IsChecked = false;
            event_7.IsChecked = false;
            event_8.IsChecked = false;
        }

        private void save_settings(object sender, EventArgs e)
        {
            //시스템 종료 시 현재 설정 저장.
            settings.SAVE(Events);
        }

        private void load_settings()
        {
            //시스템 시작 시 저장된 설정값 불러오기.
            Events = settings.LOAD();
        }

        private void EveryDay(object sender, RoutedEventArgs e)
        {
            //매일

            //나머지 해제
            check_EW.IsChecked = false;
            check_O.IsChecked = false;

            //월화수목금토일 모두 체크
            MON.IsChecked = TUE.IsChecked = WED.IsChecked = THU.IsChecked = FRI.IsChecked = SAT.IsChecked = SUN.IsChecked = true;

        }

        private void EveryWeek(object sender, RoutedEventArgs e)
        {
            //매주

            //나머지 해제
            check_ED.IsChecked = false;
            check_O.IsChecked = false;

            //일단 월화수목금토일 모두 해제
            MON.IsChecked = TUE.IsChecked = WED.IsChecked = THU.IsChecked = FRI.IsChecked = SAT.IsChecked = SUN.IsChecked = false;
        }

        private void Once(object sender, RoutedEventArgs e)
        {
            //한번만

            //나머지 해제
            check_ED.IsChecked = false;
            check_EW.IsChecked = false;

            //일단 월화수목금토일 모두 해제
            MON.IsChecked = TUE.IsChecked = WED.IsChecked = THU.IsChecked = FRI.IsChecked = SAT.IsChecked = SUN.IsChecked = false;
        }

        private void event_delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("정말로 이벤트를 삭제하시겠습니까?", "삭제", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
            {
                Button b = (Button)sender;
                Event item = (Event)b.DataContext;

                Events.Remove(item);
            }
            else if(result == MessageBoxResult.Cancel)
            {
                return;
            }
        }

        private async void manual_relay(object sender, RoutedEventArgs e)
        {
            //시리얼 통신 테스트용
            //추후 삭제
            ToggleButton b = sender as ToggleButton;
            string tag = b.Tag.ToString();

            if(b.IsChecked == true)
            {
                await Task.Run(() =>
                {
                    tag += "1";
                    serial_port.WriteLine(tag);
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    tag += "0";
                    serial_port.WriteLine(tag);
                });
            }
        }

        private void event_Checked(object sender, RoutedEventArgs e)
        {
            //이벤트가 체크 될 때
            //최대 2개만 선택가능하도록 알림
            int checked_event = 0;
            if (event_1.IsChecked == true)
            {
                checked_event += 1;
            }
            if (event_2.IsChecked == true)
            {
                checked_event += 1;
            }
            if (event_3.IsChecked == true)
            {
                checked_event += 1;
            }
            if (event_4.IsChecked == true)
            {
                checked_event += 1;
            }
            if (event_5.IsChecked == true)
            {
                checked_event += 1;
            }
            if (event_6.IsChecked == true)
            {
                checked_event += 1;
            }
            if (event_7.IsChecked == true)
            {
                checked_event += 1;
            }
            if (event_8.IsChecked == true)
            {
                checked_event += 1;
            }

            if (checked_event > 2)
            {
                MessageBox.Show("더 이상 이벤트를 추가할 수 없습니다.");

                CheckBox cb = sender as CheckBox;
                cb.IsChecked = false;
            }
        }

        private void status_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            status_box.ScrollToEnd();//자동 스크롤
        }

        private void UDP_Connect_Click(object sender, RoutedEventArgs e)
        {
            udp.SendData("하이하이", "192.168.21.11", 8000);
        }
        private void udp_Data_Received(string data)
        {
            //udp 데이터 수신 시
            MessageBox.Show($"UDP data received: {data}");
        }
    }
}