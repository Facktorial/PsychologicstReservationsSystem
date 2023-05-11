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
using System.Globalization;
//using System.Windows.Shapes;
using DataLayer;
using DataLayer.Models;
using System.Resources;
using DesktopApp.Converters;
using System.Threading;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        async void Periodicer()
        {
            while (true)
            {
                await UpdateReservation();
                Thread.Sleep(5000);
            }
        }

        async Task UpdateReservation()
        {
            Console.WriteLine("[Start] UpdateReservations");

            Dispatcher.Invoke(() =>
            {
                // Retrieve the latest reservations from the database
                Mapper.Fetch();

                if (Mapper.DomainObject.Count == Reservations.Count) { return;  }

                var domainObjectIds = new HashSet<int>(Mapper.DomainObject.Select(x => x.Id));
                var reservationsToRemove = Reservations
                    .Where(x => !domainObjectIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToList();
                var reservationsToAdd = Mapper.DomainObject
                    .Where(x => !Reservations.Any(y => y.Id == x.Id))
                    .ToList();

                // Remove reservations that are not in Mapper.DomainObject
                foreach (var id in reservationsToRemove)
                {
                    var item = Reservations.FirstOrDefault(x => x.Id == id);
                    Reservations.Remove(item);
                }
                // Add reservations that are not in Reservations
                foreach (var res in reservationsToAdd)
                {
                    Reservations.Add(Mapper.DomainObject.FirstOrDefault(x => x != null && x.Id == res.Id));
                }

                Console.WriteLine("reservations: " + Reservations.Count);

                var consMapper = new DataMapper<Consultant>(Connection);
                Consultants = new ObservableCollection<Consultant>(consMapper.DomainObject);
                
                ReservationsView.Refresh();
            });

            Console.WriteLine("[ End ] UpdateReservations");
        }

        public SqlConnector Connection { get; set; }
        public DataMapper<Reservation> Mapper { get; set; }
        public ObservableCollection<Consultant> Consultants { get; set; } 
        public ObservableCollection<Reservation> Reservations { get; set; }
        public ObservableCollection<EventType> ReservationsEvents { get; set; }
        public Reservation SelectedReservation { get; set; }
        public Consultant SelectedConsultant { get; set; }
        public EventType SelectedEnum { get; set; }
        //private readonly ResourceManager resourceManager;
        public Dictionary<string, string?> EnumValues { get; set; }
        LocalizationConverter Translator { get; set; }

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
            var resourceManager = new ResourceManager("DesktopApp.Resources.MyResourceFile", typeof(MainWindow).Assembly);

            EnumValues = new Dictionary<string, string?>();
            foreach (var item in Enum.GetNames(typeof(EventType)))
            {
                EnumValues.Add(item, resourceManager.GetString(item));
            }

            Translator = new LocalizationConverter(EnumValues);

            ReservationsEvents = new ObservableCollection<EventType>();
            foreach (EventType enumValue in Enum.GetValues(typeof(EventType)))
            {
                ReservationsEvents.Add(enumValue);
            }

            Connection = new SqlConnector(@"Data Source=..\..\..\..\Data\psycho.db");

            Mapper = new DataMapper<Reservation>(Connection);

            Reservations = new ObservableCollection<Reservation>(Mapper.DomainObject);

            new Thread(Periodicer).Start();

            InitializeComponent();
            dataGrid.CanUserAddRows = false;
            dataGrid.ItemsSource = Reservations;

            //var consMapper = new DataMapper<Consultant>(Connection);
            //Consultants = new ObservableCollection<Consultant>(consMapper.DomainObject);

            ReservationsView = CollectionViewSource.GetDefaultView(Reservations);

            Reservations.CollectionChanged +=
                (object sender, NotifyCollectionChangedEventArgs e) => ReservationsCollectionChanged(sender, e, Mapper);

            ReservationsView.Filter = FilterReservation;

            //// EXAMPLES
            //Reservations.Add(reservationsDataMapper.DomainObject[1]);
            //Reservations.RemoveAt(0);

            DataContext = this;
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                DetailWindow detailWindow = new DetailWindow(SelectedReservation, Consultants, ReservationsEvents, Translator);
                detailWindow.OnSaveReservation += x =>
                {
                    var item = ReservationsView.Cast<Reservation>().FirstOrDefault(item => item.Id == x.Id);
                    item = x;
                    Mapper.Update(Mapper.DomainObject.IndexOf(x), x, x.Id);
                    Mapper.Save();
                    ReservationsView.Refresh();
                };
                detailWindow.OnDeleteReservation += x =>
                {
                    var item = ReservationsView.Cast<Reservation>().FirstOrDefault(item => item.Id == x.Id);

                    Reservations.Remove(item);
                    //Mapper.Delete(x.Id);
                    Mapper.Save();
                    ReservationsView.Refresh();
                };

                detailWindow.ShowDialog();
            }
        }

        private void ViewPatient_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                PatientWindow patietWindow = new PatientWindow(SelectedReservation.Patient);
                patietWindow.ShowDialog();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void ActivateReservation_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                SelectedReservation.IsCanceled = false;
                Mapper.Update(Mapper.DomainObject.IndexOf(SelectedReservation), SelectedReservation, SelectedReservation.Id);
                Mapper.Save();
                ReservationsView.Refresh();
            }
        }

        private void StornoReservation_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                SelectedReservation.IsCanceled = true;
                Mapper.Update(Mapper.DomainObject.IndexOf(SelectedReservation), SelectedReservation, SelectedReservation.Id);
                Mapper.Save();
                ReservationsView.Refresh();
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
