using NGSIBaseModel.Models;

namespace NGSIBaseModel.Test.TestModels
{
    public class SleepSegment:NgsiBaseModel
    {
        //[PrimaryKey]
        //[AutoIncrement]
        [NGSIIgnore]
        public int _id { get; set; }

        //[Ignore]
        public string id { get; set; }
        
        
        public string status { get; set; }
        public string start{ get; set; }
        public string end{ get; set; }
        public string description{ get; set; }
        public long duration{ get; set; }
        public string timestamp{ get; set; }

        public SleepSegment()
        {
        }

        public SleepSegment(string status, string start, string end, string description, long duration, string timestamp)
        {
            
            this.status = status;
            this.start = start;
            this.end = end;
            this.description = description;
            this.duration = duration;
            this.timestamp = timestamp;
        }
        
    }
}