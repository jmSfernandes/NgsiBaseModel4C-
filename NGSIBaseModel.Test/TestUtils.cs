using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models;
using NGSIBaseModel.Test.TestModels;

namespace NGSIBaseModel.Test;

public class TestUtils
{
    
    public static JArray readMockData(String file)
    {
        var test_data = new StreamReader(file).ReadToEnd();
        return (JArray) JsonConvert.DeserializeObject(test_data);
    }

    public static T ReadEntityFromMockData<T>(String file)
    {
        var test_data = new StreamReader(file).ReadToEnd();
        var mock_data = (JObject) JsonConvert.DeserializeObject(test_data);
        T entity = NgsiBaseModel.FromNgsi<T>((JToken) mock_data);
        return (T) entity;
    }

    public static JToken ReadJsonFromFile(String file)
    {
        var testData = new StreamReader(file).ReadToEnd();
        var mockData = (JObject) JsonConvert.DeserializeObject(testData);
        return (JToken) mockData;
    }

   
    public static bool CompareJson(JObject obj1, JObject obj2)
    {
        obj1 = (JObject) JsonConvert.DeserializeObject(obj1.ToString());
        obj2 = (JObject) JsonConvert.DeserializeObject(obj2.ToString());

        var isEqual = true;
        foreach (var property in obj1.Properties())
        {
            var _name = property.Name;
            var _value = property.Value;
            if (obj2[_name] == null)
                return false; //if object 2 doesnt have an attribute they are not equal
            else if (_value.Type == JTokenType.Object)
            {
                if (obj2[_name].Type == JTokenType.Object)
                {
                    string _type2 = obj2[_name]["type"].ToString();
                    string _value2 = obj2[_name]["value"].ToString();
                    if (!_value["type"].ToString().Equals(_type2) ||
                        !_value["value"].ToString().Equals(_value2))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (!obj2[_name].ToString().Equals(_value.ToString()))
            {
                return false;
            }
        }

        //  if all properties are equal return true;
        return isEqual;
    }
    public static Car InitCar()
    {
        var test = new Car();
        test.id = "car_1";
        test.color = "red";
        test.model = "corvette";
        test.year = 1987;

        test.encodeMe = "This car is a simple car==!\"(with a red hood)";
        test.ignoreMe = "I was ignored!";


        test.timestamp = "2020-10-07T09:50:00Z";
        test.timestamp1 = new DateTime(2020, 10, 7, 10, 50, 0).ToUniversalTime();
        var a = NgsiUtils.DatetimeToString(test.timestamp1);
        var b = NgsiUtils.StringToDatetime(test.timestamp);
        var variations = new JArray
        {
            "Street",
            "Lounge",
            "Easy",
            "SportsWagon"
        };
        test.variations = variations.ToString();
        test.variations1 = variations;
        test.variations2 = new List<string>()
        {
            "Street",
            "Lounge",
            "Easy",
            "SportsWagon"
        };


        return test;
    }
}