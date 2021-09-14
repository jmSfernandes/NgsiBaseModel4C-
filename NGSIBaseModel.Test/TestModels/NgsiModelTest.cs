using NGSIBaseModel.Models;

namespace Tests
{
    public class NgsiModelTest : NgsiBaseModel
    {
        [NGSIIgnore] public int _id { get; set; }
        public string id { get; set; }

        [NGSIJObject] public string gpslocation { get; set; }

        public string location { get; set; }
        public double distanceHome { get; set; }
        [NGSIJArray] public string accelerometer { get; set; }
    }
}