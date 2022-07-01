using System.Reflection;
using BenchmarkDotNet.Attributes;
using NGSIBaseModel.Models;
using NGSIBaseModel.Test;
using NGSIBaseModel.Test.TestModels;

namespace Benchmark;

public class BenchMarkAttribute
{
    private Car? _car;
    private readonly Attribute _ignore = new NGSIIgnore();
    private readonly Attribute _datetime = new NGSIDateTime();
    private readonly Attribute _json = new NGSIJObject();
    private readonly Attribute _jArray = new NGSIJArray();
    private readonly Attribute _encode = new NGSIEncode();

    [GlobalSetup]
    public void SetUp()
    {
        _car = TestUtils.InitCar();
    }

    [Benchmark]
    public void AttributeIsDefined()
    {
        foreach (var property in _car.GetType().GetProperties())
        {
            Attribute.IsDefined(property, typeof(NGSIIgnore));
            Attribute.IsDefined(property, typeof(NGSIEncode));
            Attribute.IsDefined(property, typeof(NGSIDateTime));
            Attribute.IsDefined(property, typeof(NGSIJObject));
            Attribute.IsDefined(property, typeof(NGSIJArray));
        }
    }

    [Benchmark]
    public void PropertyContains()
    {
        foreach (var property in _car.GetType().GetProperties())
        {
            property.GetCustomAttributes().Contains(new NGSIIgnore());
            property.GetCustomAttributes().Contains(new NGSIEncode());
            property.GetCustomAttributes().Contains(new NGSIDateTime());
            property.GetCustomAttributes().Contains(new NGSIJArray());
            property.GetCustomAttributes().Contains(new NGSIJObject());
        }
    }

   

    [Benchmark]
    public void PropertyContainsWithObj()
    {
        foreach (var property in _car.GetType().GetProperties())
        {
            property.GetCustomAttributes().Contains(_ignore);
            property.GetCustomAttributes().Contains(_encode);
            property.GetCustomAttributes().Contains(_datetime);
            property.GetCustomAttributes().Contains(_json);
            property.GetCustomAttributes().Contains(_jArray);
        }
    }

    [Benchmark]
    public void PropertyContainsWithObjCache()
    {
        foreach (var property in _car.GetType().GetProperties())
        {
            var attrs = property.GetCustomAttributes();
            attrs.Contains(_ignore);
            attrs.Contains(_encode);
            attrs.Contains(_datetime);
            attrs.Contains(_json);
            attrs.Contains(_jArray);
        }
    }
}