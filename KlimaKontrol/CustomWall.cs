using Autodesk.Revit.DB;
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

        public ElementId RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomNumber { get; set; }
        public XYZ FacialOrientation { get; set; }
        public string Orientation { get; set; }
        public double Flache {  get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public bool IsOutside {  get; set; }
        public List<Element> HostedElements { get; set; } = new List<Element>();

    }
}
