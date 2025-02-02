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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KlimaKontrol
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private Autodesk.Revit.DB.Document document;

        private UserControl1 window;

        private SettingsControl settings;

        private SettingsViewModel settingsViewModel;

        public Autodesk.Revit.DB.Document Document
        {
            get
            {
                return document;
            }
            set
            {
                document = value;
                OnPropertyChanged("Document");
            }
        }

        public UserControl1 Window
        {
            get
            {
                return window;
            }
            set
            {
                window = value;
                OnPropertyChanged("Window");
            }
        }

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
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        private ObservableCollection<PreparedCity> selectedCities { get; set; }

        public ObservableCollection<PreparedCity> SelectedCities
        {
            get
            {
                return selectedCities;
            }
            set
            {
                selectedCities = value;
                OnPropertyChanged("SelectedCities");
            }
        }

        private ObservableCollection<Areas> areas { get; set; }

        public ObservableCollection<Areas> Areas
        {
            get
            {
                return areas;
            }
            set
            {
                areas = value;
                OnPropertyChanged("Areas");
            }
        }

        private Areas selectedArea { get; set; }

        public Areas SelectedArea
        {
            get
            {
                return selectedArea;
            }
            set
            {
                selectedArea = value;
                OnPropertyChanged("SelectedArea");
                UpdatePreparedCity();
            }
        }

        private City selectedCity { get; set; }

        public City SelectedCity
        {
            get
            {
                return selectedCity;
            }
            set
            {
                selectedCity = value;
                OnPropertyChanged("SelectedCity");
            }
        }

        private double underground { get; set; }

        public double Underground
        {
            get
            {
                return underground;
            }
            set
            {
                underground = value;
                OnPropertyChanged("Underground");
            }
        }

        private double koeffizientOuterWall { get; set; }

        public double KoeffizientOuterWall
        {
            get
            {
                return koeffizientOuterWall;
            }
            set
            {
                koeffizientOuterWall = value;
                OnPropertyChanged("KoeffizientOuterWall");
            }
        }

        private double koeffizientInnerWall { get; set; }

        public double KoeffizientInnerWall
        {
            get
            {
                return koeffizientInnerWall;
            }
            set
            {
                koeffizientInnerWall = value;
                OnPropertyChanged("KoeffizientInnerWall");
            }
        }

        private double koeffizientWindow { get; set; }

        public double KoeffizientWindow
        {
            get
            {
                return koeffizientWindow;
            }
            set
            {
                koeffizientWindow = value;
                OnPropertyChanged("KoeffizientWindow");
            }
        }

        private double koeffizientDoor { get; set; }

        public double KoeffizientOuterDoor
        {
            get
            {
                return koeffizientDoor;
            }
            set
            {
                koeffizientDoor = value;
                OnPropertyChanged("KoeffizientOuterDoor");
            }
        }

        private double koeffizientGarageDoor { get; set; }

        public double KoeffizientGarageDoor
        {
            get
            {
                return koeffizientGarageDoor;
            }
            set
            {
                koeffizientGarageDoor = value;
                OnPropertyChanged("KoeffizientGarageDoor");
            }
        }

        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return settingsViewModel;
            }
            set
            {
                settingsViewModel = value;
                OnPropertyChanged("SettingsViewModel");
            }
        }

        public ICommand SettingsCommand { get; }

        public ICommand StartCommand { get; }

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
                OnPropertyChanged("PreparedCity");
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
                OnPropertyChanged("RevitLinkInstances");
            }
        }

        private Element selectedLink { get; set; }

        public Element SelectedLink
        {
            get
            {
                return selectedLink;
            }
            set
            {
                selectedLink = value;
                OnPropertyChanged("SelectedLink");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdatePreparedCity()
        {
            PreparedCity.Clear();
            foreach (City city in Cities)
            {
                if (city.Area == SelectedArea?.AreaName && city.Min5Day_092 != 0.0)
                {
                    PreparedCity.Add(city);
                }
            }
        }

        public void SettingCmd(object param)
        {
            if (SettingsCntrl == null)
            {
                SettingsCntrl = new SettingsControl();
                SettingsCntrl.Show();
            }
            SettingsCntrl.ShowDialog();
        }

        public void StartCmd(object param)
        {
            //IL_007a: Unknown result type (might be due to invalid IL or missing references)
            if (SelectedLink != null && SelectedCity != null && SelectedArea != null)
            {
                (List<CustomRoom>, List<CustomWindow>, List<CustomDoors>, List<CustomFloor>) tuple = CreateRooms(Document, SelectedLink, SelectedCity);
                List<CustomRoom> rooms = tuple.Item1;
                List<CustomWindow> window = tuple.Item2;
                List<CustomDoors> doors = tuple.Item3;
                List<CustomFloor> floors = tuple.Item4;
                string content = CreateContent(rooms, window, doors, floors);
                SaveFile(content);
            }
            else
            {
                TaskDialog.Show("Ошибка", "Необходимо заполнить поля");
            }
        }

        private (List<CustomRoom>, List<CustomWindow>, List<CustomDoors>, List<CustomFloor>) CreateRooms(Autodesk.Revit.DB.Document document, Element selectedLink, City selectedCity)
        {
            List<CustomRoom> rooms = new List<CustomRoom>();
            Element element = document.GetElement(selectedLink.Id);
            var activedocument = ((RevitLinkInstance)((element is RevitLinkInstance) ? element : null)).GetLinkDocument();
            FilteredElementCollector linkedFilter = new FilteredElementCollector(activedocument);
            List<Element> selectedRooms = ((IEnumerable<Element>)linkedFilter.OfCategory((BuiltInCategory)(-2000160)).WhereElementIsNotElementType()).ToList();
            FilteredElementCollector floorsFilter = new FilteredElementCollector(activedocument);
            List<Element> selectedZones = ((IEnumerable<Element>)floorsFilter.OfCategory((BuiltInCategory)(-2002000)).WhereElementIsNotElementType()).ToList();
            foreach (Element room in selectedRooms)
            {
                CustomRoom customRoom = new CustomRoom(activedocument, room, selectedCity, KoeffizientOuterDoor, KoeffizientGarageDoor, KoeffizientOuterWall, KoeffizientInnerWall, KoeffizientWindow);
                rooms.Add(customRoom);
            }
            List<CustomWall> walls = new List<CustomWall>();
            foreach (CustomRoom room2 in rooms)
            {
                foreach (CustomWall wall in room2.Walls)
                {
                    walls.Add(wall);
                }
            }
            double koeffizientFloor = 0.0;
            if (selectedZones.Count > 0)
            {
                double zoneGeneral = 0.0;
                double zone1 = 0.0;
                double zone2 = 0.0;
                double zone3 = 0.0;
                double zone4 = 0.0;
                foreach (Element zone in selectedZones)
                {
                    if (zone == null)
                    {
                        continue;
                    }
                    zoneGeneral += Convert.ToDouble(zone.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsValueString());
                    try
                    {
                        if (zone.LookupParameter("ADSK_Группирование").AsString().Equals("Зона1"))
                        {
                            zone1 += Convert.ToDouble(zone.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsValueString());
                        }
                        else if (zone.LookupParameter("ADSK_Группирование").AsString().Equals("Зона2"))
                        {
                            zone2 += Convert.ToDouble(zone.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsValueString());
                        }
                        else if (zone.LookupParameter("ADSK_Группирование").AsString().Equals("Зона3"))
                        {
                            zone3 += Convert.ToDouble(zone.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsValueString());
                        }
                        else if (zone.LookupParameter("ADSK_Группирование").AsString().Equals("Зона4"))
                        {
                            zone4 += Convert.ToDouble(zone.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsValueString());
                        }
                    }
                    catch
                    {
                    }
                }
                double resistFloor = zoneGeneral / (zone1 / 2.1 + zone2 / 3.8 + zone3 / 5.2 + zone4 / 7.7);
                koeffizientFloor = 1.0 / resistFloor;
            }
            List<CustomFloor> floors = new List<CustomFloor>();
            foreach (CustomRoom room3 in rooms)
            {
                CustomFloor customFloor = new CustomFloor(activedocument, room3, selectedCity, koeffizientFloor);
                if (!floors.Any((CustomFloor w) => w.RoomId == customFloor.RoomId))
                {
                    floors.Add(customFloor);
                }
            }
            for (int i = 0; i < walls.Count; i++)
            {
                for (int j = 0; j < walls.Count; j++)
                {
                    if (i != j && walls[i].ElementId == walls[j].ElementId)
                    {
                        double room1tempIn = walls[i].TempInside;
                        double room1tempOut = walls[i].TempOutside;
                        double room2tempIn = walls[j].TempInside;
                        double room2tempOut = walls[j].TempOutside;
                        walls[i].TempInside = room1tempIn;
                        walls[i].TempOutside = room2tempIn;
                        walls[j].TempInside = room2tempIn;
                        walls[j].TempOutside = room1tempIn;
                    }
                }
            }
            foreach (CustomWall wall2 in walls)
            {
                if (wall2.Name.Contains("_НС"))
                {
                    wall2.TempOutside = selectedCity.Min5Day_092;
                }
                wall2.Calculate();
            }
            List<CustomWindow> windows = new List<CustomWindow>();
            List<CustomDoors> doors = new List<CustomDoors>();
            foreach (CustomWall wall3 in walls)
            {
                Element el = activedocument.GetElement(wall3.ElementId);
                IList<ElementId> hostObject = ((HostObject)((el is HostObject) ? el : null)).FindInserts(true, false, false, false);
                foreach (ElementId host in hostObject)
                {
                    Element element2 = activedocument.GetElement(host);
                    if (element2.Category.Id == new ElementId((BuiltInCategory)(-2000014)))
                    {
                        CustomWindow customWindow = new CustomWindow(activedocument, rooms, element2, selectedCity, KoeffizientWindow);
                        if (!windows.Any((CustomWindow w) => w.WindowId == customWindow.WindowId))
                        {
                            windows.Add(customWindow);
                        }
                    }
                    else if (element2.Category.Id == new ElementId((BuiltInCategory)(-2000023)))
                    {
                        CustomDoors customDoor = new CustomDoors(activedocument, rooms, element2, selectedCity, KoeffizientOuterDoor, koeffizientGarageDoor);
                        if (!doors.Any((CustomDoors d) => d.WindowId == customDoor.WindowId))
                        {
                            doors.Add(customDoor);
                        }
                    }
                }
            }
            return (rooms, windows, doors, floors);
        }

        private string CreateContent(List<CustomRoom> rooms, List<CustomWindow> windows, List<CustomDoors> doors, List<CustomFloor> floors)
        {
            string content = "RoomId;RoomName;ElementId;ElementName;LevelName;Length;TempInside;TempOutside;RoomTo;RoomFrom;Koeffizient;Qbasis\n";
            foreach (CustomRoom room in rooms)
            {
                foreach (CustomWall wall in room.Walls)
                {
                    string line = $"{room.RoomId};{room.Name};{wall.ElementId};{wall.Name};{wall.LvlName};{wall.Flache};{wall.TempInside};{wall.TempOutside};;;{wall.Koeffizient};{wall.Qbasis}" + "\n";
                    content += line;
                }
            }
            foreach (CustomWindow window in windows)
            {
                string line2 = $"{window.RoomId};{window.RoomName};{window.WindowId};{window.Name};{window.LvlName};{window.Flache};{window.TempInside};{window.TempOutside};;;{window.Koeffizient};{window.Qbasis}" + "\n";
                content += line2;
            }
            foreach (CustomDoors door in doors)
            {
                string line3 = $"{door.RoomId};{door.RoomName};{door.WindowId};{door.Name};{door.LvlName};{door.Flache};{door.TempInside};{door.TempOutside};{door.RoomTo};{door.RoomFrom};{door.Koeffizient};{door.Qbasis}" + "\n";
                content += line3;
            }
            foreach (CustomFloor floor in floors)
            {
                string line4 = $"{floor.RoomId};{floor.Name};Не определено;Площадь пола;{floor.LvlName};{floor.Flache};{floor.TempIn};{floor.TempOut};;;{floor.Koeffizient};{floor.Qbasis}" + "\n";
                content += line4;
            }
            return content;
        }



        public void SaveFile(string content)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Save CSV File";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
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

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(Autodesk.Revit.DB.Document doc, UserControl1 window, SettingsControl settings, SettingsViewModel settingsViewModel, ObservableCollection<City> cities, List<Element> revitLinkInstances)
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
            foreach (City city in cities)
            {
                if (city.Min5Day_092 == 0.0)
                {
                    Areas area = new Areas(city.Area);
                    Areas.Add(area);
                }
            }
        }
    }
}