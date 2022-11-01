using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models;
using NGSIBaseModel.Models.Attributes;

namespace NGSIBaseModel.Test.TestModels;

public class Car : NgsiBaseModel
{
    public Car()
    {
    }

    public Car(int year)
    {
        this.year = year;
    }

    public Car(int year, string color=null)
    {
        this.year = year;
        this.color = color;
    }

    public string id { get; set; }
    public string model { get; set; }
    public string color { get; set; }
    public int year { get; set; }

    public string timestamp { get; set; }

    [NGSIDateTime] public DateTime timestamp1 { get; set; }

    [NGSIJArray] public string variations { get; set; }

    public JArray variations1 { get; set; }

    public List<string> variations2 { get; set; }

    [NGSIIgnore] public string ignoreMe { get; set; }

    [NGSIEncode] public string encodeMe { get; set; }
}