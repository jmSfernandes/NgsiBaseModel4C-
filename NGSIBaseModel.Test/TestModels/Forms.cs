using NGSIBaseModel.Models;
using NGSIBaseModel.Models.Attributes;

namespace NGSIBaseModel.Test.TestModels;

public abstract class Forms
{
      

    public class AppFinality : NgsiBaseModel
    {
          
        public string id { get; set; }
        [NGSIJObject]public string app1 { get; set; }
        [NGSIJObject]public string app2 { get; set; }
        [NGSIJObject]public string app3 { get; set; }
        [NGSIJObject]public string app4 { get; set; }
        [NGSIJObject] public string app5 { get; set; }
        public string timestamp { get; set; }
           

    }
      
        
}