using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlimaKontrol
{
    internal class CustomFloor
    {
        public Document Document { get; set; }

        public ElementId RoomId { get; set; }

        public Element RoomElement { get; set; }

        public City SCity { get; set; }

        public string Name { get; set; }

        public ElementId Lvl { get; set; }

        public string LvlName { get; set; }

        public string RoomType { get; set; }

        public double TempIn { get; set; }

        public double TempOut { get; set; }

        public double Flache { get; set; }

        public double Qbasis { get; set; }

        public double Basis3 { get; set; } = 1.3;

        public double Koeffizient { get; set; }

        public CustomFloor(Document document, CustomRoom room, City sCity, double koeffizientFloor)
        {
            Document = document;
            RoomId = room.RoomId;
            Lvl = document.GetElement(room.RoomId).LevelId;
            LvlName = document.GetElement(Lvl).Name;
            Name = room.Name;
            SCity = sCity;
            TempOut = SCity.Min5Day_092;
            TempIn = room.TempIn;
            Koeffizient = koeffizientFloor;
            Flache = Convert.ToDouble(room.RoomElement.get_Parameter(BuiltInParameter.ROOM_AREA).AsValueString());
            Qbasis = Koeffizient * Flache * Basis3 * (TempIn - TempOut);
        }
    }
}
