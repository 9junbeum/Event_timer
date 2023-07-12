using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Event_timer
{
    /// <summary>
    /// Time_Picker.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Time_Picker : UserControl
    {
        private Time_ time = new Time_();

        public Time_Picker()
        {
            InitializeComponent();
            Init_Combobox();
        }
        private void Init_Combobox()
        {
            try
            {
                for (int i = 1; i <= 12; i++) 
                {
                    Hour.Items.Add(i);
                }
                for (int i = 0; i < 60; i++)
                {
                    Minute.Items.Add(i);
                }
                for (int i = 0; i < 60; i++)
                {
                    Second.Items.Add(i);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //오전 오후 checkbox 이벤트
            CheckBox checkBox = (CheckBox)sender;

            if (checkBox == AM)
            {
                time.AM_PM = 'a';
                PM.IsChecked = false;
            }
            else if (checkBox == PM)
            {
                time.AM_PM = 'p';
                AM.IsChecked = false;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if(PM.IsChecked == true || AM.IsChecked == true)
            {
                //정상
            }
            else
            {
                time.AM_PM = 'n';
            }
        }

        public bool Is_time_exist()
        {
            //TimePicker에 시간 값이 정상적으로 존재하는지?
            
            if(Hour.SelectedIndex != -1)
            {
                if (Minute.SelectedIndex != -1)
                {
                    if (Second.SelectedIndex != -1)
                    {
                        if (AM.IsChecked != false || PM.IsChecked != false)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void Hour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //시 변경
            if(Hour.SelectedIndex != -1)
            {
                time.Hour = int.Parse(Hour.SelectedItem.ToString());
            }
        }

        private void Minute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //분 변경
            if (Minute.SelectedIndex != -1)
            {
                time.Minute = int.Parse(Minute.SelectedItem.ToString());
            }
        }

        private void Second_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //초 변경
            if (Second.SelectedIndex != -1)
            {
                time.Second = int.Parse(Second.SelectedItem.ToString());
            }
        }
        public Time_ Get_Time()
        {
            return new Time_(time.Hour, time.Minute, time.Second, time.AM_PM);
        }
        public void Clear()
        {
            AM.IsChecked = false;
            PM.IsChecked = false;
            Hour.SelectedIndex = -1;
            Minute.SelectedIndex = -1;
            Second.SelectedIndex = -1;
        }

    }
}
