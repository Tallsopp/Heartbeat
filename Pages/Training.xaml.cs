using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System.Data;
using System.Data.SqlClient;
using HeartbeatApp.Classes;
using System;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Xml.Linq;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace HeartbeatApp.Pages
{
    /// <summary>
    /// Interaction logic for Diary.xaml
    /// </summary>
    /// 
    public partial class Training : Page
    {

        mDatePicker mDate = new mDatePicker { Date = DateTime.Today };

        // Creates the Current User
        User u = new User();

        // Initialize Base Values
        public Training(User _user)
        {
            InitializeComponent();

            u = _user;

            GetClientID();

            LoadExercises();
        }

        private void LoadExercises()
        {
            try
            {
                using (Database.con)
                {
                    Database.openConnection();

                    Database.sql = "select w.Name, w.Description from Workouts w INNER JOIN ClientWorkouts cw on cw.WorkoutID = w.ID INNER JOIN Clients c on c.ID = cw.ClientID Where c.ID = " + u.ID + " and DayOfWorkout= '" + mDate.Date.ToString("yyyy-MM-dd") + "'";

                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    DataTable dt = new DataTable("ClientWorkouts");
                    SqlDataAdapter adapter = new SqlDataAdapter(Database.cmd);
                    adapter.Fill(dt);

                    gridExercise.ItemsSource = dt.DefaultView;
                }
                Database.closeConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
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

        private void LogOut(object sender, RoutedEventArgs e)
        {
            u.ClearUser();

            SignIn signInPage = new SignIn();
            NavigationService.Navigate(signInPage, u);
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

        private void NavigateProfile(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(u);
            NavigationService.Navigate(profile, u);
        }

        private void NavigateMeals(object sender, RoutedEventArgs e)
        {
            Meals page = new Meals(u);
            NavigationService.Navigate(page, u);
        }

        #endregion

        // Custom Search Box, Updates Selection and DataGrid
        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = searchBar.Text;

            if (text.Length >= 2)
            {
                try
                {
                    using (Database.con)
                    {
                        Database.openConnection();
                        Database.sql = "SELECT * FROM Workouts WHERE Name like '" + text + "%'";

                        Database.cmd.CommandType = CommandType.Text;
                        Database.cmd.CommandText = Database.sql;
                        DataTable dt = new DataTable("Workouts");
                        SqlDataAdapter adapter = new SqlDataAdapter(Database.cmd);
                        adapter.Fill(dt);

                        exerciseList.ItemsSource = dt.DefaultView;
                    }
                    Database.closeConnection();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
                }
            }
        }

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

        private void AddCurrentSelection(object sender, RoutedEventArgs e)
        {
            try
            {
                using (Database.con)
                {
                    Database.openConnection();
                    Database.sql =
                        "EXEC AddWorkoutToClient @Date='" + mDate.Date.ToString("yyyy-MM-dd") +
                        "', @WorkoutID=" + exerciseList.SelectedValue + ", @ClientID=" + u.ID + ", @Time= " + int.Parse(timePassed.Text) + ", @Weight= " + u.Weight;

                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.dr = Database.cmd.ExecuteReader();
                }
                Database.closeConnection();

                u.SaveUserToFile();

                LoadExercises();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        private void DiaryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            mDate.Date = (DateTime)DiaryDate.SelectedDate;
            LoadExercises();
        }
    }
}