using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace EduBanking.Silverlight.ViewModels.Common
{
    public class CalendarEditViewModel : ViewModelBase
    {

        public CalendarEditViewModel()
        {
            this.currentYear = DateTime.Now.Date.Year;
            this.rootYear = this.currentYear;
            this.currentMonth = DateTime.Now.Date.Month;
            this.dicStringHolidayCurrentYear = new Dictionary<int, string>();
            this.dicStringHolidayNextYear = new Dictionary<int, string>();
            this.dicStringHoliday = this.dicStringHolidayCurrentYear;
            for (int i = 1; i <= 12; i++)
                this.dicStringHoliday.Add(i, "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW");
            this.changeMonth = new ChangeMonthCommand(this);
            BindingDayOfMonth();
        }

        public CalendarEditViewModel(int year, int month, Dictionary<int, string> special, Dictionary<int, string> specialNext)
        {
            this.currentYear = year;
            this.rootYear = year;
            this.currentMonth = month;
            
            this.dicStringHolidayCurrentYear = special;
            this.dicStringHolidayNextYear = specialNext;

            this.dicStringHoliday = this.dicStringHolidayCurrentYear;
            this.changeMonth = new ChangeMonthCommand(this);
            BindingDayOfMonth();            
        }

        #region[Properties]

        private Dictionary<int, string> dicStringHoliday;
        private Dictionary<int, string> dicStringHolidayNextYear;
        private Dictionary<int, string> dicStringHolidayCurrentYear;
        private int currentYear;
        private int currentMonth;
        private const int row = 6;
        private const int col = 7;
        private TextBlockData[,] allData;
        private int rootYear;
    
        public string HeaderMonth
        {
            get
            {
                return this.currentMonth + " - " + this.currentYear;
            }
        }

        private ChangeMonthCommand changeMonth;
        public ChangeMonthCommand ChangeMonth
        {
            get
            {
                return this.changeMonth;
            }            
        }

        private DayOfWeekEditCulture dayWeek = new DayOfWeekEditCulture(CultureInfo.CurrentCulture.Name);
        public DayOfWeekEditCulture DayCulture
        {
            get
            {
                return this.dayWeek;
            }
        }

        private string isEnable = "False";
        public string IsEnabled
        {
            get 
            {
                return this.isEnable;
            }
            set
            {
                this.isEnable = value;
                this.OnPropertyChanged("IsEnabled");
            }
        }

        #endregion

        #region [All Items in Calendar]

        public TextBlockData txtData00
        {
            get
            {
                return this.allData[0, 0];
            }

        }
        public TextBlockData txtData01
        {
            get
            {
                return this.allData[0, 1];
            }
        }
        public TextBlockData txtData02
        {
            get
            {
                return this.allData[0, 2];
            }
        }
        public TextBlockData txtData03
        {
            get
            {
                return this.allData[0, 3];
            }
        }
        public TextBlockData txtData04
        {
            get
            {
                return this.allData[0, 4];
            }
        }
        public TextBlockData txtData05
        {
            get
            {
                return this.allData[0, 5];
            }
        }
        public TextBlockData txtData06
        {
            get
            {
                return this.allData[0, 6];
            }
        }

        public TextBlockData txtData10
        {
            get
            {
                return this.allData[1, 0];
            }
        }
        public TextBlockData txtData11
        {
            get
            {
                return this.allData[1, 1];
            }
        }
        public TextBlockData txtData12
        {
            get
            {
                return this.allData[1, 2];
            }
        }
        public TextBlockData txtData13
        {
            get
            {
                return this.allData[1, 3];
            }
        }
        public TextBlockData txtData14
        {
            get
            {
                return this.allData[1, 4];
            }
        }
        public TextBlockData txtData15
        {
            get
            {
                return this.allData[1, 5];
            }
        }
        public TextBlockData txtData16
        {
            get
            {
                return this.allData[1, 6];
            }
        }

        public TextBlockData txtData20
        {
            get
            {
                return this.allData[2, 0];
            }
        }
        public TextBlockData txtData21
        {
            get
            {
                return this.allData[2, 1];
            }
        }
        public TextBlockData txtData22
        {
            get
            {
                return this.allData[2, 2];
            }
        }
        public TextBlockData txtData23
        {
            get
            {
                return this.allData[2, 3];
            }
        }
        public TextBlockData txtData24
        {
            get
            {
                return this.allData[2, 4];
            }
        }
        public TextBlockData txtData25
        {
            get
            {
                return this.allData[2, 5];
            }
        }
        public TextBlockData txtData26
        {
            get
            {
                return this.allData[2, 6];
            }
        }

        public TextBlockData txtData30
        {
            get
            {
                return this.allData[3, 0];
            }
        }
        public TextBlockData txtData31
        {
            get
            {
                return this.allData[3, 1];
            }
        }
        public TextBlockData txtData32
        {
            get
            {
                return this.allData[3, 2];
            }
        }
        public TextBlockData txtData33
        {
            get
            {
                return this.allData[3, 3];
            }
        }
        public TextBlockData txtData34
        {
            get
            {
                return this.allData[3, 4];
            }
        }
        public TextBlockData txtData35
        {
            get
            {
                return this.allData[3, 5];
            }
        }
        public TextBlockData txtData36
        {
            get
            {
                return this.allData[3, 6];
            }
        }

        public TextBlockData txtData40
        {
            get
            {
                return this.allData[4, 0];
            }
        }
        public TextBlockData txtData41
        {
            get
            {
                return this.allData[4, 1];
            }
        }
        public TextBlockData txtData42
        {
            get
            {
                return this.allData[4, 2];
            }
        }
        public TextBlockData txtData43
        {
            get
            {
                return this.allData[4, 3];
            }
        }
        public TextBlockData txtData44
        {
            get
            {
                return this.allData[4, 4];
            }
        }
        public TextBlockData txtData45
        {
            get
            {
                return this.allData[4, 5];
            }
        }
        public TextBlockData txtData46
        {
            get
            {
                return this.allData[4, 6];
            }
        }

        public TextBlockData txtData50
        {
            get
            {
                return this.allData[5, 0];
            }
        }
        public TextBlockData txtData51
        {
            get
            {
                return this.allData[5, 1];
            }
        }
        public TextBlockData txtData52
        {
            get
            {
                return this.allData[5, 2];
            }
        }
        public TextBlockData txtData53
        {
            get
            {
                return this.allData[5, 3];
            }
        }
        public TextBlockData txtData54
        {
            get
            {
                return this.allData[5, 4];
            }
        }
        public TextBlockData txtData55
        {
            get
            {
                return this.allData[5, 5];
            }
        }
        public TextBlockData txtData56
        {
            get
            {
                return this.allData[5, 6];
            }
        }

        #endregion

        #region[Functions]

        public void BindingDayOfMonth()
        {
            //reset data
            TextBlockData[,] items =  new TextBlockData[row, col];
            Holiday.Clear();           

            int dayInMonth = DateTime.DaysInMonth(this.currentYear, this.currentMonth);
            int beginDay = GetBeginDay();
            int countDay = 1;
            bool isHoliday = false;
            string color = Colors.Black.ToString();
            string currentStringHoliday = dicStringHoliday[this.currentMonth];
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    if ((i == 0 && j < beginDay) || countDay > dayInMonth)
                        items[i, j] = new TextBlockData();
                    else
                    {
                        isHoliday = currentStringHoliday[countDay - 1].ToString().Equals("H");
                        if (isHoliday)
                        {
                            Holiday.Add(countDay.ToString());
                            color = Colors.Red.ToString();
                        }
                        else
                            color = Colors.Black.ToString();
                        items[i, j] = new TextBlockData(countDay.ToString(), color);
                        countDay++;
                    }
                }
            this.allData = items;
            items = null;
            this.OnPropertyChanged("");
        }

        private void SetDefaultHoliday()
        {
            for (int month = 1; month <= 12; month++)
            {
                //reset data            
                Holiday.Clear();

                int dayInMonth = DateTime.DaysInMonth(this.currentYear, month);
                DateTime day;
                string strHoliday = "";
                for (int i = 1; i <= dayInMonth; i++)
                {
                    day = new DateTime(this.currentYear, month, i);
                    
                    if (day.DayOfWeek == DayOfWeek.Sunday)
                    {
                        Holiday.Add(i.ToString());
                        strHoliday += "H";
                    }
                    else
                        strHoliday += "W";
                }

                this.dicStringHoliday[month] = strHoliday;
            }
            this.BindingDayOfMonth();
        }

        private int GetBeginDay()
        {
            DateTime date = new DateTime(this.currentYear, this.currentMonth, 1);
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday: return 0;
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Friday: return 5;
                case DayOfWeek.Saturday: return 6;
            }
            return 0;
        }
       

        private string getCurrentStringHoliday(int year, int month)
        {
            int dayInMonth = DateTime.DaysInMonth(year, month);
            string str = "";
            for (int i = 1; i <= dayInMonth; i++)
            {
                if (ListHoliday.Contains(i.ToString()))
                    str += "H";
                else
                    str += "W";
            }
            return str;
        }

        internal void ChangeMonthEvent(string p)
        {
            if (p.Equals("Next") )
            {
                if (this.currentMonth < 12)
                {
                    this.dicStringHoliday[this.currentMonth] = this.getCurrentStringHoliday(this.currentYear, this.currentMonth);
                    this.currentMonth++;
                    this.BindingDayOfMonth();
                }
                else if (this.currentYear < this.rootYear + 1)
                {
                    this.dicStringHoliday[this.currentMonth] = this.getCurrentStringHoliday(this.currentYear, this.currentMonth);
                    this.dicStringHoliday = this.dicStringHolidayNextYear;
                    this.currentMonth = 1;
                    this.currentYear++;
                    this.BindingDayOfMonth();
                }
            }
            else
            if (p.Equals("Pre") )
            {
                if (this.currentMonth > 1)
                {
                    this.dicStringHoliday[this.currentMonth] = this.getCurrentStringHoliday(this.currentYear, this.currentMonth);
                    this.currentMonth--;
                    this.BindingDayOfMonth();
                }
                else if (this.currentYear > this.rootYear)
                {
                    this.dicStringHoliday[this.currentMonth] = this.getCurrentStringHoliday(this.currentYear, this.currentMonth);
                    this.dicStringHoliday = this.dicStringHolidayCurrentYear;
                    this.currentMonth = 12;
                    this.currentYear--;
                    this.BindingDayOfMonth();
                }
            }                     
        }

        //set default holliday
        public void SetDefault()
        {
            this.SetDefaultHoliday();
        }

        //return list holiday
        public List<string> Holiday
        {
            get
            {
                return ListHoliday;
            }
        }

        //return current year
        public int Year
        {
            get
            {
                return this.currentYear;
            }
        }

        //return current month
        public int Month
        {
            get
            {
                return this.currentMonth;
            }
        }

        //return string holiday
        public string CurrentStringHoliday
        {
            get
            {
                return this.getCurrentStringHoliday(this.currentYear, this.currentMonth);
            }
            
        }

        //return all string holiday in year
        public Dictionary<int, string> GetAllStringHoliday()
        {
            //get
            //{
                return this.dicStringHoliday;
            //}
        }

        //
        public bool IsNextYear
        {
            get
            {
                return this.currentYear != this.rootYear;
            }
        }

        #endregion

        #region [Class]

        public class TextBlockData : ViewModelBase
        {
            public TextBlockData()
            {
                this.isVisible = Visibility.Collapsed;
                this.Text = "";
                this.colorText = Colors.Black.ToString();
            }
            public TextBlockData(string day, string color)
            {
                this.icommand = new TextBlockClick(this);
                this.isVisible = Visibility.Visible;
                this.Text = day;
                this.colorText = color;
            }

            private Enum isvisible;
            public Enum isVisible
            {
                get
                {
                    return this.isvisible;
                }

                set
                {
                    this.isvisible = value;
                    this.OnPropertyChanged("isVisible");
                }
            }

            private string text;
            public string Text
            {
                get
                {
                    return this.text;
                }

                set
                {
                    this.text = value;
                    this.OnPropertyChanged("Text");
                }
            }

            private string colorText;
            public string ColorText
            {
                get
                {
                    return this.colorText;
                }

                set
                {
                    this.colorText = value;
                    this.OnPropertyChanged("ColorText");
                }
            }

            private TextBlockClick icommand;
            public TextBlockClick iCommand
            {
                get
                {
                    return this.icommand;
                }
            }
        }

        public class TextBlockClick : ICommand
        {

            private TextBlockData viewModel;
            public TextBlockClick(TextBlockData viewModel)
            {
                this.viewModel = viewModel;
                this.viewModel.PropertyChanged += (s, e) =>
                {
                    if (this.CanExecuteChanged != null)
                    {
                        this.CanExecuteChanged(this, new EventArgs());
                    }
                };
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                TextBlock item = (parameter as TextBlock);
                SolidColorBrush r = item.Foreground as SolidColorBrush;
                //MessageBox.Show(r.Color.ToString());
                if (r.Color.ToString().Equals("#FFFF0000"))
                {
                    this.viewModel.ColorText = Colors.Black.ToString();

                    //(parameter as TextBlock).Foreground = new SolidColorBrush(Colors.Black);
                    //remove items in list holiday  
                    ListHoliday.Remove(item.Text);
                }
                else
                {
                    this.viewModel.ColorText = Colors.Red.ToString();
                    //(parameter as TextBlock).Foreground = new SolidColorBrush(Colors.Red);
                    //add item in list holiday
                    ListHoliday.Add(item.Text);
                }
            }
        }

        public class ChangeMonthCommand : ICommand
        {

            private CalendarEditViewModel viewModel;
            public ChangeMonthCommand(CalendarEditViewModel viewModel)
            {
                this.viewModel = viewModel;
                this.viewModel.PropertyChanged += (s, e) =>
                {
                    if (this.CanExecuteChanged != null)
                    {
                        this.CanExecuteChanged(this, new EventArgs());
                    }
                };
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                this.viewModel.ChangeMonthEvent(parameter.ToString());
            }
        }

        private static List<string> ListHoliday = new List<string>();

        public class DayOfWeekEditCulture
        {
            public DayOfWeekEditCulture(string culture)
            {
                if (culture.Equals("vi-VN"))
                {
                    this.Su = "CN";
                    this.Mo = "H";
                    this.Tu = "B";
                    this.We = "T";
                    this.Th = "N";
                    this.Fr = "S";
                    this.Sa = "B";
                }
                else
                {
                    this.Su = "Su";
                    this.Mo = "Mo";
                    this.Tu = "Tu";
                    this.We = "We";
                    this.Th = "Th";
                    this.Fr = "Fr";
                    this.Sa = "Sa";
                }
            }
            public string Su { get; set; }
            public string Mo { get; set; }
            public string Tu { get; set; }
            public string We { get; set; }
            public string Th { get; set; }
            public string Fr { get; set; }
            public string Sa { get; set; }
        }

        #endregion

    }
}
