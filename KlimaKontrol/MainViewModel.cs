using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;

namespace KlimaKontrol
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Autodesk.Revit.DB.Document document;
        public Autodesk.Revit.DB.Document Document
        {
            get { return document; }
            set
            {
                document = value;
                OnPropertyChanged("Document");
            }
        }

        private UserControl1 window;
        public UserControl1 Window
        {
            get { return window; }
            set
            {
                window = value;
                OnPropertyChanged("Window");
            }
        }
        private SettingsControl settings;
        public SettingsControl SettingsCntrl
        {
            get
            {
                if (settings == null)
                {
                    settings = new SettingsControl();
                }
                return settings;
            }
            set
            {
                settings = value;
                OnPropertyChanged("SettingsCntrl");
            }
        }

        public ObservableCollection<City> Cities { get; private set; } = new ObservableCollection<City>();



       
        private string filePath { get; set; }
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
        private ObservableCollection<PreparedCity> selectedCities { get; set; }
        public ObservableCollection<PreparedCity> SelectedCities
        {
            get { return selectedCities; }
            set
            {
                selectedCities = value;
                OnPropertyChanged(nameof(SelectedCities));
            }
        }

        private ObservableCollection<Areas> areas { get; set; }
        public ObservableCollection<Areas> Areas
        {
            get { return areas; }
            set
            {
                areas = value;
                OnPropertyChanged(nameof(Areas));
            }
        }
        private Areas selectedArea { get; set; }
        public Areas SelectedArea
        {
            get { return selectedArea; }
            set
            {
                selectedArea = value;
                OnPropertyChanged(nameof(SelectedArea));
                UpdatePreparedCity();
            }
        }
        private void UpdatePreparedCity()
        {
            PreparedCity.Clear(); // очищаем предыдущие значения
            foreach (var city in Cities)
            {
                if (city.Area == SelectedArea?.AreaName && city.Min5Day_092 != 0)
                {
                    PreparedCity.Add(city);
                }
            }
        }
        private City selectedCity { get; set; }
        public City SelectedCity
        {
            get { return selectedCity; }
            set
            {
                selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }
        private double underground { get; set; }
        public double Underground
        {
            get { return underground; }
            set
            {
                underground = value;
                OnPropertyChanged(nameof(Underground));
            }
        }

        private SettingsViewModel settingsViewModel;
        public SettingsViewModel SettingsViewModel
        {
            get { return settingsViewModel; }
            set
            {
                settingsViewModel = value;
                OnPropertyChanged("SettingsViewModel");
            }
        }
        public ICommand SettingsCommand { get; }
        public void SettingCmd(object param)
        {
           if (SettingsCntrl == null)
            {
                SettingsCntrl = new SettingsControl();
                SettingsCntrl.Show();
            }
           SettingsCntrl.ShowDialog();
        }

        private ObservableCollection<City> preparedCity { get; set; } = new ObservableCollection<City>();
        public ObservableCollection<City> PreparedCity
        {
            get
            {
                
                return preparedCity;
            }
            set
            {
                preparedCity = value;
                OnPropertyChanged(nameof(PreparedCity));

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(Autodesk.Revit.DB.Document doc, UserControl1 window,SettingsControl settings, SettingsViewModel settingsViewModel, ObservableCollection<City> cities)
        {
            Window = window;
            Document = doc;
            SettingsCntrl = settings;
            SettingsCommand = new RelayCommand(SettingCmd);
            SettingsViewModel = settingsViewModel;
            Cities = cities;
            Areas = new ObservableCollection<Areas>();
            PreparedCity = new ObservableCollection<City>();

            foreach (var city in cities)
            {
                if (city.Min5Day_092==0)
                {
                    Areas area = new Areas(city.Area);
                    Areas.Add(area);
                }
            }

            /*foreach (var city in cities)
            {
                if (city.Area == SelectedArea && city.Min5Day_092!=0)
                {
                    PreparedCity preparedCity = new PreparedCity(city);
                    PreparedCity.Add(preparedCity);
                }
            }*/

        }
    }
}