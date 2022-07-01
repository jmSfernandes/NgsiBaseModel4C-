using System;
using NGSIBaseModel.Models;

namespace NGSIBaseModel.Test.TestModels
{
    public enum TimeOfDay
    {
        WakeTime,
        LunchTime,
        SnackTime,
        BedTime
    }

    public class MedicationEntry : NgsiBaseModel
    {
        public MedicationEntry()
        {
            MedName = "";
            MedQuantity = "";
            StartDate = DateTime.Now;
            EndDate = new DateTime();
        }

        public MedicationEntry(MedicationEntry med)
        {
            MedName = med.MedName;
            MedQuantity = med.MedQuantity;
            StartDate = med.StartDate;
            EndDate = med.EndDate;
            current = med.current;
            id = med.id;
            MedTimeOfDay = med.MedTimeOfDay;
        }

        [NGSIIgnore] public int _id { get; set; }

        //[Ignore]
        public string id { get; set; }

        [NGSIEncode] public string MedName { get; set; }

        [NGSIEncode] public string MedQuantity { get; set; }

        public bool current { get; set; }

        [NGSIIgnore]public TimeOfDay MedTimeOfDay { get; set; }
        
        public string TimeOfDay
        {
            get => MedTimeOfDay.ToString();
        }

        [NGSIDateTime] public DateTime StartDate { get; set; }

        [NGSIDateTime] public DateTime? EndDate { get; set; }
    }
}