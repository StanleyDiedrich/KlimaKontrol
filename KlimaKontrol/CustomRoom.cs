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

        public double KInnerWall { get; set; }

        public double KWindow { get; set; }

        public double KDoor { get; set; }

        public double KGDoor { get; set; }

        public List<CustomWall> Walls { get; set; } = new List<CustomWall>();

        public List<CustomWindow> Windows { get; set; } = new List<CustomWindow>();

        public List<CustomDoors> Doors { get; set; } = new List<CustomDoors>();

        public double Qtot { get; set; }

        public CustomRoom(Document document, Element room, City sCity, double koeffizientDoor, double koeffizientGarageDoor, double koeffizientOuterWall, double koeffizientInnerWall, double koeffizientWindow)
        {
            //IL_01bd: Unknown result type (might be due to invalid IL or missing references)
            //IL_01c3: Expected O, but got Unknown
            Document = document;
            RoomElement = room;
            RoomId = room.Id;
            Name = room.Name;
            SCity = sCity;
            TempOut = SCity.Min5Day_092;
            KOuterWall = koeffizientOuterWall;
            KInnerWall = koeffizientInnerWall;
            KWindow = koeffizientWindow;
            KDoor = koeffizientDoor;
            KGDoor = koeffizientGarageDoor;
            switch (RoomType = room.LookupParameter("ADSK_Тип квартиры").AsString())
            {
                case "Кухня":
                    TempIn = 19.0;
                    break;
                case "Бытовое помещение":
                    TempIn = 20.0;
                    break;
                case "Бойлерная":
                    TempIn = 16.0;
                    break;
                case "Гараж":
                    TempIn = 16.0;
                    break;
                case "Жилая комната":
                    TempIn = 20.0;
                    break;
                case "Коридор":
                    TempIn = 20.0;
                    break;
                case "Санузел":
                    TempIn = 25.0;
                    break;
            }
            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
            Element roomElement = RoomElement;
            IList<IList<BoundarySegment>> spatialElements = ((SpatialElement)((roomElement is SpatialElement) ? roomElement : null)).GetBoundarySegments(options);
            List<SpatialElement> boundaries = new List<SpatialElement>();
            foreach (IList<BoundarySegment> spatialElement in spatialElements)
            {
                foreach (BoundarySegment boundary in spatialElement)
                {
                    if (boundary == null)
                    {
                        continue;
                    }
                    Element element = Document.GetElement(boundary.ElementId);
                    Wall wall = (Wall)(object)((element is Wall) ? element : null);
                    if (wall != null)
                    {
                        ElementId wallId = boundary.ElementId;
                        if (RoomId != (ElementId)null)
                        {
                            CustomWall customWall = new CustomWall(Document, wallId, RoomId, SCity, boundary, TempIn, KOuterWall, KInnerWall, KWindow, KDoor, KGDoor);
                            Walls.Add(customWall);
                        }
                    }
                }
            }
        }
    }
}
