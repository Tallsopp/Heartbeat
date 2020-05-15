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

namespace HeartbeatApp.Pages
{
    /// <summary>
    /// Interaction logic for Breakfast.xaml
    /// </summary>
    /// 
    public partial class Meals : Page
    {
        private int _mealType = -1;

        // Open App with Today's Date
        mDatePicker mDate = new mDatePicker { Date = DateTime.Today };

        // Creates the Current User
        User u = null;

        // Initialize Base Values
        public Meals(User _user)
        {
            InitializeComponent();

            u = _user;

            GetClientID();
        }

        #region Grid Controls
        // Totals up all SelectedDate Nutritions, Calories, Carbs, Etc...
        private void CalorieCalculation()
        {
            int totalCalories = 0;
            int totalCarbs = 0;
            int totalProtein = 0;
            int totalFat = 0;

            for (int j = 1; j < 5; j++)
            {
                for (int v = 0; v < gridBreakfast.Items.Count; v++)
                {
                    DataRowView row = gridBreakfast.Items[v] as DataRowView;
                    int val = (int)row.Row.ItemArray[j];

                    switch (j)
                    {
                        case 1: totalCalories += val; break;
                        case 2: totalCarbs += val; break;
                        case 3: totalProtein += val; break;
                        case 4: totalFat += val; break;
                    }
                }
            }
            NutrientBreakdown(totalCalories, totalCarbs, totalFat, totalProtein);
        }

        // Break down of Calories and Checks
        private void NutrientBreakdown(int cals, int carbs, int fat, int protein)
        {
            // Values
            allCals.Text = cals.ToString();
            allCarbs.Text = carbs.ToString();
            allFat.Text = fat.ToString();
            allProtein.Text = protein.ToString();
        }

        // Get the Current Client's ID at the Start
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

        // Load All DataGrids with DataTables
        private void LoadMeals()
        {
            try
            {
                using (Database.con)
                {
                    Database.openConnection();

                    Database.sql = "SELECT Name, Calories, Carbs, Protein, Fat FROM ClientMeals where DateOfMeal= '" + mDate.Date.ToString("yyyy/MM/dd") + "' and MealType =" + _mealType + " and ClientID=" + u.ID;

                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;
                    DataTable dt = new DataTable("Meals");
                    SqlDataAdapter adapter = new SqlDataAdapter(Database.cmd);
                    adapter.Fill(dt);

                    gridBreakfast.ItemsSource = dt.DefaultView;
                }
                Database.closeConnection();
                CalorieCalculation();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        #endregion

        #region Add and Remove Items
        // Add SelectedItem and Add to ClientMeals from Search Bar
        private void AddSearchedItem(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView recipes = (DataRowView)((Button)e.Source).DataContext;
                string name = recipes[0].ToString();
                int calories = (int)recipes[1];
                int carbs = (int)recipes[2];
                int protein = (int)recipes[3];
                int fat = (int)recipes[4];

                using (Database.con)
                {
                    Database.openConnection();
                    Database.sql = "insert into ClientMeals (Name, Calories, Carbs, Protein, Fat, DateOfMeal, MealType, ClientID) Values('" + name + "', "
                        + calories + "," + carbs + "," + protein + "," + fat + ",'" + mDate.Date.ToString("yyyy/MM/dd") + "', " + _mealType + ", " + u.ID + ")";

                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.cmd.ExecuteNonQuery();
                    MessageBox.Show("New item added to your Diary!", "Item Added!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Database.closeConnection();
                LoadMeals();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }

        }

        // Add Custom Item to ClientMeals
        private void AddCustomFood(object sender, RoutedEventArgs e)
        {
            try
            {
                bool canAdd = false;

                // Get all values and convert
                string tempName = boxName.Text;
                List<string> values = new List<string>();
                values.Add(boxCalories.Text);
                values.Add(boxCarbs.Text);
                values.Add(boxProtein.Text);
                values.Add(boxFat.Text);

                // Check they are correct variables, if not add a message
                if (tempName.Length == 0)
                    MessageBox.Show("There is nothing in Name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Check every value if it is a numeric value, if not break method
                foreach (string s in values)
                {
                    int i = 0;
                    bool checker = int.TryParse(s, out i);
                    canAdd = checker;
                }

                if (canAdd == true)
                {
                    int tempCals = int.Parse(boxCalories.Text);
                    int tempCarbs = int.Parse(boxCarbs.Text);
                    int tempProtein = int.Parse(boxProtein.Text);
                    int tempFat = int.Parse(boxFat.Text);

                    using (Database.con)
                    {
                        Database.openConnection();
                        Database.sql = "insert into ClientMeals (Name, Calories, Carbs, Protein, Fat, DateOfMeal, MealType, ClientID) Values('"
                            + tempName + "', " + tempCals + ", " + tempCarbs + ", " + tempProtein + ", " + tempFat + ", '" +
                            mDate.Date.ToString("yyyy/MM/dd") + "', " + _mealType + ", " + u.ID + ")";

                        Database.cmd.CommandType = CommandType.Text;
                        Database.cmd.CommandText = Database.sql;

                        Database.cmd.ExecuteNonQuery();
                        MessageBox.Show("New item added to your Diary!", "Item Added!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    Database.closeConnection();
                    LoadMeals();
                }
                else
                    MessageBox.Show("Values must be accurate and numeric", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        // Remove Selected Item from ClientMeals
        private void btnRemoveItem(object sender, RoutedEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // iteratively traverse the visual tree
            while ((dep != null) &&
            !(dep is DataGridCell) &&
            !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridColumnHeader)
            {
                DataGridColumnHeader columnHeader = dep as DataGridColumnHeader;
            }

            if (dep is DataGridCell)
            {
                // navigate further up the tree
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                try
                {
                    DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
                    string item = dataRowView[0].ToString();
                    int cals = int.Parse(dataRowView[1].ToString());

                    using (Database.con)
                    {
                        Database.openConnection();
                        Database.sql = "Delete FROM ClientMeals WHERE Name like '" + item + "' and ClientID = " + u.ID;

                        Database.cmd.CommandType = CommandType.Text;
                        Database.cmd.CommandText = Database.sql;

                        Database.cmd.ExecuteNonQuery();
                        MessageBox.Show("You Deleted : " + item);
                    }
                    Database.closeConnection();
                    LoadMeals();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        #endregion

        // All Menu Interactions, Open Closes etc
        #region Menu Management
        // Open up the Make Your Own Menu
        private void btnMakeOwnMenu(object sender, RoutedEventArgs e)
        {
            if (menuMakeOwn.Visibility == Visibility.Hidden)
                menuMakeOwn.Visibility = Visibility.Visible;
            else
                menuMakeOwn.Visibility = Visibility.Hidden;
        }

        // Open Up Custom Add Item Menu
        private void btnOpenAddItem(object sender, RoutedEventArgs e)
        {
            // Get the Button with the Name and Assign MealType
            Button _button = (Button)e.Source;
            string _name = _button.Name;

            // Set Visibility of Add Menu
            if (menuAddItem.Visibility == Visibility.Collapsed)
                menuAddItem.Visibility = Visibility.Visible;
            else if (menuAddItem.Visibility == Visibility.Visible)
            {
                ClearMenus();
            }
        }

        // Clear all menus
        private void ClearMenus()
        {
            menuAddItem.Visibility = Visibility.Collapsed;
            boxName.Clear();
            boxCalories.Clear();
            boxCarbs.Clear();
            boxProtein.Clear();
            boxFat.Clear();
            searchBar.Clear();
            gridSearch.ItemsSource = null;
        }
        #endregion

        // Controls the Side Menu
        #region Button Clicks

        // Opens the Side Menu
        private void MenuOpen(object sender, RoutedEventArgs e)
        {
            SideMenuOpen.Visibility = Visibility.Collapsed;
            SideMenuClose.Visibility = Visibility.Visible;
        }
        // Closes the Side Menu
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

        private void NavigateDiary(object sender, RoutedEventArgs e)
        {
            Diary diary = new Diary(u);
            NavigationService.Navigate(diary, u);
        }

        // Go to Pages on Click
        private void NavigateTraining(object sender, RoutedEventArgs e)
        {
            Training training = new Training(u);
            NavigationService.Navigate(training, u);
        }

        private void NavigateProfile(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(u);
            NavigationService.Navigate(profile, u);
        }

        #endregion

        // Opens the Log Out Context Menu
        private void MenuLogOut(object sender, RoutedEventArgs e)
        {
            //open or close the menu
            if (menuLogOut.Visibility == Visibility.Hidden)
                menuLogOut.Visibility = Visibility.Visible;
            else
                menuLogOut.Visibility = Visibility.Hidden;
        }

        // Log Out the User and Clear it
        private void LogOutUser(object sender, RoutedEventArgs e)
        {
            // click to log out the user
            u.ClearUser();

            SignIn signIn = new SignIn();
            NavigationService.Navigate(signIn);
        }

        // Store the value when the date is changed
        private void DiaryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            mDate.Date = (DateTime)DiaryDate.SelectedDate;
            LoadMeals();
        }

        // Custom Search Box, Updates Selection and DataGrid
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = searchBar.Text;

            if (text.Length >= 2)
            {
                try
                {
                    using (Database.con)
                    {
                        Database.openConnection();
                        Database.sql = "SELECT RecipeName, Calories, Carbs, Protein, Fat FROM Recipes where RecipeName like '" + text + "%'";

                        Database.cmd.CommandType = CommandType.Text;
                        Database.cmd.CommandText = Database.sql;
                        DataTable dt = new DataTable("Meals");
                        SqlDataAdapter adapter = new SqlDataAdapter(Database.cmd);
                        adapter.Fill(dt);

                        gridSearch.ItemsSource = dt.DefaultView;
                    }
                    Database.closeConnection();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
                }
            }
        }

        // Select the current meal
        private void SelectMeal(object sender, SelectionChangedEventArgs e)
        {
            _mealType = MealSelect.SelectedIndex;
            LoadMeals();
        }
    }
}
