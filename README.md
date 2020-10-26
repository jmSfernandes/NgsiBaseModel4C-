# NgsiBaseModel 4 C#

This is a multi-purpose parser for the NGSI10 standard.
The NGSI10 standard is a the default standard used by [ORION](https://fiware-orion.readthedocs.io/en/master/) the main Componnent of the [FIWARE project](https://www.fiware.org/).

This parser is able to simply convert the Json objects received in the NGSI format (both standard and keyValues) to C# Models. 
And also convert Java classes to the equivalent and compliant NGSI10 entities.

### The Classes should extend the NGSIBaseModel. And fullfill the following constraints:

  - The class name should be the same as the entity type which we are parsing;
  
  - the fields of the class must be public, otherwise a reflection error will be raised due to the classes be in different packages;
  
  - You must use the JObject from the [Newtonsoft library](https://www.nuget.org/packages/Newtonsoft.Json). You can easly convert from and to other Json libraries by calling the method `toString()` and parsing the string.
  
  - The attributes of the class should be named the same as the entity attributes (ignoring case);
  - If the class is to be sent to an orion instance the id field should be defined, otherwise the id and type attributes will not be defined by the base model (You can still defined them manually after retriving the json);
  - The class can have list field but they should either be `List<String>` or a <code>List</code> of another class that also extends the *NGSIBaseModel* 
  - If you want to parse date strings directly the class field should be of type date. The model only supports the format `"yyyy-MM-dd'T'HH:mm:ss'Z'"`(ISO8601). 
For other formats create String fields and parse them accordingly after the model creates the objects.
  - If you have a field that supports any of the [forbidden characters of ORION](https://fiware-orion.readthedocs.io/en/master/user/forbidden_characters/index.html) use the annotation `[NGSIEncode]`. 
  The parser will perform urlEncoding and decoding on fields marked with that annotation; 
  - It is also possible to ignore field that are only client specific, with the annotation `[NGSIIgnore]`. Those fields will not be added to the NGSI json Object and will not be parsed from the NGSI objects as well.

## Example of a Class:
```C# 
    public class Car :NgsiBaseModel
    {
    public String id {get; set;};
    public String model {get; set;};
    public String color {get; set;};
    
    //this field will be converted into a JsonArray 
    public List<String> variations {get; set;};
    
    //this field will be ignored
    [NGSIIgnore]
    public String ignoreMe {get; set;}
    
    //this field will be parsed 
    [NGSIEncoded]
    public String encodeMe {get; set;};

    //this field will be parsed from and to the format "yyyy-MM-dd'T'HH:mm:ss'Z'"
    public string timestamp {get; set;};
    ...
    
    }



```


  

### From NGSI:
To convert json to a compliant class is just necessary to call the method <code>fromNgsi()</code> 
```C#
    JObject json= new JsonObject(jsonString);
    Car car= NgsiBaseModel.FromNgsi<Car>(car_json);

```

### To NGSI:
To convert json to a compliant class is just necessary to call the method <code>fromNgsi()</code> 
```C#
   
    Car test = new Car();
    test.id = "car_1";
    test.color = "red";
    test.model = "corvette";
    test.encodeMe = "This car is a simple car==!\"(with a red hood)";
    test.ignoreMe = "I was ignored!";

    test.timestamp = "2020-10-07T09:50:00Z";
    List<string> variations = new List<string>();
    variations.Add("Street");
    variations.Add("Lounge");
    variations.Add("Easy");
    variations.Add("SportsWagon");
    test.variations = variations;
    
    JObject actual = NgsiBaseModel.ToNgsi<Car>(test); 
    
     
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
