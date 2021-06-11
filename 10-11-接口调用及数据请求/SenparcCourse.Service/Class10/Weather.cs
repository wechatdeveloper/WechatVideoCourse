using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenparcCourse.Service.Class10
{

    public class Weather
    {
        public string message { get; set; }
        public int status { get; set; }
        public string date { get; set; }
        public string time { get; set; }

        private DateTime AddTime { get; set; }

        public Weather()
        {
            AddTime = DateTime.Now;
        }
    } 

}
