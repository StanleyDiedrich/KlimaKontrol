using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KlimaKontrol
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
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

        }
        public void DeleteCntrl(object param)
        {

        }
        public void AcceptCntrl (object param)
        {
            if (param is SettingsControl settingsWindow)
            {
                // Сохраните настройки или выполните другие нужные операции здесь

                // Закрытие окна настроек
                settingsWindow.Close();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SettingsViewModel()
        {
           
            LoadCommand = new RelayCommand(LoadCntrl);
            DeleteCommand = new RelayCommand(DeleteCntrl);
            AcceptCommand = new RelayCommand(AcceptCntrl);
        }
    }
}
