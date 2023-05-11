using DataLayer.Models;
using DesktopApp.Converters;
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
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public delegate void SaveReservation(Reservation reservation);
    public delegate void DeleteReservation(Reservation reservation);

    public partial class DetailWindow : Window
    {
        private Reservation originalReservation;
        public event SaveReservation OnSaveReservation;
        public event SaveReservation OnDeleteReservation;
        public Reservation Reservation {  get; set; }
        public ObservableCollection<Consultant> Consultants { get; set; }
        public ObservableCollection<EventType> EnumValues { get; set; }
        public LocalizationConverter Conv { get; set; }

        public DetailWindow(
            Reservation reservation,
            ObservableCollection<Consultant> consultants,
            ObservableCollection<EventType> reservationsEvent,
            LocalizationConverter conv
        )
        {
            originalReservation = new Reservation(reservation);
            Reservation = reservation;
            Consultants = consultants;
            EnumValues = reservationsEvent;
            Conv = conv;

            DataContext = this;

            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Reservation.Consultant != originalReservation.Consultant
            || Reservation.DateTime != originalReservation.DateTime
            || Reservation.Subject != originalReservation.Subject
            || Reservation.Type != originalReservation.Type) // BECAUSE NOT USING NOTIFY ONCHANGE
                OnSaveReservation?.Invoke(Reservation);
            Close();
        }

        private void DeleteReservation_Click(object sender, RoutedEventArgs e)
        {
            //Reservation.IsCanceled = true;

            OnDeleteReservation?.Invoke(Reservation);
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

        private void DateTime_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
