using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models;
using NGSIBaseModel.Test.TestModels;
using Xunit;

namespace NGSIBaseModel.Test
{
    public class ComplexObjectsAndLists
    {
        [Fact]
        public void TestFromNgsiComplexList()
        {
            var expected = InitSensor();
            expected.accelerometerList.ForEach(x => x.id = null);
            var json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor.json");
            var actual = NgsiBaseModel.FromNgsi<Sensor>(json);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestFromNgsiComplexListKeyValues()
        {
            var expected = InitSensor();
            expected.accelerometerList.ForEach(x => x.id = null);
            var json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor_keyValues.json");
            var actual = NgsiBaseModel.FromNgsi<Sensor>(json);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void TestFromNgsiComplexListMapId()
        {
            var expected = InitSensorById();
            expected.accelerometerList.ForEach(x =>
            {
                x.t = default;
                x.x = default;
                x.y = default;
                x.z = default;
            });
            var json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/Sensor_map_byId.json");
            var actual = NgsiBaseModel.FromNgsi<SensorMapById>(json);

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void TestFromNgsiComplexObject()
        {
            var expected = InitModelObject();
            expected.accelerometer.id = null;
            var json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/NgsiModelObject.json");
            var actual = NgsiBaseModel.FromNgsi<NgsiModelObject>(json);

            Assert.Equal(actual, expected);
        }
        
        
        [Fact]
        public void TestFromNgsiComplexObjectMapById()
        {
            var expected = InitModelObjectById();
            expected.accelerometer.t = default;
            expected.accelerometer.x = default;
            expected.accelerometer.y = default;
            expected.accelerometer.z = default;
            var json = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/NgsiModelObjectById.json");
            var actual = NgsiBaseModel.FromNgsi<NgsiModelObjectById>(json);

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
        public void TestToNgsiComplexObject()
        {
            var obj = InitModelObject();

            var expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/NgsiModelObject.json");
            var actual = NgsiBaseModel.ToNgsi<NgsiModelObject>(obj);

            Assert.True(TestUtils.CompareJson(expected, actual));
        }

        [Fact]
        public void TestToNgsiComplexObjectMapById()
        {
            var obj = InitModelObjectById();

            var expected = (JObject) TestUtils.ReadJsonFromFile("../../../jsonFiles/NgsiModelObjectById.json");
            var actual = NgsiBaseModel.ToNgsi<NgsiModelObjectById>(obj);

            Assert.True(TestUtils.CompareJson(expected, actual));
        }

        private NgsiModelObject InitModelObject()
        {
            var test = new NgsiModelObject()
            {
                id = "model_1",
                location = "home",
                distanceHome = 1000,
                accelerometer = GetAccelList()[0]
            };

            return test;
        }

        private NgsiModelObjectById InitModelObjectById()
        {
            var test = new NgsiModelObjectById()
            {
                id = "model_1",
                location = "home",
                distanceHome = 1000,
                accelerometer = GetAccelList()[0]
            };

            return test;
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


        private SensorMapById InitSensorById()
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
            var accelerometers = new List<Accelerometer>
            {
                new() {id = "acel_1", x = -0.384399, y = 2.5191802, z = 9.2885742, t = "2020-10-06T18:42:14Z"},
                new() {id = "acel_2", x = -0.357467, y = 2.4694976, z = 9.5740814, t = "2020-10-06T18:42:14Z"},
                new() {id = "acel_3", x = -0.4628143, y = 2.37971496, z = 9.45077514, t = "2020-10-06T18:42:14Z"},
                new() {id = "acel_4", x = -0.4059600, y = 2.38749694, z = 9.59562683, t = "2020-10-06T18:42:14Z"}
            };
            return accelerometers;
        }
    }
}