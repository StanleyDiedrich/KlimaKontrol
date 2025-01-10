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
using OfficeOpenXml.Drawing.Chart;
using System.IO;

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
                CollectAllRooms(Document, SelectedLink,SelectedCity);
            }
            else
            {
                TaskDialog.Show("Ошибка", "Необходимо заполнить поля");
            }
        }

        [Obsolete]
        private void CollectAllRooms(Autodesk.Revit.DB.Document document, Element selectedLink, City preparedCity)
        {
            var activedocument = (document.GetElement(selectedLink.Id) as RevitLinkInstance).GetLinkDocument();
            FilteredElementCollector linkedFilter = new FilteredElementCollector(activedocument);
            var selectedRooms = linkedFilter.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToList();
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            List<CustomWall> walls = new List<CustomWall>();
            HashSet<(ElementId wallId, ElementId roomId)> uniquePairs = new HashSet<(ElementId, ElementId)>();
            List<CustomRoom> customrooms = new List<CustomRoom>();
            foreach (var room in selectedRooms)
            {
                
                CustomRoom customroom = new CustomRoom(activedocument, room, SelectedCity);
                customrooms.Add(customroom);
            }
            for (int i = 0; i < customrooms.Count; i++)
            {
                for (int j = 0; j < customrooms.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    else
                    {
                        for (int k = 0; k < customrooms[i].Walls.Count; k++)
                        {
                            for (int l = 0; l < customrooms[j].Walls.Count; l++)
                            {
                                if (customrooms[i].Walls[k].Name.Contains("НС") )
                                {
                                    customrooms[i].Walls[k].TempOutside = SelectedCity.Min5Day_092;
                                   
                                }
                                if (customrooms[i].Walls[k].ElementId == customrooms[j].Walls[l].ElementId)
                                {
                                    customrooms[i].Walls[k].TempOutside = customrooms[j].Walls[l].TempInside;
                                }
                            }
                        }
                    }
                }
            }

            string a = "ElementId;Имя комнаты;Длина;Площадь;Твнутри;Тснаружи;Qтеплопотери" + "\n";
            foreach (var room in customrooms)
            {
                foreach (var wall in room.Walls)
                {
                    a += $"{wall.ElementId};{room.Name};{wall.Length};{wall.Flache};{wall.TempInside};{wall.TempOutside};{wall.Qbasis}\n";
                }

                /*o.Qbasis = o.Koeffizient * (o.TempInside - o.TempOutside) * o.Flache * o.Beta3;
                a += $"{o.ElementId};{o.RoomName};{o.Length};{o.Flache};{o.TempInside};{o.TempOutside};{o.Qbasis};{o.Qtot}\n";*/
            }

            SaveFile(a);


        }






















        public void SaveFile(string content) // спрятали функцию сохранения 
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Save CSV File";
            //saveFileDialog.FileName = Collection.First().Elements.First().SystemName + ".csv";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.Write(content);
                    }

                    Console.WriteLine("CSV file saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving CSV file: " + ex.Message);
                }
            }
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