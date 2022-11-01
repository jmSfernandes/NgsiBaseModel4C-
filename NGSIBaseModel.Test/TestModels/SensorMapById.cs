using System.Collections.Generic;
using NGSIBaseModel.Models;
using NGSIBaseModel.Models.Attributes;

namespace NGSIBaseModel.Test.TestModels;

public class SensorMapById : NgsiModelObject
{
    public string id { get; set; }
    public string model { get; set; }

    public float accuracy { get; set; }

    [NGSIMapIds] public List<Accelerometer> accelerometerList { get; set; }

    public string timestamp { get; set; }
}