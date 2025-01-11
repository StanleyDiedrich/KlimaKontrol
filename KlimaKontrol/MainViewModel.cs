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
        private double koeffizientOuterWall { get; set; }
        public double KoeffizientOuterWall
        {
            get { return koeffizientOuterWall; }
            set
            {
                koeffizientOuterWall = value;
                OnPropertyChanged(nameof(KoeffizientOuterWall));
            }
        }

        private double koeffizientWindow { get; set; }
        public double KoeffizientWindow
        {
            get { return koeffizientWindow; }
            set
            {
                koeffizientWindow = value;
                OnPropertyChanged(nameof(KoeffizientWindow));
            }
        }
        private double koeffizientDoor { get; set; }
        public double KoeffizientOuterDoor
        {
            get { return koeffizientDoor; }
            set
            {
                koeffizientDoor = value;
                OnPropertyChanged(nameof(KoeffizientOuterDoor));
            }
        }

        private double koeffizientGarageDoor { get; set; }
        public double KoeffizientGarageDoor
        {
            get { return koeffizientGarageDoor; }
            set
            {
                koeffizientGarageDoor = value;
                OnPropertyChanged(nameof(KoeffizientGarageDoor));
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
                
                CustomRoom customroom = new CustomRoom(activedocument, room, SelectedCity,KoeffizientOuterDoor,KoeffizientGarageDoor,KoeffizientOuterWall,KoeffizientWindow);
                customrooms.Add(customroom);
            }
            HashSet<ElementId> viewedWalls = new HashSet<ElementId>();

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
                            // Проверка, была ли стена уже просмотрена
                            if (viewedWalls.Contains(customrooms[i].Walls[k].ElementId))
                            {
                                continue; // Пропустить уже просмотренные стены
                            }

                            for (int l = 0; l < customrooms[j].Walls.Count; l++)
                            {
                                if (viewedWalls.Contains(customrooms[j].Walls[l].RoomId))
                                {
                                    continue; // Пропустить уже просмотренные стены
                                }
                                if (customrooms[i].Walls[k].Name.Contains("НС"))
                                {
                                    customrooms[i].Walls[k].TempOutside = SelectedCity.Min5Day_092;
                                    customrooms[i].Walls[k].Koeffizient = customrooms[i].Walls[k].KOuterWall;
                                }

                                if (customrooms[i].Walls[k].ElementId == customrooms[j].Walls[l].ElementId)
                                {
                                    customrooms[i].Walls[k].TempOutside = customrooms[j].Walls[l].TempInside;
                                    customrooms[i].Walls[k].Koeffizient = 2.1;
                                }

                               /* customrooms[i].Walls[k].Qbasis = WallCalculation(customrooms[i].Walls[k]);*/
                                
                            }

                          
                        }
                    }
                  
                }
                
                //customrooms[i].WallCalculation();
                //customrooms[i].WindowCalculation();
                //customrooms[i].DoorCalculation();
                //customrooms[i].RoomCalculation();
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
                        for (int k = 0; k < customrooms[i].Doors.Count; k++)
                        {
                            // Проверка, была ли стена уже просмотрена
                            /*if (viewedWalls.Contains(customrooms[i].Doors[k].ElementId))
                            {
                                continue; // Пропустить уже просмотренные стены
                            }*/

                            for (int l = 0; l < customrooms[j].Doors.Count; l++)
                            {
                                

                                if (customrooms[i].Doors[k].WindowId == customrooms[j].Doors[l].WindowId)
                                {
                                    customrooms[i].Doors[k].TempOutside = customrooms[j].Doors[l].TempInside;
                                }

                                /* customrooms[i].Walls[k].Qbasis = WallCalculation(customrooms[i].Walls[k]);*/

                            }


                        }
                    }

                }
                
            }
            /*customrooms[i].WallCalculation();
            customrooms[i].WindowCalculation();
            customrooms[i].DoorCalculation();
            customrooms[i].RoomCalculation();*/
            RoomCalculation(customrooms);



            string a = "ElementId;Имя семейства;Имя комнаты;Id комнаты;Длина;Площадь;Твнутри;Тснаружи;К,м2/С*Вт;Qтеплопотери;Qобщ" + "\n";
            foreach (var room in customrooms)
            {
                foreach (var wall in room.Walls)
                {
                    a += $"{wall.ElementId};{wall.Name};{room.Name};{room.RoomId};{wall.Length};{wall.Flache};{wall.TempInside};{wall.TempOutside};{wall.Koeffizient};{wall.Qbasis};{room.Qtot}\n";
                }
                foreach (var window in room.Windows)
                {
                    a += $"{window.WindowId};{window.Name};{window.RoomName};{room.RoomId};{window.Width};{window.Flache};{window.TempInside};{window.TempOutside};{window.Koeffizient};{window.Qbasis};{room.Qtot}\n";
                }
                foreach (var door in room.Doors)
                {
                    a += $"{door.WindowId};{door.Name};{door.RoomName};{room.RoomId};{door.Width};{door.Flache};{door.TempInside};{door.TempOutside};{door.Koeffizient};{door.Qbasis};{room.Qtot}\n";
                }

                /*o.Qbasis = o.Koeffizient * (o.TempInside - o.TempOutside) * o.Flache * o.Beta3;
                a += $"{o.ElementId};{o.RoomName};{o.Length};{o.Flache};{o.TempInside};{o.TempOutside};{o.Qbasis};{o.Qtot}\n";*/
            }

            SaveFile(a);


        }


        public static double RoomCalculation(List<CustomRoom> customRooms)
        {
            var customGroups = customRooms.GroupBy(r => r.Name);

            foreach (var customRoomsGroup in customGroups)
            {
                double totalQ = 0; // Переменная для хранения общего значения Q

                foreach (var customRoom in customRoomsGroup)
                {
                    double res = 0;

                    foreach (var wall in customRoom.Walls)
                    {
                        wall.Qbasis = 1.3 * wall.Koeffizient * wall.Flache * (wall.TempInside - wall.TempOutside);
                        res += wall.Qbasis;
                    }

                    foreach (var window in customRoom.Windows)
                    {
                        window.Qbasis = 1.3 * window.Koeffizient * window.Flache * (window.TempInside - window.TempOutside);
                        res += window.Qbasis;
                    }

                    foreach (var door in customRoom.Doors)
                    {
                        door.Qbasis = 1.3 * door.Koeffizient * door.Flache * (door.TempInside - door.TempOutside);
                        res += door.Qbasis;
                    }

                    customRoom.Qtot = res; // Сохраняем результат в текущей комнате
                    totalQ += res; // Добавляем к общему значению
                }
                customRoomsGroup.First().Qtot = totalQ;
                // Вы можете использовать totalQ здесь, если это необходимо для дальнейших расчетов
            }

            return 0; // Возвращаем общее значение Q
        }

        /*public double WallCalculation (CustomWall wall)
        {
            double res = 0;
             res = 1.3 * wall.Koeffizient * wall.Flache * (wall.TempInside - wall.TempOutside);

            return res;
        }
        public double WindowCalculation(CustomRoom room)
        {
            foreach (var window in room.Windows )
            {
                double res = 0;
                res = 1.3 * window.Koeffizient * window.Flache * (window.TempInside - window.TempOutside);
                return res;
            }

            return 0;
           
        }
        

        public double RoomCalculation(CustomRoom customRoom)
        {
            double res = 0;

            foreach (var wall in customRoom.Walls)
            {
                res += wall.Qbasis;

            }
            foreach (var window in customRoom.Windows)
            {
                  
               res += window.Qbasis;
                
            }

            foreach (var door in customRoom.Doors)
            {
                res += door.Qbasis;
            }


            return res; 
        }*/




















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