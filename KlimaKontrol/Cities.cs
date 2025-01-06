using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace KlimaKontrol
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Area { get; set; }
        public string Town { get; set; }
        public double Min5Day_092 { get; set; }
        public double Min5Day_098 { get;set; }
        public double MinAbs_092 { get; set; }
        public double MinAbs_098 { get; set; }
        public double MinAbs_094 { get; set; }
        public double MinAbs { get; set; }
        public double MinAmpl { get; set; }
        public double AverTempHP { get; set; }
        public double Days_0 { get; set; }
        public double Temp_0 { get; set; }
        public double Days_8 { get; set; }
        public double Temp_8 { get; set; }

        public double Days_10 { get; set; }
        public double Temp_10 { get; set; }
        public double AverHumidify { get; set;}
        public double AverHumidify_15 { get; set;}
        public double RainMM { get; set;}
        public string WindDirection { get; set; }
        public double WindVelocityMax { get; set; }
        public double WindVelocityAver { get; set; }
        private static int _nextId = 1;
        public City( string town, double min5Day_092, double min5Day_098, double minAbs_092,
            double minAbs_098,  double minAbs, double minAmpl, double averTempHP, double days_0,
            double temp_0, double days_8, double temp_8, double days_10, double temp_10, double averHumidify,
            double averHumidify_15, double rainMM, string windDirection, double windVelocityMax, double windVelocityAver)
        {
            Id = _nextId;
            Town = town;
            
            Min5Day_092 = min5Day_092;
            Min5Day_098 = min5Day_098;
            MinAbs_092 = minAbs_092;
            MinAbs_098 = minAbs_098;
            
            MinAbs = minAbs;
            MinAmpl = minAmpl;
            AverTempHP = averTempHP;
            Days_0 = days_0;
            Temp_0 = temp_0;
            Days_8 = days_8;
            Temp_8 = temp_8;
            Days_10 = days_10;
            Temp_10 = temp_10;
            AverHumidify = averHumidify;
            AverHumidify_15 = averHumidify_15;
            RainMM = rainMM;
            WindDirection = windDirection;
            WindVelocityMax = windVelocityMax;
            WindVelocityAver = windVelocityAver;
            _nextId++;
        }
        public City (string area)
        {
            Id = _nextId;
            Area = area;
        }
        public City()
        {
            Id= _nextId;
            _nextId++;
            
        }
    }
}
