using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace KlimaKontrol
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Main : IExternalCommand
    {
        static AddInId AddInId = new AddInId(new Guid("B41EE846-761B-40C8-9C33-60C73F1F9C42"));
       
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uIDocument = uiapp.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uIDocument.Document;


            

            UserControl1 window = new UserControl1();
            SettingsControl settingsControl = new SettingsControl();
            SettingsViewModel settingsViewModel = new SettingsViewModel();
            MainViewModel mainViewModel = new MainViewModel(doc, window, settingsControl,settingsViewModel);

            window.DataContext = mainViewModel;
            settingsControl.DataContext = settingsViewModel;
            window.ShowDialog();
            

            return Result.Succeeded;
            
        }
    }
}
