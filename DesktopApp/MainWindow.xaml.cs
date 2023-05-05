using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
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
//using System.Windows.Shapes;
using DataLayer;
using DataLayer.Models;


namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SqlConnector Connection { get; set; }
        public DataMapper<Reservation> Mapper { get; set; }
        public ObservableCollection<Consultant> Consultants { get; set; } 
        public ObservableCollection<Reservation> Reservations { get; set; }
        public ObservableCollection<EventType> ReservationsEvents { get; set; }
        public Reservation SelectedReservation { get; set; }
        public EventType SelectedEnum { get; set; }

        private void ReservationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e, DataMapper<Reservation> mapper)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                mapper.Insert(Reservations[^1]);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Reservation item in e.OldItems)
                {
                    mapper.Delete(item.Id);
                }
            }
        }

        private bool _showCanceled;
        public bool ShowCanceled
        {
            get { return _showCanceled; }
            set
            {
                _showCanceled = value;
                ReservationsView.Refresh();
            }
        }

        public ICollectionView ReservationsView { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Connection = new SqlConnector(@"Data Source=..\..\..\..\Data\psycho.db");
            Mapper = new DataMapper<Reservation>(Connection);
            var consMapper = new DataMapper<Consultant>(Connection);

            Reservations = new ObservableCollection<Reservation>(Mapper.DomainObject);
            ReservationsEvents = new ObservableCollection<EventType>();
            foreach (EventType enumValue in Enum.GetValues(typeof(EventType)))
            {
                ReservationsEvents.Add(enumValue);
            }
            Consultants = new ObservableCollection<Consultant>(consMapper.DomainObject);

            ReservationsView = CollectionViewSource.GetDefaultView(Reservations);

            dataGrid.ItemsSource = Reservations;

            Reservations.CollectionChanged +=
                (object sender, NotifyCollectionChangedEventArgs e) => ReservationsCollectionChanged(sender, e, Mapper);

            ReservationsView.Filter = FilterReservation;

            //// EXAMPLES
            //Reservations.Add(reservationsDataMapper.DomainObject[1]);
            //Reservations.RemoveAt(0);

            DataContext = this;
        }

        private void AddCustomer(object sender, RoutedEventArgs e)
        {
            //CustomerWindow w = new CustomerWindow();
            ////w.Show();
            //w.OnSaveCustomer += x => this.Customers.Add(x);
            //w.ShowDialog();
            ////this.Customers.Add(w.Customer);
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                DetailWindow detailWindow = new DetailWindow(SelectedReservation);
                detailWindow.ShowDialog();

                // Refresh the reservations after editing or canceling
                // Reservations = dataLayer.GetReservations();
            }
        }

        private void UpdateReservation_Click(object sender, RoutedEventArgs e)
        {
            Reservation current = (Reservation) dataGrid.SelectedValue;
            Console.WriteLine(current.Patient.Name);
            if (SelectedReservation != null)
            {
                Console.WriteLine(SelectedReservation.Patient.Name);
                //Mapper.Update(Mapper.DomainObject.IndexOf(SelectedReservation), SelectedReservation, SelectedReservation.Id);
                //Mapper.Save();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Selection chg");
            Reservation current = (Reservation) dataGrid.SelectedItem;

            if (current != null)
            {
                Console.WriteLine(current.Patient.Name);
            }
        }

        private void StornoReservation_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(SelectedReservation.Patient.Name);
            if (SelectedReservation != null)
            {
                Console.WriteLine(SelectedReservation.Patient.Name);
                Mapper.Update(Mapper.DomainObject.IndexOf(SelectedReservation), SelectedReservation, SelectedReservation.Id);
                Mapper.Save();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private bool FilterReservation(object item)
        {
            Reservation reservation = item as Reservation;
            if (reservation != null)
            {
                return ShowCanceled ? reservation.IsCanceled : !reservation.IsCanceled; // Show all reservations
            }
            return false;
        }
    }
}
