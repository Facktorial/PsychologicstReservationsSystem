using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using DataLayer.Models;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Reservation> Reservations { get; set; }
        public Reservation SelectedReservation { get; set; }

        public MainWindow()
        {
            InitializeComponent();



            Reservations = new ObservableCollection<Reservation>();
            //Reservations = dataLayer.GetReservations();
            DataContext = this;
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                //DetailWindow detailWindow = new DetailWindow(SelectedReservation);
                //detailWindow.ShowDialog();

                // Refresh the reservations after editing or canceling
                // Reservations = dataLayer.GetReservations();
            }
        }
    }
}
