using System;
using System.Collections.Generic;
using NGSIBaseModel.Models;

namespace NGSIBaseModel.Test.TestModels
{
    public class MedicationEntryGroup : NgsiBaseModel
    {
        public MedicationEntryGroup()
        {
        }

        public List<MedicationEntry> meds { get; set; }

        [NGSIIgnore] public int _id { get; set; }

        //[Ignore]
        public string id { get; set; }
        
        [NGSIDateTime] public DateTime timestamp { get; set; }

        
    }
}