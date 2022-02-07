using System;
using System.Collections.Generic;
using System.Globalization;
using MyPoc.Models;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models;
using NGSIBaseModel.Test.TestModels;
using RestSharp;
using Xunit;

namespace NGSIBaseModel.Test
{
    public class TestsNgsiBaseModel
    {
        public TestsNgsiBaseModel()
        {
            //TestUtils.InsertCourseMockData("../../../testMockData.json");
            //TestUtils.InsertCourseMockData("../../../testMockDataStudents.json");
            //TestUtils.InsertCourseMockData("../../../mockDataQuestionnaires.json");
        }


        [Fact]
        public void TestNgsiIgnoreFromNgsi()
        {
            Car car = TestUtils.ReadEntityFromMockData<Car>("../../../jsonFiles/car.json");
            Assert.Null(car.ignoreMe);
        }

        [Fact]
        public void TestNgsiToNgsi()
        {
            Car test = InitCar();
            JObject expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car.json");
            JObject actual = NgsiBaseModel.ToNgsi<Car>(test);
            Assert.True(TestUtils.CompareJson(expected, actual));
        }

        [Fact]
        public void TestNgsiEncode()
        {
            Car test = InitCar();
            string expected = "This%20car%20is%20a%20simple%20car%3d%3d!%22%28with%20a%20red%20hood%29";
            JObject test_json = NgsiBaseModel.ToNgsi<Car>(test);
            string actual = ((JObject) test_json.GetValue("encodeme"))?.GetValue("value")?.ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestNgsiDecode()
        {
            Car car = TestUtils.ReadEntityFromMockData<Car>("../../../jsonFiles/car.json");
            string expected = "This car is a simple car==!\"(with a red hood)";
            Assert.Equal(car.encodeMe, expected);
        }

        [Fact]
        public void TestFromNgsi()
        {
            Car expected = InitCar();

            JObject carJson = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car.json");
            Car actual = NgsiBaseModel.FromNgsi<Car>(carJson);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestFromNgsiKeyValues()
        {
            Car expected = InitCar();

            JObject carJson = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car_keyValues.json");
            Car actual = NgsiBaseModel.FromNgsi<Car>(carJson);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestFromNgsiComplexList()
        {
            Sensor expected = initSensor();

            JObject json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor.json");
            Sensor actual = NgsiBaseModel.FromNgsi<Sensor>(json);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestFromNgsiComplexListKeyValues()
        {
            Sensor expected = initSensor();

            JObject json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor_keyValues.json");
            Sensor actual = NgsiBaseModel.FromNgsi<Sensor>(json);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void TestToNgsiComplexList()
        {
            Sensor sensor = initSensor();

            JObject expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor.json");
            JObject actual = NgsiBaseModel.ToNgsi<Sensor>(sensor);

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
            var actual=NgsiBaseModel.ToNgsi<MedicationEntryGroup>(data);

            Assert.NotNull(actual);
        }

        private Car InitCar()
        {
            Car test = new Car();
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
            JArray variations = new JArray
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


        private Sensor initSensor()
        {
            Sensor test = new Sensor();
            test.id = "sensor_1";
            test.model = "arduino_dth_11";
            test.accuracy = 0.5f;
            test.timestamp = "2020-10-07T09:50:00Z";
            List<Accelerometer> accelerometers = new List<Accelerometer>();
            accelerometers.Add(new Accelerometer
                {x = -0.384399, y = 2.5191802, z = 9.2885742, t = "2020-10-06T18:42:14Z"});
            accelerometers.Add(new Accelerometer
                {x = -0.357467, y = 2.4694976, z = 9.5740814, t = "2020-10-06T18:42:14Z"});
            accelerometers.Add(new Accelerometer
                {x = -0.4628143, y = 2.37971496, z = 9.45077514, t = "2020-10-06T18:42:14Z"});
            accelerometers.Add(new Accelerometer
                {x = -0.4059600, y = 2.38749694, z = 9.59562683, t = "2020-10-06T18:42:14Z"});
            test.accelerometerList = accelerometers;

            return test;
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