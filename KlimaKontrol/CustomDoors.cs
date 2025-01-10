using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Controls;
using System;

namespace KlimaKontrol
{
    public class CustomDoors
    {
        public Document Document { get; set; }
        public string Name { get; set; }
        public Element WindowElement { get; set; }
        public ElementId WindowId { get; set; }
        public ElementId RoomId { get; set; }
        public ElementId RoomTo { get; set; }
        public ElementId RoomFrom { get; set; }
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
        

        public CustomDoors (Document doc, Element helement, ElementId helementId, ElementId roomId, City preparedCity, double insideTemp)
        {
            Document = doc;
            Name = helement.Name;
            WindowId = helementId;
            WindowElement = helement;
            RoomId = roomId;
            City = preparedCity;
            try
            {
               // RoomId = (WindowElement as FamilyInstance).ToRoom.Id;
                RoomTo = (WindowElement as FamilyInstance).ToRoom.Id;
            }
            catch
            {
               // RoomId = (WindowElement as FamilyInstance).FromRoom.Id;
                RoomFrom = (WindowElement as FamilyInstance).FromRoom.Id;
            }

            if (RoomFrom == null || RoomTo == null)
            {
                TempOutside = City.Min5Day_092;
            }
           

            RoomName = doc.GetElement(RoomId).Name;
            City = preparedCity;
            TempInside = insideTemp;
            //TempOutside = City.Min5Day_092;

            string size = WindowElement.Name;
            size = size.Replace("(h)", "");
            string[] dimensions = size.Split('х'); // Разделяем строку по "х"
            double width = 0;
            double height = 0;
            
                try
                {
                    width = double.Parse(dimensions[0])/1000; // Первый элемент — ширина
                    height = double.Parse(dimensions[1])/1000; // Второй элемент — высота
                }
                catch
                {
                    try
                    {
                        width = Convert.ToDouble(WindowElement.LookupParameter("Ширина").AsValueString()) / 1000;
                        height = Convert.ToDouble(WindowElement.LookupParameter("Высота").AsValueString()) / 1000;
                    }
                    catch
                    { }
                   
                }

                Width = width;
                Height = height;
                /*Width = Convert.ToDouble(WindowElement.get_Parameter(BuiltInParameter.GENERIC_WIDTH).AsValueString()) / 1000;
                Height = Convert.ToDouble(WindowElement.get_Parameter(BuiltInParameter.GENERIC_HEIGHT).AsValueString()) / 1000;*/
                Flache = Width * Height;
          
            
        }
    }
}