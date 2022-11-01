using NGSIBaseModel.Models;

namespace NGSIBaseModel.Test.TestModels;

public class Accelerometer : NgsiBaseModel
{
    public string id { get; set; }
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }

    public string t { get; set; }
}