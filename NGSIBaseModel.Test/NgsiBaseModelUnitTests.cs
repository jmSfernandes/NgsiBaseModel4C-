using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models;
using NGSIBaseModel.Test.TestModels;
using Xunit;

namespace NGSIBaseModel.Test;

public class TestsNgsiBaseModel
{
    [Fact]
    public void TestNgsiIgnoreFromNgsi()
    {
        var car = TestUtils.ReadEntityFromMockData<Car>("../../../jsonFiles/car.json");
        Assert.Null(car.ignoreMe);
    }

    [Fact]
    public void TestNgsiToNgsi()
    {
        var test = TestUtils.InitCar();
        var expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car.json");
        var actual = NgsiBaseModel.ToNgsi<Car>(test);
        Assert.True(TestUtils.CompareJson(expected, actual));
    }
    
    [Fact]
    public void TestNgsiToNgsi2()
    {
        var test = TestUtils.InitCar();
        var json = JsonSerializer.Serialize(test);
        
        var expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car.json");
        var actual = NgsiBaseModel.ToNgsi<Car>(test);
        Assert.True(TestUtils.CompareJson(expected, actual));
    }

    [Fact]
    public void TestNgsiEncode()
    {
        var test = TestUtils.InitCar();
        var expected = "This%20car%20is%20a%20simple%20car%3d%3d!%22%28with%20a%20red%20hood%29";
        var testJson = NgsiBaseModel.ToNgsi<Car>(test);
        var actual = ((JObject) testJson.GetValue("encodeme"))?.GetValue("value")?.ToString();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestNgsiDecode()
    {
        var car = TestUtils.ReadEntityFromMockData<Car>("../../../jsonFiles/car.json");
        var expected = "This car is a simple car==!\"(with a red hood)";
        Assert.Equal(car.encodeMe, expected);
    }

    [Fact]
    public void TestFromNgsi()
    {
        var expected = TestUtils.InitCar();

        var carJson = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car.json");
        var actual = NgsiBaseModel.FromNgsi<Car>(carJson);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestFromNgsiKeyValues()
    {
        var expected = TestUtils.InitCar();

        var carJson = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car_keyValues.json");
        var actual = NgsiBaseModel.FromNgsi<Car>(carJson);

        Assert.Equal(expected, actual);
    }


    [Fact]
    public void TestNgsiSmartphoneData()
    {
        SmartphoneData data = InitSmartphoneData();

        JObject actual = NgsiBaseModel.ToNgsi<SmartphoneData>(data);

        Assert.NotNull(actual);
    }

    [Fact]
    public void TestNgsiSleepSegment()
    {
        SleepSegment data = InitSleepSegment();

        JObject actual = NgsiBaseModel.ToNgsi<SleepSegment>(data);

        Assert.NotNull(actual);
        var app1 = new JObject
        {
            {"name", "facebook"},
            {"finality", "leisure"}
        };
    }

    [Fact]
    public void TestNgsiMedicationEntry()
    {
        var data = InitMed();

        var actual = NgsiBaseModel.ToNgsi<MedicationEntry>(data);

        Assert.NotNull(actual);
    }

    [Fact]
    public void TestNgsiMedicationEntryGroup()
    {
        var data = new MedicationEntryGroup();
        data.id = "test_123";
        var meds = new List<MedicationEntry>
        {
            InitMed(),
            InitMed(),
            InitMed()
        };
        data.meds = meds;
        var actual = NgsiBaseModel.ToNgsi<MedicationEntryGroup>(data);

        Assert.NotNull(actual);
    }

    [Fact]
    public void TestJsonConvert()
    {
        Car expected = TestUtils.InitCar();
        var obj = JObject.FromObject(expected);

        Assert.NotNull(obj);
    }

    [Fact]
    public void TestNgsiDecodeIgnore()
    {
        var expected = "Em que tipo de veículo esteve?";
        var actual= NgsiUtils.DecodeAttribute(expected);

            
        Assert.Equal(expected,actual);
    }


    private SmartphoneData InitSmartphoneData()
    {
        var test = new SmartphoneData
        {
            id = "data_1",
            activity = "Still",
            operating_system = "Android",
            timestamp = "2021-12-13T19:38:11.71Z",
            location = "Other",
            distanceHome = 0,
            step_count = 0,
            phone_lock = "False",
            foregroundApp = "",
            soundmax = 32768,
            lightmax = 40000,
            proximity = "0",
            proximitymax = 1,
        };

        return test;
    }

    private SleepSegment InitSleepSegment()
    {
        var test = new SleepSegment
        {
            id = "ss_data_1",
            timestamp = "2021-12-13T19:38:11.71Z",
            end = "2021-12-13T19:38:11.71Z",
            start = "2021-12-13T10:38:11.71Z",
            duration = 30000000,
            description = "test",
            status = "1"
        };
        return test;
    }


    private MedicationEntry InitMed()
    {
        var test = new MedicationEntry
        {
            id = "med_1",
            current = true,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            MedName = "Paracetamol (genrico)",
            MedQuantity = "25 mg",
            MedTimeOfDay = TimeOfDay.LunchTime
        };
        return test;
    }
}