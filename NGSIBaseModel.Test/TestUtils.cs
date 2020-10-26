using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models;
using RestSharp;
using Xamarin.Forms;


namespace NGSIBaseModel.Test
{
    public class TestUtils
    {
        public static void InsertCourseMockData(String file)
        {
            var testData = new StreamReader(file).ReadToEnd();
            var client = new RestClient("http://socialiteorion2.dei.uc.pt:9000/v2/op/update");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Fiware-ServicePath", "/");
            request.AddHeader("Fiware-Service", "batina_test");
            request.AddHeader("Content-Type", "application/json");

            var body = JsonConvert.SerializeObject(new
                {actionType = "REPLACE", entities = JsonConvert.DeserializeObject(testData)});

            request.AddParameter("undefined", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Debug.Write(response.Content);
        }

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

        public static ImageSource GetImage(String id)
        {
            var client = new RestClient("http://socialiteorion2.dei.uc.pt:9000/v2/entities/" + id +
                                        "?type=media_file&options=keyValues");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Fiware-ServicePath", "/");
            request.AddHeader("Fiware-Service", "batina_test");

            IRestResponse response = client.Execute(request);
            var responseJson = JToken.Parse(response.Content);
            ImageSource im = ImageSource.FromUri(new Uri(responseJson["download_path"].ToString()));
            return im;
        }

        public static T GetEntity<T>(String id)
        {
            var type = typeof(T).Name.ToLower();
            var client = new RestClient("http://socialiteorion2.dei.uc.pt:9000/v2/entities/" + id + "?type=" + type +
                                        "&options=keyValues");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Fiware-ServicePath", "/");
            request.AddHeader("Fiware-Service", "batina_test");

            IRestResponse response = client.Execute(request);
            var responseJson = JToken.Parse(response.Content);

            return NgsiBaseModel.FromNgsi<T>(responseJson);
        }

        public static void DeleteEntity<T>(String id)
        {
            var type = typeof(T).Name.ToLower();
            var client = new RestClient("http://socialiteorion2.dei.uc.pt:9000/v2/entities/" + id + "?type=" + type);
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("Fiware-ServicePath", "/");
            request.AddHeader("Fiware-Service", "batina_test");

            IRestResponse response = client.Execute(request);
            //var responseJson = JToken.Parse(response.Content);

            //return NgsiBaseModel.fromNGSI<T>(responseJson);
        }

        public static bool CompareJson(JObject obj1, JObject obj2)
        {
            obj1 = (JObject) JsonConvert.DeserializeObject(obj1.ToString());
            obj2 = (JObject) JsonConvert.DeserializeObject(obj2.ToString());

            bool isEqual = true;
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
    }
}