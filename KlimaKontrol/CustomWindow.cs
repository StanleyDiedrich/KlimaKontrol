using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Windows.Controls;

namespace KlimaKontrol
{
   public  class CustomWindow
    {
        public Document Document { get; set; }
        public string Name { get; set; }
        public Element WindowElement { get; set; }
        public ElementId WindowId { get; set; }
        public ElementId RoomId { get; set; }
        public string RoomName { get; set; }
        public City City { get; set; }
        public ElementId Level { get; set; }
        public string LvlName { get; set; }
        public double Koeffizient { get; set; } = 1;
        public double Resistance { get; set; }
        public double Flache { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double TempInside { get; set; }
        public double TempOutside { get; set; }
        public double Qbasis { get; set; }
       

        public CustomWindow (Document doc, Element helement, ElementId helementId,  City preparedCity, double insideTemp)
        {
            Document = doc;
            Name = helement.Name;
            WindowId = helementId;
            WindowElement = helement;
            try
            {
                RoomId = (WindowElement as FamilyInstance).ToRoom.Id;
            }
            catch
            {
                RoomId = (WindowElement as FamilyInstance).FromRoom.Id;
            }
            
            RoomName = doc.GetElement(RoomId).Name;
            City = preparedCity;
            TempInside = insideTemp;
            TempOutside = City.Min5Day_092;

           

            Width = Convert.ToDouble((WindowElement as FamilyInstance).LookupParameter("Облицовка_высота").AsValueString()) / 1000;
            Height = Convert.ToDouble((WindowElement as FamilyInstance).LookupParameter("Облицовка_ширина").AsValueString()) / 1000;
            Flache = Width*Height;
        }

    }
}