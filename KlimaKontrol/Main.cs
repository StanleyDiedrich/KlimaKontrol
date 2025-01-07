using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;
using Newtonsoft.Json;

using System.IO;
using System.Collections.ObjectModel;

namespace KlimaKontrol
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Main : IExternalCommand
    {
        static AddInId AddInId = new AddInId(new Guid("B41EE846-761B-40C8-9C33-60C73F1F9C42"));

        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();

        public void LoadDefaultJSON()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "KlimaKontrol.cities.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                Cities = JsonConvert.DeserializeObject<ObservableCollection<City>>(json);
            }
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uIDocument = uiapp.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uIDocument.Document;

            if (Cities.Count == 0)
            {
                LoadDefaultJSON();
            }

            FilteredElementCollector filteredModels = new FilteredElementCollector(doc);
            var fmodels = filteredModels.OfClass(typeof(RevitLinkInstance)).ToList();






            UserControl1 window = new UserControl1();
            SettingsControl settingsControl = new SettingsControl();
            SettingsViewModel settingsViewModel = new SettingsViewModel(Cities);  // Используем Cities 
            MainViewModel mainViewModel = new MainViewModel(doc, window, settingsControl, settingsViewModel, Cities, fmodels);

            window.DataContext = mainViewModel;
            settingsControl.DataContext = settingsViewModel;
            window.ShowDialog();

            return Result.Succeeded;
        }
    }
}
