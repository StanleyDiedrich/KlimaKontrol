using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlimaKontrol
{
    public class CustomRoom
    {
        public Document Document { get; set; }
        public ElementId RoomId { get; set; }
        public Element RoomElement { get; set; }
        public City SCity { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public double TempIn { get; set; }
        public double TempOut { get; set; }

        public double KOuterWall { get; set; }
        public double KWindow { get; set; }
        public double KDoor { get; set; }
        public double KGDoor { get; set; }
        public List<CustomWall> Walls { get; set; } = new List<CustomWall>();
        public List<CustomWindow> Windows { get; set; } = new List<CustomWindow>();

        public List<CustomDoors> Doors { get; set; } = new List<CustomDoors>();
        public double Qtot { get; set; }
       
        public CustomRoom(Document document, Element room, City sCity, double koeffizientDoor,double koeffizientGarageDoor,double koeffizientOuterWall, double koeffizientWindow)
        {
            Document = document;
            RoomElement = room;
            RoomId = room.Id;
            Name = room.Name;
            SCity = sCity;
            TempOut = SCity.Min5Day_092;
            KOuterWall = koeffizientOuterWall;
            KWindow = koeffizientWindow;
            KDoor = koeffizientDoor;
            KGDoor = koeffizientGarageDoor;

            string roomtype = room.LookupParameter("ADSK_Тип квартиры").AsString();
            RoomType = roomtype;
            if (roomtype == "Кухня")
            {
                TempIn = 19;
            }
            else if (roomtype == "Бытовое помещение")
            {
                TempIn = 20;
            }
            else if (roomtype == "Бойлерная")
            {
                TempIn = 16;
            }
            else if (roomtype == "Гараж")
            {
                TempIn = 16;
            }
            else if (roomtype == "Жилая комната")
            {
                TempIn = 20;
            }
            else if (roomtype == "Коридор")
            {
                TempIn = 20;
            }
            else if (roomtype == "С/у")
            {
                TempIn = 25;
            }

            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            var spatialElements = (RoomElement as SpatialElement).GetBoundarySegments(options);
            List<SpatialElement> boundaries = new List<SpatialElement>();
           
            foreach (var spatialElement in spatialElements)
            {
                foreach (var boundary in spatialElement)
                {
                    if (boundary != null)
                    {
                        if (Document.GetElement(boundary.ElementId) is Wall wall)
                        {
                            ElementId wallId = boundary.ElementId;
                            //Document doc, ElementId elementId, ElementId roomId, City preparedCity, BoundarySegment boundary,double insideTemp
                            if (RoomId !=null)
                            {
                                CustomWall customWall = new CustomWall(Document, wallId, RoomId, SCity, boundary, TempIn, KOuterWall, KWindow,KDoor,KGDoor);
                                Windows = customWall.Windows;
                                Doors = customWall.Doors;
                                Walls.Add(customWall);
                            }
                            
                        }
                    }
                }
            }




        }


        public double WallCalculation()
        {
            double res = 0;
            foreach (var wall in Walls )
            {
                res = 1.3 * wall.Koeffizient * wall.Flache * (wall.TempInside - wall.TempOutside);
                wall.Qbasis = res;
            }
            return res;
        }
        public double WindowCalculation()
        {
            double res = 0;
            foreach (var window in Windows)
            {
                res = 1.3*window.Koeffizient*window.Flache*(window.TempInside - window.TempOutside);
                window.Qbasis = res;
            }

            return res;
        }
        public double DoorCalculation ()
        {
            double res = 0;
            foreach (var door in Doors)
            {
                res =1.3*door.Koeffizient*door.Flache *(door.TempInside - door.TempOutside);
                door.Qbasis = res;
            }
            return res;
        }

        public double RoomCalculation()
        {
            double result = 0;
            
            foreach (var wall in Walls)
            {
                result += wall.Qbasis;

            }
            foreach (var window in Windows)
            {
                result += window.Qbasis;
            }
            foreach (var door in Doors)
            {
                result += door.Qbasis;
            }
            Qtot = result;

            return Qtot;
        }
    }
}
