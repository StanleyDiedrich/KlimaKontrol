using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlimaKontrol
{

    public class PreparedCity
    {
        
        public int Id { get; set; }
        public string Area { get; set; }
        public string Town { get; set; }
        public double Min5Day_092 { get; set; }
        public double Min5Day_098 { get; set; }
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
        public double AverHumidify { get; set; }
        public double AverHumidify_15 { get; set; }
        public double RainMM { get; set; }
        public string WindDirection { get; set; }
        public double WindVelocityMax { get; set; }
        public double WindVelocityAver { get; set; }
        private static int _nextId = 1;
        
      
        public PreparedCity(City city)
        {
            Id = city.Id;
            Town = city.Town;

            Min5Day_092 = city.Min5Day_092;
            Min5Day_098 = city.Min5Day_098;
            MinAbs_092 = city.MinAbs_092;
            MinAbs_098 = city.MinAbs_098;

            MinAbs = city.MinAbs;
            MinAmpl = city.MinAmpl;
            AverTempHP = city.AverTempHP;
            Days_0 = city.Days_0;
            Temp_0 = city.Temp_0;
            Days_8 = city.Days_8;
            Temp_8 = city.Temp_8;
            Days_10 = city.Days_10;
            Temp_10 = city.Temp_10;
            AverHumidify = city.AverHumidify;
            AverHumidify_15 = city.AverHumidify_15;
            RainMM = city.RainMM;
            WindDirection = city.WindDirection;
            WindVelocityMax = city.WindVelocityMax;
            WindVelocityAver = city.WindVelocityAver;
            _nextId++;

        }
    }
}
