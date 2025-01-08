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
            foreach (var room in selectedRooms)
            {
                var insideTemp = 0;
                string roomtype = room.LookupParameter("ADSK_Тип квартиры").AsString();
                if (roomtype == "Кухня")
                {
                    insideTemp = 19;
                }
                else if (roomtype == "Бытовое помещение")
                {
                    insideTemp = 20;
                }
                else if (roomtype == "Бойлерная")
                {
                    insideTemp = 16;
                }
                else if (roomtype == "Гараж")
                {
                    insideTemp = 16;
                }
                else if (roomtype == "Жилая комната")
                {
                    insideTemp = 20;
                }
                else if (roomtype == "Коридор")
                {
                    insideTemp = 20;
                }
                else if (roomtype == "С/у")
                {
                    insideTemp = 25;
                }

                var defroom = room as SpatialElement;
                if (defroom != null)
                {
                    var spatialElements = defroom.GetBoundarySegments(options);
                    List<SpatialElement> boundaries = new List<SpatialElement>();

                    foreach (var spatialElement in spatialElements)
                    {
                        foreach (var boundary in spatialElement)
                        {
                            if (boundary != null)
                            {
                                if (activedocument.GetElement(boundary.ElementId) is Wall wall)
                                {
                                    var points = boundary.GetCurve().Tessellate();
                                    double  p1X = points[0].X;
                                    double p1Y = points[0].Y;
                                    double p1Z = points[0].Z;
                                    double p2X = points[1].X;
                                    double p2Y = points[1].Y;
                                    double p2Z = points[1].Z;
                                    double dx = p2X - p1X;
                                    double dy = p2Y - p1Y;
                                    double dz = p2Z - p1Z;
                                    double length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                                   
                                    CustomWall customWall = new CustomWall(activedocument, boundary.ElementId, room.Id, length,
                                                   preparedCity, insideTemp);
                                    walls.Add(customWall);
                                    /* var pair = (boundary.ElementId, room.Id); // Создаем кортеж

                                     // Проверяем уникальность комбинации wallId и roomId
                                     if (uniquePairs.Add(pair)) // Попытка добавить возвращает false, если уже существует
                                     {
                                         double length = boundary.GetCurve().Length;
                                         CustomWall customWall = new CustomWall(activedocument, boundary.ElementId, room.Id, length,
                                             preparedCity, insideTemp);
                                         walls.Add(customWall);
                                     }*/
                                }
                            }
                        }
                    }
                }

            }

            List<CustomWall> outsideWalls = new List<CustomWall>();
            Dictionary<CustomWall, CustomWall> pairofWalls = new Dictionary<CustomWall, CustomWall>();

            for (int i = 0; i < walls.Count; i++)
            {
                bool isPairFound = false;

                for (int j = 0; j < walls.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (walls[i].ElementId == new ElementId(1546360))
                    {
                        if (walls[j].ElementId == new ElementId(1546360))
                        {
                            if (walls[i].ElementId == walls[j].ElementId)
                            {
                                if (walls[i].RoomId != walls[j].RoomId)
                                {
                                    pairofWalls.Add(walls[i], walls[j]);
                                    isPairFound = true;
                                    break; // Если пара найдена, можно выйти из внутреннего цикла
                                }
                            }
                        }
                        
                        
                    }
                }

                if (!isPairFound && !outsideWalls.Contains(walls[i]))
                {
                    outsideWalls.Add(walls[i]);
                }
            }

            foreach (var pairofwall in pairofWalls)
            {

                CustomWall wall1 = pairofwall.Key;
                CustomWall wall2 = pairofwall.Value;

                if (wall1.ElementId == new ElementId(1546366))
                {
                    wall1 = wall1;
                }
                double temp1 = wall1.TempInside;
                double temp2 = wall2.TempInside;

                if (temp1 > temp2)
                {
                    
                    wall1.TempOutside = temp1;
                    wall2.TempOutside = temp2;

                }
                else if (temp1 < temp2)
                {
                    wall1.TempOutside = temp2;
                    wall2.TempOutside = temp1;
                }
                else if (temp1 == temp2)
                {
                    wall1.TempOutside = temp2;
                    wall2.TempOutside= temp1;

                }


                if (!outsideWalls.Contains(wall1))
                {
                    outsideWalls.Add(wall1);
                }

            }


            var groupedRooms = walls.GroupBy(wall => wall.RoomId);
            foreach (var roomGroup in groupedRooms)
            {
                var roomId = roomGroup.Key;

                // Вычисляем сумму Qbase для текущей группы
                var totalQbase = roomGroup.Sum(wall => wall.Qbasis);

                var selectedwall = roomGroup.Take(1).First();
                selectedwall.Qtot = totalQbase;

            }


                string a = "ElementId;Имя комнаты;Длина;Площадь;Твнутри;Тснаружи;Qтеплопотери;Qcумм" + "\n";
                foreach (var o in outsideWalls)
                {
                    o.Qbasis = o.Koeffizient * (o.TempInside - o.TempOutside) * o.Flache * o.Beta3;
                    a += $"{o.ElementId};{o.RoomName};{o.Length};{o.Flache};{o.TempInside};{o.TempOutside};{o.Qbasis};{o.Qtot}\n";
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