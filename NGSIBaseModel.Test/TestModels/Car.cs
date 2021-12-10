using System;
using System.Collections.Generic;
using NGSIBaseModel.Models;

namespace NGSIBaseModel.Test.TestModels
{
    public class Car :NgsiBaseModel
    {
        public string id { get; set; }
        public string model { get; set; }
        public string color { get; set; }

        public string timestamp { get; set; }
        
        [NGSIDateTime] public DateTime timestamp1 { get; set; }

        [NGSIIgnore] public List<string> variations { get; set; }

        [NGSIIgnore] public string ignoreMe { get; set; }
        
        [NGSIEncode] public string encodeMe { get; set; }
    }
}