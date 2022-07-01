# NgsiBaseModel 4 C#
[![Nuget badge](https://img.shields.io/nuget/v/NGSIBaseModel)](https://www.nuget.org/packages/NGSIBaseModel)

[![Nuget badge](https://img.shields.io/nuget/dt/NGSIBaseModel)](https://www.nuget.org/packages/NGSIBaseModel)

This is a multi-purpose parser for the NGSI10 standard.
The NGSI10 standard is a the default standard used by [ORION](https://fiware-orion.readthedocs.io/en/master/) the main
Componnent of the [FIWARE project](https://www.fiware.org/).

This parser is able to simply convert the Json objects received in the NGSI format (both standard and keyValues) to C#
Models.
And also convert Java classes to the equivalent and compliant NGSI10 entities.

### The Classes should extend the NGSIBaseModel. And fullfill the following constraints:

- The class name should be the same as the entity type which we are parsing;


- the fields of the class must be public, otherwise a reflection error will be raised due to the classes be in different
  packages;


- You must use the JObject from the [Newtonsoft library](https://www.nuget.org/packages/Newtonsoft.Json). You can easly
  convert from and to other Json libraries by calling the method `toString()` and parsing the string.


- The attributes of the class should be named the same as the entity attributes (ignoring case);


- If the class is to be sent to an orion instance the id field should be defined, otherwise the id and type attributes
  will not be defined by the base model (You can still defined them manually after retriving the json);


- The class can have list field but they should either be `List<String>` or a <code>List</code> of another class that
  also extends the *NGSIBaseModel*


- If you want to parse date strings directly the class field should be of type date. The model only supports the
  format `"yyyy-MM-dd'T'HH:mm:ss'Z'"`(ISO8601).
  For other formats create String fields and parse them accordingly after the model creates the objects. You can also
  use the `[NGSIDateTime]` annotation to directly convert from `string` to `DateTime` and vice-versa;


- If you have a field that supports any of
  the [forbidden characters of ORION](https://fiware-orion.readthedocs.io/en/master/user/forbidden_characters/index.html)
  use the annotation `[NGSIEncode]`.
  The parser will perform urlEncoding and decoding on fields marked with that annotation;


- It is also possible to ignore field that are only client specific, with the annotation `[NGSIIgnore]`. Those fields
  will not be added to the NGSI json Object and will not be parsed from the NGSI objects as well.

## Example of a Class:

```csharp
    public class Car :NgsiBaseModel
    {
    public String id {get; set;};
    public String model {get; set;};
    public String color {get; set;};
    
    //this field will be converted into a Json Array this is good to use when trying to store complex information in a local database;
    [NGSIJArray]public string variations {get; set;}
    
    //this field will be converted into a Json Array as well
    public JArray variations1 {get; set;}
    
    //this field will be converted into a Json Array as well
    public List<String> variations2 { get; set; }
    //this field will be ignored
    [NGSIIgnore]
    public String ignoreMe {get; set;}
    
    //this field will be parsed 
    [NGSIEncoded]
    public String encodeMe {get; set;}

    //this field will be parsed from and to the format "yyyy-MM-dd'T'HH:mm:ss'Z'"
    public string timestamp {get; set;}
    
    [NGSIDateTime]public DateTime timestamp1 {get; set;}
    ...
    
    }


```

## From NGSI:

To convert json to a compliant class is just necessary to call the method <code>fromNgsi()</code>

```csharp
    JObject json= new JsonObject(jsonString);
    Car car= NgsiBaseModel.FromNgsi<Car>(car_json);

```

## To NGSI:

To convert json to a compliant class is just necessary to call the method <code>fromNgsi()</code>

```csharp
   
    Car test = new Car();
    test.id = "car_1";
    test.color = "red";
    test.model = "corvette";
    test.encodeMe = "This car is a simple car==!\"(with a red hood)";
    test.ignoreMe = "I was ignored!";

    test.timestamp = "2020-10-07T09:50:00Z";
    test.timestamp1 = new DateTime(2020,10,7,10,50,0).ToUniversalTime();
    JArray variations = new JArray
           {
               "Street",
               "Lounge",
               "Easy",
               "SportsWagon"
           };
    test.variations = variations.ToString();
    
    JObject actual = NgsiBaseModel.ToNgsi<Car>(test); 
    
     
```

## Complex Types

Its also possible to map complex types; for instance, a object `Sensor` could hold a list of values for
a `Accelerometer`:

```csharp
     public class Sensor: NgsiBaseModel
    {
        public string id { get; set; }
        public string model { get; set; }
        
        //the Accelerometer class is a NgsiBaseModel class as well
        public List<Accelerometer> accelerometerList { get; set; }

        public string timestamp { get; set; }
    }
    
     public class Accelerometer:NgsiBaseModel
    {
        
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public string t { get; set; }
    }

```

The values are then mapped to a `JArray` of `JObject`s`:

```json

{
  "id": "sensor_1",
  "type": "sensor",
  ...
  "accelerometerlist": {
    "type": "Text",
    "value": [
      {
        "x": -0.384399,
        "y": 2.5191802,
        "z": 9.2885742,
        "t": "2020-10-06T18:42:14Z"
      },
      ...
      {
        "x": -0.4059600,
        "y": 2.38749694,
        "z": 9.59562683,
        "t": "2020-10-06T18:42:14Z"
      }
    ]
  }
}
```

This data can be mapped to and from Ngsi format:

```csharp
//To NGSI

  Sensor test = new Sensor();
  test.id = "sensor_1";
  test.model = "arduino_dth_11";
  test.timestamp = "2020-10-07T09:50:00Z";
  List<Accelerometer> accelerometers = new List<Accelerometer>();
  accelerometers.Add(new Accelerometer
      {id="acel_1",x = -0.384399, y = 2.5191802, z = 9.2885742, t = "2020-10-06T18:42:14Z"});
  accelerometers.Add(new Accelerometer
      {id="acel_2",x = -0.357467, y = 2.4694976, z = 9.5740814, t = "2020-10-06T18:42:14Z"});
  accelerometers.Add(new Accelerometer
      {id="acel_3",x = -0.4628143, y = 2.37971496, z = 9.45077514, t = "2020-10-06T18:42:14Z"});
  accelerometers.Add(new Accelerometer
      {id="acel_4",x = -0.4059600, y = 2.38749694, z = 9.59562683, t = "2020-10-06T18:42:14Z"});
  test.accelerometerList = accelerometers;
  
    JObject jobj = NgsiBaseModel.ToNgsi<Sensor>(test); 
    
 //From NGSI
 Sensor test1 = NgsiBaseModel.FromNgsi<Sensor>(test1);
  
```

Its also possible to map only the ids of nested objects by using the `[NGSIMapIds]`
Obtaining only a Json Array of strings containing the ids of the objects.


Its alo possible to work with single objects instead of lists,
where the object will be mapped to a Json Object.
Or if the `[NGSIMapIds]`, attribute is use the id of the object will be mapped to a string value.


**NOTE**: To use the `[NGSIMapIds]` attribute the target object or List of objects must extend the class NgsiBaseModel and have a string `id` property.

```csharp
public class Sensor: NgsiBaseModel
{
    public string id { get; set; }
    public string model { get; set; }
    
    //the Accelerometer class is a NgsiBaseModel class as well
    [NGSIMapIds]public List<Accelerometer> accelerometerList { get; set; }

    public string timestamp { get; set; }
}
```

```json
{
  "id": "sensor_1",
  "type": "sensor",
  ...
  "accelerometerlist": {
    "type": "Text",
    "value": [
      "acel_1",
      "acel_2",
      "acel_3",
      "acel_4"
    ]
  }
}

```



## Use the Library

Copyright [2020]

```
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.```
