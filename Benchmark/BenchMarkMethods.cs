using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;
using NGSIBaseModel;
using NGSIBaseModel.Models;
using NGSIBaseModel.Test;
using NGSIBaseModel.Test.TestModels;

namespace Benchmark;

[MemoryDiagnoser()]
public class BenchMarkMethods
{
    private Car? _car;
    private JObject? _carJson;
    private JObject? _carJson2;
    private PropertyInfo property;
    private string propertyName;

    private readonly string fileJson =
        "C:/Users/marce/RiderProjects/NgsiBaseModel4CSharp/NGSIBaseModel.Test/jsonFiles/car_keyValues.json";

    private readonly string fileJson2 =
        "C:/Users/marce/RiderProjects/NgsiBaseModel4CSharp/NGSIBaseModel.Test/jsonFiles/car.json";


    [GlobalSetup]
    public void SetUp()
    {
        _car = TestUtils.InitCar();
        _carJson = (JObject) TestUtils.ReadJsonFromFile(fileJson);
        _carJson2 = (JObject) TestUtils.ReadJsonFromFile(fileJson2);
        property = _car.GetType().GetProperty("model");
        propertyName = property.PropertyType.Name;
    }
    /*    
      [Benchmark]
      public Car ActivatorMethod()
      {
         // return (Car) Activator.CreateInstance(typeof(Car),1992);
         return (Car) Activator.CreateInstance(typeof(Car));
      }
      
      [Benchmark]
      public Car ActivatorMethodOneParams()
      {
          // return (Car) Activator.CreateInstance(typeof(Car),1992);
          return (Car) Activator.CreateInstance(typeof(Car),1992);
      }
      
      [Benchmark]
      public Car ActivatorMethodTwoParams()
      {
          // return (Car) Activator.CreateInstance(typeof(Car),1992);
          return (Car) Activator.CreateInstance(typeof(Car),1992,"red");
      }
      
      
      [Benchmark]
      public Car GetConstructorInfo()
      {
          return (Car)typeof(Car).GetConstructor(BindingFlags.Public|BindingFlags.Instance, new []{typeof(int)})
              .Invoke(new object?[]{1992});
      }
  
      private readonly ConstructorInfo? ctroInfo =
          typeof(Car).GetConstructor(BindingFlags.Public | BindingFlags.Instance, new []{typeof(int)});
      
      private readonly object[] empty=new object?[]{1992};
      
      [Benchmark]
      public Car GetConstructorInfoCatched()
      {
          return (Car)ctroInfo.Invoke(new object?[]{1992});
      }
      [Benchmark]
      public Car GetConstructorInfoCatched2()
      {
          return (Car)ctroInfo.Invoke(empty);
      }
    
  
      [Benchmark]
      public JObject ToNgsi()
      {
          return NgsiBaseModel.ToNgsi<Car>(_car);
      }
        */

    [Benchmark]
    public JObject ToJsonNewtonsoft()
    {
        return JObject.FromObject(_car);
    }

  


    [Benchmark]
    public Car FromNgsiKeyValues()
    {
        return NgsiBaseModel.FromNgsi<Car>(_carJson);
    }

    [Benchmark]
    public Car FromNgsi()
    {
        return NgsiBaseModel.FromNgsi<Car>(_carJson2);
    }


}