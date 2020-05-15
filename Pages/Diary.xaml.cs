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
using System.Xml;
using System.Windows.Controls.Primitives;
using System.IO;

namespace HeartbeatApp.Pages
{
    /// <summary>
    /// Interaction logic for Diary.xaml
    /// </summary>
    /// 
    public partial class Diary : Page
    {
        // Open App with Today's Date
        mDatePicker mDate = new mDatePicker { Date = DateTime.Today };

        // Creates the Current User
        User u = null;

        // Initialize Base Values
        public Diary(User _user)
        {
            InitializeComponent();

            u = _user;

            DiaryDate.DataContext = mDate;
            DiaryDate.SelectedDate = DateTime.Today;

            GetClientID();
        }

        // Set and validate the Current User logged in
        private void SetUser()
        {
            try
            {
                using (Database.con)
                {
                    Database.openConnection();
                    Database.sql = "select Forename, LastName, Height, Weight, Gender, CalorieIntake, DateOfBirth from Clients WHERE ID =" + u.ID;
                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.dr = Database.cmd.ExecuteReader();

                    while (Database.dr.Read())
                    {
                        u.Forename = Database.dr.GetString(0);
                        u.LastName = Database.dr.GetString(1);
                        u.Height = Database.dr.GetInt32(2);
                        u.Weight = Database.dr.GetInt32(3);
                        u.Gender = Database.dr.GetInt32(4);
                        u.CalorieIntake = Database.dr.GetInt32(5);
                        u.DateOfBirth = Database.dr.GetDateTime(6);
                    }
                }
                Database.closeConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
                Database.closeConnection();
            }
        }

        #region Grid Controls
        // Totals up all SelectedDate Nutritions, Calories, Carbs, Etc...
        private void CalorieCalculation()
        {
            int cals = 0, carbs = 0, fat = 0, protein = 0;

            try
            {
                using (Database.con)
                {
                    Database.openConnection();
                    Database.sql = "exec SumCaloriesByDay @Date='" + mDate.Date.ToString("yyyy-MM-dd") + "', @ClientID= " + u.ID;
                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.dr = Database.cmd.ExecuteReader();

                    while (Database.dr.Read())
                    {
                        cals = Database.dr.GetInt32(0);
                        carbs = Database.dr.GetInt32(1);
                        protein = Database.dr.GetInt32(2);
                        fat = Database.dr.GetInt32(3);
                    }
                }
                Database.closeConnection();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }

            NutrientBreakdown(cals, carbs, protein, fat);
        }

        // Break down of Calories and Checks
        private void NutrientBreakdown(int cals, int carbs, int fat, int protein)
        {
            currWeight.Text = "Weight: " + u.Weight;

            targetCals.Text = "Target Calories: " +  u.CalorieIntake + " kcals";

            consumeCals.Text = "Remaining: " + (u.CalorieIntake - cals).ToString() + " kcals";

            #region Assigning to XAML

                // Values
                allCals.Text = cals.ToString();
                allCarbs.Text = carbs.ToString();
                allFat.Text = fat.ToString();
                allProtein.Text = protein.ToString();

                // Daily
                DailyCals.Text = u.CalorieIntake.ToString();
                DailyCarbs.Text = (u.CalorieIntake / 0.6f).ToString();
                DailyFat.Text = (u.CalorieIntake / 0.15f).ToString();
                DailyProtein.Text = (u.CalorieIntake / 0.25f).ToString();

                // Remaining
                RemainingCals.Text = consumeCals.Text;
                RemainingCarbs.Text = (u.CalorieIntake - carbs).ToString();
                RemainingFat.Text = (u.CalorieIntake - fat).ToString();
                RemainingProtein.Text = (u.CalorieIntake - protein).ToString();

            #region Styles Table
            // Calories
            if (cals > 0)
            {
                if (cals > u.CalorieIntake)
                {
                    allCals.Background = new SolidColorBrush(Colors.Red);
                    DailyCals.Background = new SolidColorBrush(Colors.Red);
                    RemainingCals.Background = new SolidColorBrush(Colors.Red);
                }
                else if (cals < (u.CalorieIntake - 200))
                {
                    allCals.Background = new SolidColorBrush(Colors.Green);
                    DailyCals.Background = new SolidColorBrush(Colors.Green);
                    RemainingCals.Background = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    allCals.Background = new SolidColorBrush(Colors.LightGray);
                    DailyCals.Background = new SolidColorBrush(Colors.LightGray);
                    RemainingCals.Background = new SolidColorBrush(Colors.LightGray);
                }

                // Carbs
                if (carbs > (u.CalorieIntake / 0.6f))
                {
                    allCals.Background = new SolidColorBrush(Colors.Red);
                    DailyCals.Background = new SolidColorBrush(Colors.Red);
                    RemainingCals.Background = new SolidColorBrush(Colors.Red);
                }
                else if (carbs < (u.CalorieIntake / 0.6f))
                {
                    allCals.Background = new SolidColorBrush(Colors.Green);
                    DailyCals.Background = new SolidColorBrush(Colors.Green);
                    RemainingCals.Background = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    allCals.Background = new SolidColorBrush(Colors.LightGray);
                    DailyCals.Background = new SolidColorBrush(Colors.LightGray);
                    RemainingCals.Background = new SolidColorBrush(Colors.LightGray);
                }

                // Protein
                if (protein > (u.CalorieIntake / 0.25f))
                {
                    allCals.Background = new SolidColorBrush(Colors.Red);
                    DailyCals.Background = new SolidColorBrush(Colors.Red);
                    RemainingCals.Background = new SolidColorBrush(Colors.Red);
                }
                else if (protein < (u.CalorieIntake / 0.25f))
                {
                    allCals.Background = new SolidColorBrush(Colors.Green);
                    DailyCals.Background = new SolidColorBrush(Colors.Green);
                    RemainingCals.Background = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    allCals.Background = new SolidColorBrush(Colors.LightGray);
                    DailyCals.Background = new SolidColorBrush(Colors.LightGray);
                    RemainingCals.Background = new SolidColorBrush(Colors.LightGray);
                }

                // Fat
                if (fat > (u.CalorieIntake / 0.25f))
                {
                    allCals.Background = new SolidColorBrush(Colors.Red);
                    DailyCals.Background = new SolidColorBrush(Colors.Red);
                    RemainingCals.Background = new SolidColorBrush(Colors.Red);
                }
                else if (fat < (u.CalorieIntake / 0.25f))
                {
                    allCals.Background = new SolidColorBrush(Colors.Green);
                    DailyCals.Background = new SolidColorBrush(Colors.Green);
                    RemainingCals.Background = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    allCals.Background = new SolidColorBrush(Colors.LightGray);
                    DailyCals.Background = new SolidColorBrush(Colors.LightGray);
                    RemainingCals.Background = new SolidColorBrush(Colors.LightGray);
                }
            }
            #endregion

            #endregion
        }

        // From ClientAuthenticate, get ID, then Set up the User account
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

            SetUser();

        }

        // Add Exercise to Grid
        private void AddExercise(object sender, RoutedEventArgs e)
        {
            u.SelectedDate = (DateTime)DiaryDate.SelectedDate;

            Training training = new Training(u);
            NavigationService.Navigate(training, u);
        }

        #endregion

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

        //// Loads User data when Page has finished Loading
        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            u = (User)e.ExtraData;
        }

        // Go to Pages on Click
        private void NavigateTraining(object sender, RoutedEventArgs e)
        {
            Training training = new Training(u);
            NavigationService.Navigate(training, u);
        }

        private void NavigateMeals(object sender, RoutedEventArgs e)
        {
            Meals page = new Meals(u);
            NavigationService.Navigate(page, u);
        }

        private void NavigateProfile(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(u);
            NavigationService.Navigate(profile, u);
        }

        #endregion

        // Diary Date Changed, Updates Grids to SelectedDate
        private void DiaryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            mDate.Date = (DateTime)DiaryDate.SelectedDate;
            CalorieCalculation();
        }


        private void MenuLogOut(object sender, RoutedEventArgs e)
        {
            //open or close the menu
            if (menuLogOut.Visibility == Visibility.Hidden)
                menuLogOut.Visibility = Visibility.Visible;
            else
                menuLogOut.Visibility = Visibility.Hidden;
        }

        private void LogOutUser(object sender, RoutedEventArgs e)
        {
            // click to log out the user
            u.ClearUser();

            SignIn signIn = new SignIn();
            NavigationService.Navigate(signIn);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                u.ReadUserFile();
            }
            catch (IOException ex)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
            u.SaveUserToFile();
            }
        }
    }
}
