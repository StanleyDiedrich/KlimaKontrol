using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Controls;
using System;
using System.Collections.Generic;

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

        public double Koeffizient { get; set; } = 1.0;

        public double Resistance { get; set; }

        public double Flache { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Basis3 { get; set; } = 1.3;

        public double TempInside { get; set; }

        public double TempOutside { get; set; }

        public double Qbasis { get; set; }

        public double KDoor { get; set; }

        public double KGDoor { get; set; }

        public CustomDoors(Document doc, List<CustomRoom> rooms, Element helement, City preparedCity, double kDoor, double kGDoor)
        {
            Document = doc;
            Name = helement.Name;
            WindowId = helement.Id;
            WindowElement = helement;
            KDoor = kDoor;
            KGDoor = kGDoor;
            City = preparedCity;
            Element windowElement = WindowElement;
            FamilyInstance familyInstance = (FamilyInstance)(object)((windowElement is FamilyInstance) ? windowElement : null);
            if (familyInstance != null)
            {
                if (familyInstance.ToRoom != null)
                {
                    RoomTo = ((Element)familyInstance.ToRoom).Id;
                    RoomId = RoomTo;
                }
                if (familyInstance.FromRoom != null)
                {
                    RoomFrom = ((Element)familyInstance.FromRoom).Id;
                    RoomId = RoomFrom;
                }
            }
            if (RoomFrom == (ElementId)null || RoomTo == (ElementId)null)
            {
                TempOutside = City.Min5Day_092;
                foreach (CustomRoom room in rooms)
                {
                    if (room.RoomId == RoomId)
                    {
                        TempInside = room.TempIn;
                        break;
                    }
                }
            }
            else
            {
                double room1In = 0.0;
                double room2In = 0.0;
                foreach (CustomRoom room2 in rooms)
                {
                    if (room2.RoomId == RoomTo)
                    {
                        room1In = room2.TempIn;
                    }
                }
                foreach (CustomRoom room3 in rooms)
                {
                    if (room3.RoomId == RoomFrom)
                    {
                        room2In = room3.TempIn;
                    }
                }
                TempInside = room1In;
                TempOutside = room2In;
            }
            if (WindowElement.LookupParameter("ADSK_Группирование").AsValueString() == "Ворота")
            {
                Koeffizient = KGDoor;
            }
            else
            {
                Koeffizient = KDoor;
            }
            RoomName = doc.GetElement(RoomId).Name;
            City = preparedCity;
            string size = WindowElement.Name;
            size = size.Replace("(h)", "");
            string[] dimensions = size.Split('х');
            double width = 0.0;
            double height = 0.0;
            try
            {
                width = double.Parse(dimensions[0]) / 1000.0;
                height = double.Parse(dimensions[1]) / 1000.0;
            }
            catch
            {
                try
                {
                    width = Convert.ToDouble(WindowElement.LookupParameter("Ширина").AsValueString()) / 1000.0;
                    height = Convert.ToDouble(WindowElement.LookupParameter("Высота").AsValueString()) / 1000.0;
                }
                catch
                {
                }
            }
            Width = width;
            Height = height;
            Flache = Width * Height;
            Qbasis = Koeffizient * Basis3 * Flache * (TempInside - TempOutside);
        }
    }
}