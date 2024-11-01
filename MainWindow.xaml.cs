using System.Data;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using Npgsql.Replication;
using Microsoft.VisualBasic;
using System.Numerics;
using System.Collections.Specialized;
using System.Security.AccessControl;
using System.Collections.ObjectModel;

// will want to remove this if do not use it
namespace System.Windows.Controls
{
    public static class MyExt
    {
        // Adding function that is on Forms to WPF
        public static void PerformClick(this Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}

namespace book_tracker
{
    public partial class MainWindow : Window
    {
        public class Books
        {
            public string list_novelid { get; set; }
            public string list_bookname { get; set; }
            public string list_book_buy { get; set; }
            public string list_book_total { get; set; }
            public string list_book_image { get; set; }

        }

        string connectionString = @"Server=server_name;Port=port_number;User Id=postgres;Password=server_pw;Database=name_of_database";
        NpgsqlCommand command = new NpgsqlCommand();


        public MainWindow()
        {
            InitializeComponent();
            TotalCollected();
            listDisplay();
            
        }

       // public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        private void listDisplay()
        {
            using (NpgsqlConnection npgCon = new NpgsqlConnection(connectionString))
            {
                npgCon.Open();
                NpgsqlCommand npgQuery = new NpgsqlCommand("Select * from light_novel", npgCon);
                npgQuery.Connection = npgCon;
                NpgsqlDataAdapter npgAdapter = new NpgsqlDataAdapter(npgQuery);

                DataTable bookDataTable = new DataTable();
                npgAdapter.Fill(bookDataTable);

                // total count of the list from database
                int lstCount = bookDataTable.Rows.Count;
                

                int curCount = lstCount;

                int i = 0;

                List<Books> items = new List<Books>();
                while (lstCount > i)
                {
                    items.Add(new Books()
                    {
                        list_novelid = bookDataTable.Rows[i]["novel_id"].ToString(),
                        list_bookname = bookDataTable.Rows[i]["title"].ToString(),
                        list_book_buy = bookDataTable.Rows[i]["vol_bought"].ToString(),
                        list_book_total = bookDataTable.Rows[i]["total_vol"].ToString(),
                        list_book_image = bookDataTable.Rows[i]["book_image"].ToString()
                    });

                   
                    displayTest.ItemsSource = items;
                    i++;

                    // filter through book names from list and refresh the list
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(displayTest.ItemsSource);
                    view.Filter = UserBookFilter;

                }

                npgCon.Close();
            }
        }

        private void TotalCollected()
        {
            using (NpgsqlConnection npgCon = new NpgsqlConnection(connectionString))
            {
                npgCon.Open();
                command.Connection = npgCon;
                command.CommandText = "SELECT SUM(vol_bought) FROM light_novel"; 
                TotalBooks.Text = command.ExecuteScalar().ToString(); //green underline means warning
                npgCon.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private bool UserBookFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
                return true;
            else
                return ((item as Books).list_bookname.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void BookFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(displayTest.ItemsSource).Refresh();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddBookWindow addBookWindow = new AddBookWindow();
            addBookWindow.Show();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newWindow = new MainWindow();
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            Close();
        }
    }
}