using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models;
using NGSIBaseModel.Test.TestModels;
using Xunit;

namespace NGSIBaseModel.Test
{
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
        public void TestFromNgsiComplexList()
        {
            var expected = InitSensor();

            var json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor.json");
            var actual = NgsiBaseModel.FromNgsi<Sensor>(json);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestFromNgsiComplexListKeyValues()
        {
            var expected = InitSensor();

            var json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor_keyValues.json");
            var actual = NgsiBaseModel.FromNgsi<Sensor>(json);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void TestToNgsiComplexList()
        {
            var sensor = InitSensor();

            var expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor.json");
            var actual = NgsiBaseModel.ToNgsi<Sensor>(sensor);

            Assert.True(TestUtils.CompareJson(expected, actual));
        }

        [Fact]
        public void TestToNgsiComplexListMapById()
        {
            var sensor = InitSensorById();

            var expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor_map_byId.json");
            var actual = NgsiBaseModel.ToNgsi<SensorMapById>(sensor);

            Assert.True(TestUtils.CompareJson(expected, actual));
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


        private Sensor InitSensor()
        {
            var test = new Sensor
            {
                id = "sensor_1",
                model = "arduino_dth_11",
                accuracy = 0.5f,
                timestamp = "2020-10-07T09:50:00Z",
                accelerometerList = GetAccelList()
            };

            return test;
        }

       
        private Sensor InitSensorById()
        {
            var test = new SensorMapById
            {
                id = "sensor_1",
                model = "arduino_dth_11",
                accuracy = 0.5f,
                timestamp = "2020-10-07T09:50:00Z",
                accelerometerList = GetAccelList()
            };

            return test;
        }

        private List<Accelerometer> GetAccelList()
        {
            List<Accelerometer> accelerometers = new List<Accelerometer>
            {
                new() {id = "acel_1", x = -0.384399, y = 2.5191802, z = 9.2885742, t = "2020-10-06T18:42:14Z"},
                new() {id = "acel_2", x = -0.357467, y = 2.4694976, z = 9.5740814, t = "2020-10-06T18:42:14Z"},
                new() {id = "acel_3", x = -0.4628143, y = 2.37971496, z = 9.45077514, t = "2020-10-06T18:42:14Z"},
                new() {id = "acel_4", x = -0.4059600, y = 2.38749694, z = 9.59562683, t = "2020-10-06T18:42:14Z"}
            };
            return accelerometers;
        }

        private SmartphoneData InitSmartphoneData()
        {
            SmartphoneData test = new SmartphoneData();
            test.id = "data_1";
            test.activity = "Still";
            test.operating_system = "Android";
            test.timestamp = "2021-12-13T19:38:11.71Z";
            test.location = "Other";
            test.distanceHome = 0;
            test.step_count = 0;
            test.phone_lock = "False";
            test.foregroundApp = "";
            test.foregroundApp = "";
            test.lightmax = 40000;
            test.lightmin = 0;
            test.lightavg = 0;
            test.proximity = "0";
            test.proximitymax = 1;
            test.proximitymin = 0;
            test.soundmax = 32768;
            test.soundmin = 0;
            test.soundavg = 0;

            return test;
        }

        private SleepSegment InitSleepSegment()
        {
            SleepSegment test = new SleepSegment();
            test.id = "ss_data_1";

            test.timestamp = "2021-12-13T19:38:11.71Z";
            test.end = "2021-12-13T19:38:11.71Z";
            test.start = "2021-12-13T10:38:11.71Z";
            test.start = "2021-12-13T10:38:11.71Z";
            test.duration = 30000000;
            test.description = "test";
            test.status = "1";
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
            var j = new JObject {{"test", 1}};
            return test;
        }
    }
}