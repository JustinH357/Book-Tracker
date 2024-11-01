using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static book_tracker.MainWindow;

namespace book_tracker
{
    public partial class AddBookWindow : Window
    {
        string connectionString = @"Server=server_name;Port=port_number;User Id=postgres;Password=server_pw;Database=name_of_database";
        NpgsqlCommand command = new NpgsqlCommand();

        

        public AddBookWindow()
        {
            InitializeComponent();
        }

        
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection npgCon = new NpgsqlConnection(connectionString))
            {
                npgCon.Open();
                NpgsqlCommand checkBookName = new NpgsqlCommand("select count (*) from light_novel where title ilike '%' || @bookName || '%'", npgCon);
                checkBookName.Parameters.AddWithValue("@bookName", TypeBookName.Text);
                int BookExist = (int)(long)checkBookName.ExecuteScalar();

                // if book exist, the count in the Db will be 1 which is greater than 0 meaning it exist
                if (BookExist > 0)
                {
                    FindDupeTest.Text = "book is in Db or empty value";

                } 
                else
                {
                    // clear the textblock when the add button is click to check if book exist in DB
                    FindDupeTest.Text = string.Empty;

                    // add title and book image for now to Db
                    NpgsqlCommand addBook = new NpgsqlCommand("insert into light_novel (title, book_image) values (@title, @bookImage)", npgCon);
                    addBook.Parameters.AddWithValue("@title", TypeBookName.Text);
                    addBook.Parameters.AddWithValue("@bookImage", FileName.Text);

                    //ExecuteNonQuery is needed to perform the insert into on Postgres
                    addBook.ExecuteNonQuery();

                    Books books = new Books();
                    
                    // clear the text from textbox and file 
                    TypeBookName.Clear();
                    FileName.Text = string.Empty;

                    FindDupeTest.Text = "Sucessfully Added to Db";

             
                     // this window close when add is successfully done
                    Close();
                }
                npgCon.Close();
                
               

            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                FileName.Text = filename;
            }
        }


        // messing around with button disappear on click
       /* private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Hides Current Button
            button1.Visibility = Visibility.Collapsed;

            //Checks and Shows Button 2
            if (button2.Visibility == Visibility.Collapsed)
            {
                button2.Visibility = Visibility.Visible;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //Hides Current Button
            button2.Visibility = Visibility.Collapsed;

            //Checks and Shows Button 1
            if (button1.Visibility == Visibility.Collapsed)
            {
                button1.Visibility = Visibility.Visible;
            }
        }*/
    }
}
