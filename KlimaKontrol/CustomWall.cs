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
        public double Flache {  get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Koeffizient { get; set; }
        public double Resistance { get; set; }
        public double Length { get; set; }
        public double N_Koef { get; set; } = 1;
        public double TempOutside { get; set; }
        public double TempInside { get; set; }
        public bool IsOutside {  get; set; }
        public IList<ElementId> HostedElements { get; set; } = new List<ElementId>();
        public double Beta1 { get; set; }
        public double Beta2 { get; set; }
        public double Beta3 { get; set; } = 1;
        public double Qbasis {  get; set; }
        public double Qtransm { get; set; }
        public double Qinf { get; set; }
        public double Qvent { get; set; }
        public double Qtot { get; set; }
        public BoundarySegment BoundarySegment { get; set; }
        public enum Abbreviation
        {
            НС,
            ВС
        }

        public Abbreviation WallAbbreviation { get; set; }

        public CustomWall ( Document doc, ElementId elementId, ElementId roomId, City preparedCity, BoundarySegment boundary ,double insideTemp)
        {
           
            ElementId = elementId;
            Element = doc.GetElement(ElementId);
            Koeffizient = 1;
            Name = Element.Name;
            Level = Element.LevelId;
            LvlName = doc.GetElement(Level).Name;
            RoomId = roomId;
            RoomName = doc.GetElement(RoomId).Name;
            RoomNumber = (doc.GetElement(RoomId) as SpatialElement).Number;
            FacialOrientation = (Element as Wall).Orientation;
            Length = boundary.GetCurve().Length; //* 304.8;
            Flache = Convert.ToDouble(Element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsValueString());
            Width = Convert.ToDouble(Element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString());
            Height = Convert.ToDouble(Element.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsValueString());
            Beta3 = 1.3;
            Qtot = 0;
            TempInside = insideTemp;
            HostedElements = (Element as HostObject).FindInserts(true,false,false,true);
            List<Element> doors = new List<Element>();
            List<Element> windows = new List<Element>();
            List<Element> helements = new List<Element>();
            foreach (var hostElement  in HostedElements)
            {
                if (doc.GetElement(hostElement) is FamilyInstance)
                {
                    helements.Add(doc.GetElement(hostElement));   
                }
            }
            foreach (var helement in helements)
            {
                if (helement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Windows)
                {
                    windows.Add(helement);
                }
                else if (helement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors)
                {
                    doors.Add(helement);
                }

            }
            foreach (var window in windows)
            {
                if ((window as FamilyInstance).FromRoom == null || (window as FamilyInstance).ToRoom == null)
                {
                    TempOutside = preparedCity.Min5Day_092;
                }

            }
            
            Qbasis =1.3*Koeffizient*Flache*(TempInside-TempOutside);
            /*Koeffizient = koeffizient;
            Resistance = resistance;
            N_Koef = n_Koef;
            TempOutside = tempOutside;
            TempInside = tempInside;
            IsOutside = isOutside;
            HostedElements = hostedElements;
            Beta1 = beta1;
            Beta2 = beta2;
            
            Qbasis = qbasis;
            Qtransm = qtransm;
            Qinf = qinf;
            Qvent = qvent;
           
            WallAbbreviation = myAbbreviation;*/
        }

       
    }
}
