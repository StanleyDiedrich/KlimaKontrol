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
using Autodesk.Revit.UI;
using Autodesk.Revit.Creation;

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


        public ICommand StartCommand { get; }
        public void StartCmd(object param)
        {
            if (SelectedLink!=null && SelectedCity!=null && SelectedArea!=null)
            {
                CollectAllRooms(Document, SelectedLink);
            }
            else
            {
                TaskDialog.Show("Ошибка", "Необходимо заполнить поля");
            }
        }

        private void CollectAllRooms(Autodesk.Revit.DB.Document document, Element selectedLink)
        {
            var activedocument = (document.GetElement(selectedLink.Id) as RevitLinkInstance).GetLinkDocument();
            FilteredElementCollector linkedFilter = new FilteredElementCollector(activedocument);
            var selectedRooms = linkedFilter.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToList();
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            List<CustomWall> walls = new List<CustomWall>();
            foreach (var room in selectedRooms)
            {
                
                    var defroom = room as SpatialElement;
                    if (defroom != null)
                    {
                        var spatialElements = defroom.GetBoundarySegments(options);
                        List<SpatialElement> boundaries = new List<SpatialElement>();

                        foreach (var spatialElement in spatialElements)
                        {
                            foreach (var boundary in spatialElement)
                            {
                                if (boundary == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (activedocument.GetElement(boundary.ElementId) is Wall)
                                    {
                                        CustomWall customWall = new CustomWall(activedocument, boundary.ElementId, room.Id);
                                        walls.Add(customWall);
                                    }
                                }
                            }
                            
                        }
                        
                        
                    }

                

               

            }
            string a = "";

            foreach (var o in walls)
            {
                a += $"{o.ElementId};{o.RoomName};\n";
            }

            TaskDialog.Show("Вывод помещений", a);
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

        private List<Element> revitLinkInstances { get; set; } = new List<Element>();
        public List<Element> RevitLinkInstances
        {
            get
            {
                return revitLinkInstances;
            }
            set
            {
                revitLinkInstances = value;
                OnPropertyChanged(nameof(RevitLinkInstances));
            }
        }

        private Element selectedLink {  get; set; }
        public Element SelectedLink
        {
            get
            {
                return selectedLink;
            }
            set
            {
                selectedLink = value;
                OnPropertyChanged(nameof(SelectedLink));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(Autodesk.Revit.DB.Document doc, UserControl1 window,SettingsControl settings, SettingsViewModel settingsViewModel, ObservableCollection<City> cities, List<Element> revitLinkInstances)
        {
            Window = window;
            Document = doc;
            SettingsCntrl = settings;
            StartCommand = new RelayCommand(StartCmd);
            SettingsCommand = new RelayCommand(SettingCmd);
            SettingsViewModel = settingsViewModel;
            Cities = cities;
            Areas = new ObservableCollection<Areas>();
            PreparedCity = new ObservableCollection<City>();
            RevitLinkInstances = revitLinkInstances;
            foreach (var city in cities)
            {
                if (city.Min5Day_092 == 0)
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