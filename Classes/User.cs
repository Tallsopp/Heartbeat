using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Essentials;

namespace HeartbeatApp.Classes
{
    public class User : INotifyPropertyChanged
    {
        [XmlAttribute]
        public int ID { get; set; }

        public string userName
        {
            get; set;
        }

        public string Password { get; set; }

        public string Forename { get; set; }

        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public DateTime DateOfBirth { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Gender { get; set; }
        public DateTime SelectedDate { get; set; }

        public int StartingWeight { get; set; }
        public int GoalWeight { get; set; }

        private int _calorieIntake;
        public int CalorieIntake
        {
            get { return _calorieIntake; }
            set
            {
                _calorieIntake = value;
                NotifyPropertyChanged("CalorieIntake");
            }
        }

        public void ClearUser()
        {
            ID = -1;
            userName = "";
            Password = "";
            Forename = "";
            LastName = "";
            DateOfBirth = DateTime.Today;
            Height = 0;
            Weight = 0;
            Gender = -1;
            SelectedDate = DateTime.Today;
            StartingWeight = 0;
            GoalWeight = 0;
            _calorieIntake = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void LogIn(string _user, string _pass)
        {
            userName = _user;
            Password = _pass;
        }

        public void Register(string _user, string _pass, string _fore, string _last, DateTime _date, string _email)
        {
            userName = _user;
            Password = _pass;
            Forename = _fore;
            LastName = _last;
            DateOfBirth = (DateTime)_date;
            EmailAddress = _email;
            ID = 1;
        }

        public User()
        {
            userName = "";
            Password = "";
            Forename = "";
            LastName = "";
            DateOfBirth = DateTime.Today;
            EmailAddress = "";
        }

        public void SaveUserToFile()
        {
            string[] details = { Forename, LastName, userName, Password, ID.ToString() };

            //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string docPath = @"C:\";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "HeartbeatUser.txt")))
            {
                foreach (var line in details)
                    outputFile.WriteLine(line.ToString());
            }
        }

        public void ReadUserFile()
        {
            int counter = 0;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"c:\HeartbeatUser.txt");

            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);
                counter++;
            }

            file.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
        }
    }
}
