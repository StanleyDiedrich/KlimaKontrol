using Autodesk.Revit.UI;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data.Entity;
using System.Collections.ObjectModel;

namespace KlimaKontrol
{
    public class SettingsViewModel : INotifyPropertyChanged
    {


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

        private SettingsViewModel settingsViewModel { get; set; }
        public SettingsViewModel SetViewModel
        { 
            get { return settingsViewModel; }
            set
            {
                settingsViewModel = value;
                OnPropertyChanged(nameof(SetViewModel));
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AcceptCommand { get; }
        AppDbContext appDbContext;
        public void LoadCntrl(object param)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = "Выберите файл для пакетного сохранения Excel документов";
            dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            // Проверяем, выбрал ли пользователь файл
            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
            }
            CreateNewTable();
            //CreateNewTable(FilePath);
        }

        private void CreateNewTable()
        {
            City newCity = new City(); // Создание нового экземпляра
            Cities.Add(newCity); // Добавление нового города в коллекцию
        }
        /*private void CreateNewTable(string filePath)
        {
            List<City> cityList = new List<City>();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["ХП"]; // Первая таблица
                int rowCount = worksheet.Dimension.Rows;


                for (int i = 6; i < rowCount; i++)
                {
                    string town = worksheet.Cells[i, 1].Text;
                    double min5Day_092 = double.Parse(worksheet.Cells[i, 2].Text);
                    double min5Day_098 = double.Parse(worksheet.Cells[i, 3].Text);
                    double minAbs_092 = double.Parse(worksheet.Cells[i, 4].Text);
                    double minAbs_098 = double.Parse(worksheet.Cells[i, 5].Text);
                    double minAbs_094 = double.Parse(worksheet.Cells[i, 6].Text);
                    double minAbs = double.Parse(worksheet.Cells[i, 7].Text);
                    double minAmpl = double.Parse(worksheet.Cells[i, 8].Text);
                    double averTempHP = double.Parse(worksheet.Cells[i, 9].Text);
                    double days_0 = double.Parse(worksheet.Cells[i, 10].Text);
                    double temp_0 = double.Parse(worksheet.Cells[i, 11].Text);
                    double days_8 = double.Parse(worksheet.Cells[i, 12].Text);
                    double temp_8 = double.Parse(worksheet.Cells[i, 13].Text);
                    double days_10 = double.Parse(worksheet.Cells[i, 14].Text);
                    double temp_10 = double.Parse(worksheet.Cells[i, 15].Text);
                    double averHumidify = double.Parse(worksheet.Cells[i, 16].Text);
                    double averHumidify_15 = double.Parse(worksheet.Cells[i, 17].Text);
                    double rainMM = double.Parse(worksheet.Cells[i, 18].Text);
                    string windDirection = worksheet.Cells[i, 19].Text;
                    double windVelocityMax = double.Parse(worksheet.Cells[i, 20].Text);
                    double windVelocityAver = double.Parse(worksheet.Cells[i, 21].Text);

                    City newCity = new City(town, min5Day_092, min5Day_098, minAbs_092, minAbs_098, minAbs_094, minAbs, minAmpl,
                                            averTempHP, days_0, temp_0, days_8, temp_8, days_10, temp_10, averHumidify,
                                            averHumidify_15, rainMM, windDirection, windVelocityMax, windVelocityAver);

                    cityList.Add(newCity);

                }


            }
            
                    
            
        }*/

        public void DeleteCntrl(object param)
        {

        }
        public void AcceptCntrl (object param)
        {
            if (param is SettingsControl settingsWindow)
            {
                // Сохраните настройки или выполните другие нужные операции здесь

                // Закрытие окна настроек
                appDbContext.SaveChanges();
                
                
                
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SettingsViewModel()
        {
            appDbContext = new AppDbContext();
            appDbContext.Cities.Load();
            Cities = new ObservableCollection<City>(appDbContext.Cities.Local.ToList());
            OnPropertyChanged(nameof(Cities));
            LoadCommand = new RelayCommand(LoadCntrl);
            DeleteCommand = new RelayCommand(DeleteCntrl);
            AcceptCommand = new RelayCommand(AcceptCntrl);
        }
    }
}
