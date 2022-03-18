using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AverageHeartRate
{
    public class Data
    {
        public IList<HeartRate> data { get; set; }
        public object next_token { get; set; }
    }
}
