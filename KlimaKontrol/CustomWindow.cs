using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace KlimaKontrol
{
    


        public class CustomWindow
        {
            public Document Document { get; set; }

            public string Name { get; set; }

            public Element WindowElement { get; set; }

            public ElementId WindowId { get; set; }

            public ElementId RoomId { get; set; }

            public List<CustomRoom> Rooms { get; set; }

            public string RoomName { get; set; }

            public City City { get; set; }

            public ElementId Level { get; set; }

            public string LvlName { get; set; }

            public double Koeffizient { get; set; } = 1.0;

            public double KWindow { get; set; }

            public double Resistance { get; set; }

            public double Flache { get; set; }

            public double Width { get; set; }

            public double Height { get; set; }

            public double Basis3 { get; set; } = 1.3;

            public double TempInside { get; set; }

            public double TempOutside { get; set; }

            public double Qbasis { get; set; }

            public CustomWindow(Document doc, List<CustomRoom> rooms, Element helement, City preparedCity, double kWindow)
            {
                Document = doc;
                Rooms = rooms;
                Name = helement.Name;
                WindowId = helement.Id;
                WindowElement = helement;
                Element windowElement = WindowElement;
                FamilyInstance familyInstance = (FamilyInstance)(object)((windowElement is FamilyInstance) ? windowElement : null);
                if (familyInstance != null)
                {
                    if (familyInstance.ToRoom != null)
                    {
                        RoomId = ((Element)familyInstance.ToRoom).Id;
                    }
                    else if (familyInstance.FromRoom != null)
                    {
                        RoomId = ((Element)familyInstance.FromRoom).Id;
                    }
                }
                RoomName = doc.GetElement(RoomId).Name;
                City = preparedCity;
                foreach (CustomRoom room in rooms)
                {
                    if (room.RoomId == RoomId)
                    {
                        TempInside = room.TempIn;
                    }
                }
                TempOutside = City.Min5Day_092;
                Koeffizient = kWindow;
                Element windowElement2 = WindowElement;
                Width = Convert.ToDouble(((windowElement2 is FamilyInstance) ? windowElement2 : null).LookupParameter("Облицовка_высота").AsValueString()) / 1000.0;
                Element windowElement3 = WindowElement;
                Height = Convert.ToDouble(((windowElement3 is FamilyInstance) ? windowElement3 : null).LookupParameter("Облицовка_ширина").AsValueString()) / 1000.0;
                Flache = Width * Height;
                Qbasis = Koeffizient * Flache * Basis3 * (TempInside - TempOutside);
            }

        }
    }

