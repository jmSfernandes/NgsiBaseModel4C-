using System.Collections.Generic;
using NGSIBaseModel.Models;

namespace NGSIBaseModel.Test.TestModels
{
    public class Sensor: NgsiBaseModel
    {
        public string id { get; set; }
        public string model { get; set; }
        
        public float accuracy { get; set; }
        public List<Accelerometer> accelerometerList { get; set; }

        public string timestamp { get; set; }
    }
}