using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using HeartbeatApp.Classes;
using System;
using System.Windows.Navigation;
using Xamarin.Essentials;

namespace HeartbeatApp.Pages
{
    /// <summary>
    /// Interaction logic for Diary.xaml
    /// </summary>
    /// 
    public partial class Profile : Page
    {
        // Creates the Current User
        User u = new User();

        // Initialize Base Values
        public Profile(User _user)
        {
            InitializeComponent();

            u = _user;

            GetClientID();
            LoadProfile();
        }

        private void GetClientID()
        {
            try
            {
                using (Database.con)
                {
                    Database.openConnection();
                    Database.sql = "exec GetClientIDFromUser @Username = '" + u.userName + "', @Password = '" + u.Password + "'";
                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.dr = Database.cmd.ExecuteReader();

                    while (Database.dr.Read())
                        u.ID = Database.dr.GetInt32(0);
                }
                Database.closeConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        // Load Profile and load to Profile Page
        private void LoadProfile()
        {
            try
            {
                using (Database.con)
                {
                    Database.openConnection();
                    Database.sql = "SELECT Forename, LastName, DateOfBirth, Height, Weight, Gender from Clients where ID = " + u.ID;
                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.dr = Database.cmd.ExecuteReader();
                }
                Database.closeConnection();

                FirstName.Text = u.Forename;
                LastName.Text = u.LastName;
                DateofBirth.SelectedDate = u.DateOfBirth;
                Height.Text = u.Height.ToString();
                Weight.Text = u.Weight.ToString();
                EmailAddress.Text = u.EmailAddress;

                int gender = u.Gender;

                // Male
                if (gender == 0)
                {
                    genderMale.IsChecked = true;
                    genderFemale.IsChecked = false;
                }
                // Female
                else if (gender == 1)
                {
                    genderMale.IsChecked = false;
                    genderFemale.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        private void LogOut(object sender, RoutedEventArgs e)
        {
            u.ClearUser();

            SignIn signInPage = new SignIn();
            NavigationService.Navigate(signInPage, u);
        }

        // Once finished, Profile updated to the Database and Displayed
        private void UpdateProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                int height = int.Parse(Height.Text);
                int weight = int.Parse(Weight.Text);
                int gender = -1;

                DateTime date = (DateTime)DateofBirth.SelectedDate;

                // Male
                if (genderMale.IsChecked == true && genderFemale.IsChecked == false)
                {
                    gender = 0;
                    genderMale.IsChecked = true;
                    genderFemale.IsChecked = false;
                }
                // Female
                else if (genderFemale.IsChecked == true && genderMale.IsChecked == false)
                {
                    gender = 1;
                    genderMale.IsChecked = false;
                    genderFemale.IsChecked = true;
                }

                using (Database.con)
                {
                    Database.openConnection();

                    Database.sql =
                        "exec UpdateProfile @Forename='" + FirstName.Text + "', @Surname='" + LastName.Text +
                        "', @DateOfBirth='" + date.ToString("yyyy-MM-dd") + "', @EmailAddress='" + EmailAddress.Text +
                        "', @Weight=" + weight + ", @Height=" + height + ", @Gender=" + gender + ", @ClientID=" + u.ID + "";

                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;
                    DataTable dt = new DataTable("User");
                }

                MessageBox.Show("Profile Updated!", "Updated!", MessageBoxButton.OK, MessageBoxImage.Information);

                u.Forename = FirstName.Text;
                u.LastName = LastName.Text;
                u.DateOfBirth = date;
                u.Height = height;
                u.Weight = weight;
                u.Gender = gender;
                u.EmailAddress = EmailAddress.Text;

                Database.closeConnection();

                u.SaveUserToFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        // All Button Click events
        #region Button Clicks

        // Opens the Side Menu
        private void MenuOpen(object sender, RoutedEventArgs e)
        {
            SideMenuOpen.Visibility = Visibility.Collapsed;
            SideMenuClose.Visibility = Visibility.Visible;
        }

        private void MenuClose(object sender, RoutedEventArgs e)
        {
            SideMenuOpen.Visibility = Visibility.Visible;
            SideMenuClose.Visibility = Visibility.Collapsed;
        }

        // Closes the Side Menu
        private void ButtonMenuClose_Click(object sender, RoutedEventArgs e)
        {
            object objOpen = FindName("ButtonMenuOpen");
            object objClose = FindName("ButtonMenuClose");

            Button btnOpen = (objOpen as Button);
            Button btnClose = (objClose as Button);

            if (btnOpen != null && btnClose != null)
            {
                btnOpen.Visibility = Visibility.Visible;
                btnClose.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        // Manages Navigation Between all Pages
        #region Page Navigation

        // Loads User data when Page has finished Loading
        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            u = (User)e.ExtraData;
        }

        // Go to Pages on CLick
        private void NavigateDiary_Click(object sender, RoutedEventArgs e)
        {
            Diary diaryPage = new Diary(u);
            NavigationService.Navigate(diaryPage, u);
        }

        private void NavigateMeals(object sender, RoutedEventArgs e)
        {
            Meals page = new Meals(u);
            NavigationService.Navigate(page, u);
        }

        private void NavigateTraining(object sender, RoutedEventArgs e)
        {
            Training training = new Training(u);
            NavigationService.Navigate(training, u);
        }

        #endregion


        private void MenuLogOut(object sender, RoutedEventArgs e)
        {
            //open or close the menu
        }

        private void LogOutUser(object sender, RoutedEventArgs e)
        {
            // click to log out the user
            u.ClearUser();

            SignIn signIn = new SignIn();
            NavigationService.Navigate(signIn);

        }

        private void genderMale_Checked(object sender, RoutedEventArgs e)
        {
            genderFemale.IsChecked = false;
        }

        private void genderFemale_Checked(object sender, RoutedEventArgs e)
        {
            genderMale.IsChecked = false;
        }
    }
}
