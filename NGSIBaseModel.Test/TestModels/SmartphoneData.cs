using NGSIBaseModel.Models;
using NGSIBaseModel.Models.Attributes;

namespace NGSIBaseModel.Test.TestModels;

public class SmartphoneData : NgsiBaseModel
{
    //[PrimaryKey]
    //[AutoIncrement]
    [NGSIIgnore]
    public int _id { get; set; }

    //[Ignore]
    public string id { get; set; }
    public string operating_system { get; set; }
    public string activity { get; set; }

    public string location { get; set; }
    public float distanceHome { get; set; }

    public string timestamp { get; set; }
    public int step_count { get; set; }

    public string gravity { get; set; }
    public string attitude { get; set; }

    public string phone_lock { get; set; }

    public string foregroundApp { get; set; }
    private long time_to_next_alarm { get; set; }

    private int fall_times { get; set; }
    public float lightmax { get; set; }
    public float lightmin { get; set; }
    public float lightavg { get; set; }


    //Attributes that are JSON and must be stored as strings 

    [NGSIJArray] public string accelerometer { get; set; }


    [NGSIJArray] public string gyroscope { get; set; }
    [NGSIJObject] public string gps { get; set; }


    [NGSIJObject] public string wifi { get; set; }

    [NGSIJArray] public string bledevices { get; set; }

    public string connectivity { get; set; }
    public float proximitymax { get; set; }
    public float proximitymin { get; set; }
    public string proximity { get; set; }
    public double soundmax { get; set; }
    public double soundmin { get; set; }
    public double soundavg { get; set; }
    [NGSIJArray] public string battery { get; set; }
    [NGSIJArray] public string compass { get; set; }
    [NGSIJArray] public string barometer { get; set; }

    public string ToStringCustom()
    {
        return
            $"Data=> id\t{this.id}\ntime\t{this.timestamp}\noperating_system\t{this.operating_system}\nactivity\t{this.activity}\nlocation\t{this.location}\ndistanceHome\t{this.distanceHome}\nwifi\t{this.wifi}\nstep_count\t{this.step_count}\nsoundpeak\t{this.soundmax}\nsounavg\t{this.soundavg}\nBLT\t{this.bledevices}";
    }
}