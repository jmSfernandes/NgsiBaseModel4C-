using System;
using System.Collections.Generic;
using System.Globalization;
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
            Car test = InitCar();

            JObject car_json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car.json");
            Car test_car = NgsiBaseModel.FromNgsi<Car>(car_json);

            Assert.Equal(test, test_car);
        }

        [Fact]
        public void TestFromNgsiKeyValues()
        {
            Car test = InitCar();

            JObject car_json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/car_keyValues.json");
            Car test_car = NgsiBaseModel.FromNgsi<Car>(car_json);

            Assert.Equal(test, test_car);
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
            var a = NgsiBaseModel.DatetimeToString(test.timestamp1);
            var b = NgsiBaseModel.StringToDatetime(test.timestamp);
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
            test.accuracy =  0.5f;
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
    }
}