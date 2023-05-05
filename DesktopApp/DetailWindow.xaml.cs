using DataLayer.Models;
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

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>

    public partial class DetailWindow : Window
    {
        private Reservation Reservation;

        public DetailWindow(Reservation reservation)
        {
            InitializeComponent();
            this.Reservation = reservation;
            DataContext = reservation;
        }

        private void UpdateReservation_Click(object sender, RoutedEventArgs e)
        {
            // Perform the necessary update logic based on the changes made in the UI
            // For example:
            // reservation.Property1 = newValue1;
            // reservation.Property2 = newValue2;

            // Save the changes to the database using your DataMapper or other data access logic

            Close();
        }

        private void DeleteReservation_Click(object sender, RoutedEventArgs e)
        {
            // Perform the necessary deletion logic
            // For example:
            // Delete the reservation from the database using your DataMapper or other data access logic

            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
