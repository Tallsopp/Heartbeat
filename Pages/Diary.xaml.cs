using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System.Data;
using System.Data.SqlClient;
using HeartbeatApp.Classes;
using System;
using System.Windows.Navigation;

namespace HeartbeatApp.Pages
{
    /// <summary>
    /// Interaction logic for Diary.xaml
    /// </summary>
    public partial class Diary : Page
    {
        //test variables
        private string currentDate;

        private int selectedCalories;
        private int selectedCarbs;
        private int selectedFat;
        private int selectedProtein;

        private int startingWeight = 0;
        private int targetWeight = 0;
        private int remainingCalories = 0;

        // Creates the Current User
        User u = new User();

        // Initialize Base Values
        public Diary()
        {
            InitializeComponent();
            CalorieCalculation();
        }

        // Checks if a String is of a Numeric Value, and Can Convert it
        public static bool isNumeric(string s)
        {
            return int.TryParse(s, out int n);
        }

        private void CalorieCalculation()
        {
            StartingWeight.Text = "Starting Weight: " + startingWeight;
            TargetWeight.Text = "Target Weight: " + targetWeight;
            RemainingCalories.Text = "Remaining: " + remainingCalories;
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

        // Get the Lsst ID number of a Database Table
        private int GetLastID(string connectionString)
        {
            int lastID = 0;

            using (Database.con)
            {
                Database.openConnection();
                Database.sql = connectionString;
                Database.cmd.CommandType = CommandType.Text;
                Database.cmd.CommandText = Database.sql;

                Database.dr = Database.cmd.ExecuteReader();

                while (Database.dr.Read())
                    lastID = Convert.ToInt32(Database.dr.GetValue(0)) + 1;

                Database.dr.Close();
                return lastID;
            }
        }

        // Updates the Database Table Recipes and ClientMeals, grabs the Page's data
        private void UpdateDiary(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)((Button)sender).Parent;
            StackPanel breakfast = (StackPanel)stackPanel.Children[0];
            StackPanel rowsPanel = (StackPanel)breakfast.Children[1];

            int id = GetLastID("SELECT MAX(ID) FROM Recipes");

            List<StackPanel> panels = new List<StackPanel>();
            List<string> v = new List<string>();

            foreach (StackPanel p in rowsPanel.Children)
                panels.Add(p);

            if (panels.Count > 0)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    for (int j = 0; j < panels[i].Children.Count; j++)
                    {
                        if (panels[i].Children[j] is TextBox)
                        {
                            List<TextBox> tempValues = new List<TextBox>();
                            tempValues.Add(panels[i].Children[j] as TextBox);

                            InsertToSql(tempValues);
                        }
                    }
                }

                void InsertToSql(List<TextBox> t)
                {
                    for (int i = 0; i < t.Count; i++)
                    {
                        v.Add(t[i].Text);
                    }
                }

                List<int> nutrients = new List<int>(); ;
                for (int i = 0; i < v.Count; i++)
                {
                    if (isNumeric(v[i]))
                        nutrients.Add(int.Parse(v[1]));
                }

                if (nutrients.Count >= 4)
                {
                    string insertNewRecipe = "insert into Recipes (ID, RecipeName, Calories, Carbs, Protein, Fat) values(" + id + ", '" + v[0] + "', " + nutrients[0] + ", " + nutrients[1] + ", " + nutrients[2] + ", " + nutrients[3] + ")";

                    using (Database.con)
                    {
                        Database.openConnection();
                        Database.sql = insertNewRecipe;
                        Database.cmd.CommandType = CommandType.Text;
                        Database.cmd.CommandText = Database.sql;
                        Database.dr = Database.cmd.ExecuteReader();

                        MessageBox.Show("Recipe Added", "Congrats", MessageBoxButton.OK, MessageBoxImage.Information);
                        Database.dr.Close();
                    }
                }
                else
                    MessageBox.Show("All Fields must be Accurate", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Database.closeConnection();
        }

        private void OpenBreakfast(object sender, RoutedEventArgs e)
        {
            if (panelBreakfast.Visibility == Visibility.Visible)
                CloseMenus();
            else
            {
                CloseMenus();
                panelBreakfast.Visibility = Visibility.Visible;
            }
        }

        private void OpenLunch(object sender, RoutedEventArgs e)
        {
            if (panelLunch.Visibility == Visibility.Visible)
                CloseMenus();
            else
            {
                CloseMenus();
                panelLunch.Visibility = Visibility.Visible;
            }
        }

        private void OpenDinner(object sender, RoutedEventArgs e)
        {
            if (panelDinner.Visibility == Visibility.Visible)
                CloseMenus();
            else
            {
                CloseMenus();
                panelDinner.Visibility = Visibility.Visible;
            }
        }

        private void OpenSnacks(object sender, RoutedEventArgs e)
        {
            if (panelSnacks.Visibility == Visibility.Visible)
                CloseMenus();
            else
            {
                CloseMenus();
                panelSnacks.Visibility = Visibility.Visible;
            }
        }

        private void OpenWater(object sender, RoutedEventArgs e)
        {
            if (panelWater.Visibility == Visibility.Visible)
                CloseMenus();
            else
            {
                CloseMenus();
                panelWater.Visibility = Visibility.Visible;
            }
        }

        private void CloseMenus()
        {
            List<StackPanel> stackPanels = new List<StackPanel>();
            stackPanels.Add(panelBreakfast);
            stackPanels.Add(panelLunch);
            stackPanels.Add(panelDinner);
            stackPanels.Add(panelSnacks);
            stackPanels.Add(panelWater);

            foreach (var p in stackPanels)
                p.Visibility = Visibility.Collapsed;
        }

        #endregion


        // Manages Navigation Between all Pages
        #region Page Navigation

        //// Loads User data when Page has finished Loading
        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            u = (User)e.ExtraData;
        }

        //// Go to Pages on CLick
        private void NavigateHome_Click(object sender, RoutedEventArgs e)
        {
            SignIn signInPage = new SignIn();
            NavigationService.Navigate(signInPage, u);
        }

        private void NavigateDiary_Click(object sender, RoutedEventArgs e)
        {
            Diary diaryPage = new Diary();
            NavigationService.Navigate(diaryPage, u);
        }

        #endregion

        // Handles all Events related to the Rows in the Grid/StackPanels related to the User's Meals
        #region Rows Events
        private void AddExistingItem(object sender, RoutedEventArgs e)
        {
            using (Database.con)
            {
                Database.openConnection();
                Database.sql = "";
                Database.cmd.CommandType = CommandType.Text;
                Database.cmd.CommandText = Database.sql;

                Database.dr = Database.cmd.ExecuteReader();

                if (Database.dr.HasRows)
                {
                    while (Database.dr.Read())
                    {

                    }
                }

                Database.dr.Close();
            }


            DataGrid dataGrid = new DataGrid();
            dataGrid.Background = new SolidColorBrush(Colors.White);
            dataGrid.AlternatingRowBackground = new SolidColorBrush(Colors.LightBlue);
            dataGrid.AlternationCount = 1;
            dataGrid.AutoGenerateColumns = true;
        }

        private void GridControls(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)((Button)sender).Parent;

            StackPanel rowsPanel = (StackPanel)stackPanel.Children[1];
            StackPanel buttonsPanel = (StackPanel)stackPanel.Children[2];

            if (rowsPanel.Visibility == Visibility.Collapsed)
            {
                rowsPanel.Visibility = Visibility.Visible;
                buttonsPanel.Visibility = Visibility.Visible;

                if (rowsPanel.Children.Count == 0)
                    AddRow(rowsPanel);
            }
            else if (stackPanel.Visibility == Visibility.Visible)
            {
                AddRow(rowsPanel);
            }
        }

        private void AddRow(StackPanel stack)
        {
            // Main StackPanel containing a row of values
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;

            TextBox foodName = new TextBox();
            foodName.Name = "foodName";
            foodName.Text = "Name of Meal";
            foodName.Width = 150;
            foodName.MouseEnter += MouseEntered;
            foodName.MouseLeave += MouseLeft;
            tempStack.Children.Add(foodName);

            TextBox txtCalories = new TextBox();
            txtCalories.Name = "txtCalories";
            txtCalories.Text = "Calories";
            txtCalories.Width = 75;
            txtCalories.MouseEnter += MouseEntered;
            txtCalories.MouseLeave += MouseLeft;
            tempStack.Children.Add(txtCalories);

            TextBox txtCarbs = new TextBox();
            txtCarbs.Name = "txtCarbs";
            txtCarbs.Text = "Carbs";
            txtCarbs.Width = 75;
            txtCarbs.MouseEnter += MouseEntered;
            txtCarbs.MouseLeave += MouseLeft;
            tempStack.Children.Add(txtCarbs);

            TextBox txtProtein = new TextBox();
            txtProtein.Name = "txtProtein";
            txtProtein.Text = "Protein";
            txtProtein.Width = 75;
            txtProtein.MouseEnter += MouseEntered;
            txtProtein.MouseLeave += MouseLeft;
            tempStack.Children.Add(txtProtein);

            TextBox txtFat = new TextBox();
            txtFat.Name = "txtFat";
            txtFat.Text = "Fat";
            txtFat.Width = 75;
            txtFat.MouseEnter += MouseEntered;
            txtFat.MouseLeave += MouseLeft;
            tempStack.Children.Add(txtFat);

            Button btnDelete = new Button();
            btnDelete.Content = "Delete?";
            btnDelete.HorizontalAlignment = HorizontalAlignment.Right;
            btnDelete.Click += DeleteRow;
            tempStack.Children.Add(btnDelete);

            stack.Children.Add(tempStack);
        }

        private void DeleteRow(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            StackPanel stackPanel = (StackPanel)btn.Parent;
            StackPanel main = (StackPanel)stackPanel.Parent;
            TextBox recipeName = stackPanel.Children[0] as TextBox;

            if (!String.IsNullOrWhiteSpace(recipeName.Text))
            {
                using (Database.con)
                {
                    Database.openConnection();
                    Database.sql = "DELETE FROM Recipes WHERE RecipeName like " + "'" + recipeName.Text + "'";
                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.dr = Database.cmd.ExecuteReader();

                    MessageBox.Show("Food Item Removed", "Congrats", MessageBoxButton.OK, MessageBoxImage.Information);

                    Database.dr.Close();

                    Database.closeConnection();
                }
            }
            main.Children.Remove(stackPanel);
        }

        private void CancelItem(object sender, RoutedEventArgs e)
        {
            StackPanel buttonsPanel = (StackPanel)((Button)sender).Parent;
            StackPanel subPanel = (StackPanel)buttonsPanel.Parent;
            StackPanel rowsPanel = (StackPanel)subPanel.Children[1];

            buttonsPanel.Visibility = Visibility.Collapsed;
            rowsPanel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region MouseEvents
        private void MouseEntered(object sender, RoutedEventArgs e)
        {
            string currText = ((TextBox)sender).Text;
            switch (currText)
            {
                case "Name of Meal": ((TextBox)sender).Text = ""; break;
                case "Calories": ((TextBox)sender).Text = ""; break;
                case "Carbs": ((TextBox)sender).Text = ""; break;
                case "Protein": ((TextBox)sender).Text = ""; break;
                case "Fat": ((TextBox)sender).Text = ""; break;
            }
        }
        private void MouseLeft(object sender, RoutedEventArgs e)
        {
            string textName = ((TextBox)sender).Name;
            switch (textName)
            {
                case "foodName":
                    if (((TextBox)sender).Text == "")
                        ((TextBox)sender).Text = "Name of Meal"; break;
                case "txtCalories":
                    if (((TextBox)sender).Text == "")
                        ((TextBox)sender).Text = "Calories"; break;
                case "txtCarbs":
                    if (((TextBox)sender).Text == "")
                        ((TextBox)sender).Text = "Carbs"; break;
                case "txtProtein":
                    if (((TextBox)sender).Text == "")
                        ((TextBox)sender).Text = "Protein"; break;
                case "txtFat":
                    if (((TextBox)sender).Text == "")
                        ((TextBox)sender).Text = "Fat"; break;
            }
        }
        #endregion

    }
}
