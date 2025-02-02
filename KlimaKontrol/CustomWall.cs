using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlimaKontrol
{
    public class CustomWall
    {
        public enum Abbreviation
        {
            НС,
            ВС
        }

        public string Name { get; set; }

        public Element Element { get; set; }

        public ElementId ElementId { get; set; }

        public ElementId RoomId { get; set; }

        public ElementId Level { get; set; }

        public string LvlName { get; set; }

        public string RoomName { get; set; }

        public string RoomNumber { get; set; }

        public XYZ FacialOrientation { get; set; }

        public string Orientation { get; set; }

        public double Flache { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Koeffizient { get; set; }

        public double Resistance { get; set; }

        public double Length { get; set; }

        public double N_Koef { get; set; } = 1.0;

        public double TempOutside { get; set; }

        public double TempInside { get; set; }

        public bool IsOutside { get; set; }

        public IList<ElementId> HostedElements { get; set; } = new List<ElementId>();

        public List<CustomWindow> Windows { get; set; } = new List<CustomWindow>();

        public List<CustomDoors> Doors { get; set; } = new List<CustomDoors>();

        public double Beta1 { get; set; }

        public double Beta2 { get; set; }

        public double Beta3 { get; set; } = 1.0;

        public double Qbasis { get; set; }

        public double Qtransm { get; set; }

        public double Qinf { get; set; }

        public double Qvent { get; set; }

        public double Qtot { get; set; }

        public BoundarySegment BoundarySegment { get; set; }

        public Abbreviation WallAbbreviation { get; set; }

        public double KOuterWall { get; set; }

        public double KInnerWall { get; set; }

        public double KWindow { get; set; }

        public double KDoor { get; set; }

        public double KGDoor { get; set; }

        public CustomWall(Document doc, ElementId elementId, ElementId roomId, City preparedCity, BoundarySegment boundary, double insideTemp, double kOuterWall, double kInnerWall, double kWindow, double kDoor, double kGDoor)
        {
            ElementId = elementId;
            Element = doc.GetElement(ElementId);
            Koeffizient = 2.1;
            Name = Element.Name;
            Level = Element.LevelId;
            LvlName = doc.GetElement(Level).Name;
            RoomId = roomId;
            RoomName = doc.GetElement(RoomId).Name;
            Element element = doc.GetElement(RoomId);
            RoomNumber = ((SpatialElement)((element is SpatialElement) ? element : null)).Number;
            Element element2 = Element;
            FacialOrientation = ((Wall)((element2 is Wall) ? element2 : null)).Orientation;
            Length = boundary.GetCurve().Length * 304.8 / 1000.0;
            //Width = Convert.ToDouble(Element[(BuiltInParameter)(-1004005)].AsValueString());
            Height = Convert.ToDouble(Element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString()) / 1000.0;
            Flache = Length * Height;
            Beta3 = 1.3;
            Qtot = 0.0;
            TempInside = insideTemp;
            Element element3 = Element;
            HostedElements = ((HostObject)((element3 is HostObject) ? element3 : null)).FindInserts(true, false, false, true);
            KOuterWall = kOuterWall;
            KInnerWall = kInnerWall;
            KWindow = kWindow;
            KDoor = kDoor;
            KGDoor = kGDoor;
        }

        internal void Calculate()
        {
            if (Name.Contains("_НС"))
            {
                Koeffizient = KOuterWall;
            }
            else
            {
                Koeffizient = KInnerWall;
            }
            Qbasis = KOuterWall * Flache * (TempInside - TempOutside) * Beta3;
        }
    }
}
