using NGSIBaseModel.Models;

namespace NGSIBaseModel.Test.TestModels;

public class NgsiModelObject : NgsiBaseModel
{
    [NGSIIgnore] public int _id { get; set; }
    public string id { get; set; }

    public string location { get; set; }
    public float distanceHome { get; set; }
    public Accelerometer accelerometer { get; set; }
}

public class NgsiModelObjectById : NgsiBaseModel
{
    [NGSIIgnore] public int _id { get; set; }
    public string id { get; set; }
    
    public string location { get; set; }
    public float distanceHome { get; set; }
    [NGSIMapIds] public Accelerometer accelerometer { get; set; }
}