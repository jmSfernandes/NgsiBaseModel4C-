using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NGSIBaseModel.Models.Attributes;

namespace NGSIBaseModel.Models;

public class NgsiBaseModel
{
    private static readonly Attribute IgnoreAttr = new NGSIIgnore();
    private static readonly Attribute EncodeAttr = new NGSIEncode();
    private static readonly Attribute JObjAttr = new NGSIJObject();
    private static readonly Attribute JArrayAttr = new NGSIJArray();
    private static readonly Attribute DateTimeAttr = new NGSIDateTime();
    private static readonly Attribute MapIdsAttr = new NGSIMapIds();

    public static T FromNgsi<T>(JToken entity)
    {
        if (entity.Contains("type"))
        {
            if (!typeof(T).Name.ToLower().Equals(entity["type"].ToString()))
            {
                throw new Exception("The type of NGSI Entity must be the same as the class");
            }
        }

        var obj = Activator.CreateInstance<T>();
        foreach (var property in typeof(T).GetProperties())
        {
            var isIgnore = property.GetCustomAttributes().Contains(IgnoreAttr);
            if (isIgnore || !((JObject) entity).ContainsKey(property.Name.ToLower())) continue;


            var isEncoded = property.GetCustomAttributes().Contains(EncodeAttr);
            var isJObject = property.GetCustomAttributes().Contains(JObjAttr);
            var isJArray = property.GetCustomAttributes().Contains(JArrayAttr);
            var isMapId = property.GetCustomAttributes().Contains(MapIdsAttr);


            var attribute = entity[property.Name.ToLower()];

            if (attribute.Type == JTokenType.Object && attribute["value"] != null)
                attribute = attribute["value"];

            obj = SetValue<T>(obj, attribute, property, isEncoded, isJObject, isJArray, isMapId);
        }

        return obj;
    }

    private static T SetValue<T>(object obj, JToken value, PropertyInfo property, bool isEncoded, bool isJObject,
        bool isJArray, bool isMapId)
    {
        if (value == null) return (T) obj;

        if (value.Type == JTokenType.String)
        {
            if (isMapId)
                return (T) SetObjectProperty(obj, value, property, isEncoded, isMapId);

            if (isEncoded)
                value = NgsiUtils.DecodeAttribute(value);


            property.SetValue(obj, value.ToString());
        }
        else if (value.Type == JTokenType.Date)
        {
            var dateTime = NgsiUtils.StringToDatetime(value.ToString());
            if (property.GetCustomAttributes().Contains(new NGSIDateTime()))
                property.SetValue(obj, dateTime);
            else
            {
                dateTime = dateTime.ToLocalTime();
                property.SetValue(obj, NgsiUtils.DatetimeToString(dateTime));
            }
        }
        else if (value.Type == JTokenType.Integer)
        {
            property.SetValue(obj, int.Parse(value.ToString()));
        }
        else if (value.Type == JTokenType.Float && property.PropertyType == typeof(float))
        {
            property.SetValue(obj, float.Parse(value.ToString()));
        }
        else if (value.Type == JTokenType.Float)
        {
            property.SetValue(obj, double.Parse(value.ToString()));
        }
        else if (value.Type == JTokenType.Array)
        {
            if (isJArray)
            {
                property.SetValue(obj, value.ToString());
            }
            else if (property.PropertyType.Name.ToLower().Equals("jarray"))
            {
                property.SetValue(obj, (JArray) value);
            }
            else
            {
                var type = property.PropertyType;
                if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) return (T) obj;

                var itemType = type.GetGenericArguments()[0]; // use this...
                var array = Activator.CreateInstance(type);

                foreach (var v in value)
                {
                    var val = v;
                    if (isEncoded)
                        val = NgsiUtils.DecodeAttribute(val);

                    if (itemType.Name.ToLower().Equals("string"))
                    {
                        array.GetType().GetMethod("Add").Invoke(array, new[] {val.ToString()});
                    }
                    else
                    {
                        var objSub = Activator.CreateInstance(itemType);

                        if (itemType.BaseType != null && itemType.BaseType == typeof(NgsiBaseModel) && !isMapId)
                        {
                            objSub = typeof(NgsiBaseModel).GetMethod("FromNgsi")
                                .MakeGenericMethod(itemType)
                                .Invoke(typeof(NgsiBaseModel), new object[] {val});
                        }
                        else
                        {
                            objSub.GetType().GetProperty("id").SetValue(objSub, val.ToString());
                        }

                        array.GetType().GetMethod("Add").Invoke(array, new object[] {objSub});
                    }
                }

                property.SetValue(obj, array);
            }
        }
        else if (value.Type == JTokenType.Object)
        {
            if (isJObject)
                property.SetValue(obj, value.ToString());
            else
                return (T) SetObjectProperty(obj, value, property, isEncoded, isMapId);
        }

        return (T) obj;
    }

    private static object SetObjectProperty(object obj, JToken value, PropertyInfo property, bool isEncoded,
        bool isMapId)
    {
        if (isEncoded)
            value = NgsiUtils.DecodeAttribute(value);
        var objType = property.PropertyType;
        var objSub = Activator.CreateInstance(objType);
        if (objType.BaseType != null && objType.BaseType == typeof(NgsiBaseModel))
        {
            if (isMapId)
                objSub.GetType().GetProperty("id").SetValue(objSub, value.ToString());
            else
                objSub = typeof(NgsiBaseModel).GetMethod("FromNgsi")
                    .MakeGenericMethod(objType)
                    .Invoke(obj, new object[] {value});
        }
        else
        {
            objSub = JsonConvert.DeserializeObject(value.ToString());
        }

        property.SetValue(obj, objSub);

        return obj;
    }

    public static JObject ToNgsi<T>(object obj)
    {
        if (!typeof(T).Name.ToLower().Equals(obj.GetType().Name.ToLower()))
        {
            throw new Exception("The type of NGSI Entity must be the same as the class");
        }

        if (obj.GetType().GetProperty("id") == null)
        {
            throw new Exception(
                "The model is not compliant with the NGSIBaseModel it must implement an id property");
        }

        var _id = (string) typeof(T).GetProperty("id").GetValue(obj);

        var json = new JObject
        {
            {"id", _id},
            {"type", typeof(T).Name.ToLower()}
        };
        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            var attrs = prop.GetCustomAttributes();
            var isIgnore = attrs.Contains(IgnoreAttr);

            if (isIgnore || (prop.Name == "id")) continue;

            var isEncoded = attrs.Contains(EncodeAttr);
            var isJObject = attrs.Contains(JObjAttr);
            var isJArray = attrs.Contains(JArrayAttr);
            var isDate = attrs.Contains(DateTimeAttr);
            var isMapIds = attrs.Contains(MapIdsAttr);


            var value = prop.GetValue(obj);
            if (value == null) continue;

            var propType = prop.PropertyType;
            var propTypeName = propType.Name.ToLower();

            var propertyName = prop.Name.ToLower();

            if (isJObject)
                value = JObject.Parse((string) value);
            else
                isJObject = propTypeName.Equals("jobject");

            if (isJArray)
                value = JArray.Parse((string) value);
            else
                isJArray = propTypeName.Equals("jarray");


            if (propType.IsClass && (isJArray || isJObject))
            {
                var attrJson = SetJsonAttributeComplex(value, prop, "object", isEncoded);
                json.Add(propertyName, attrJson);
            }
            else if (propType.IsAnsiClass && isDate)
            {
                var attrJson = SetJsonAttributeComplex(value, prop, "date", isEncoded);
                json.Add(propertyName, attrJson);
            }
            else if (propType.IsGenericType &&
                     propType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var attrJson = SetJsonAttributeComplex(value, prop, "array", isEncoded, isMapIds);
                json.Add(propertyName, attrJson);
            }
            else if (propType.IsClass && !propTypeName.Equals("string"))
            {
                var attrJson = SetJsonAttributeComplex(value, prop, "object", isEncoded, isMapIds);
                json.Add(propertyName, attrJson);
            }
            else
            {
                var attrJson = SetJsonAttributeSimple(value, prop, isEncoded);
                json.Add(propertyName, attrJson);
            }
        }

        return json;
    }


    private static JToken SetJsonAttributeSimple(object value, PropertyInfo property, bool isEncoded)
    {
        var attrObj = new JObject();
        if (isEncoded)
            value = NgsiUtils.EncodeAttribute(value.ToString());
        switch (JToken.FromObject(value).Type)
        {
            case (JTokenType.String):
                attrObj.Add("value", value.ToString());
                if (NgsiUtils.IsDatetime(value.ToString()))
                    attrObj.Add("type", "ISO8601");
                else
                    attrObj.Add("type", "Text");
                break;
            case (JTokenType.Date):
                attrObj.Add("value", $"{value:yyyy-MM-dd HH:mm:ss}");
                attrObj.Add("type", "ISO8601");
                break;
            case (JTokenType.Integer):
                if (property.PropertyType.Name.Equals("Int32"))
                    attrObj.Add("value", (int) value);
                else
                    attrObj.Add("value", (long) value);
                attrObj.Add("type", "Integer");
                break;
            case (JTokenType.Float):
                if (property.PropertyType.Name.Equals("Single"))
                    attrObj.Add("value", (float) value);
                else
                    attrObj.Add("value", (double) value);
                attrObj.Add("type", "Float");
                break;
            case (JTokenType.Boolean):
                attrObj.Add("value", (bool) value);
                attrObj.Add("type", "Boolean");
                break;
            default:
                break;
        }

        return attrObj;
    }


    private static JToken SetJsonAttributeComplex(object value, PropertyInfo property, string type, bool isEncoded,
        bool isMapIds = false)
    {
        var attrObj = new JObject();

        switch (type)
        {
            case ("object"):
                if (value.GetType() == typeof(JObject))
                {
                    attrObj.Add("value", (JObject) value);
                    attrObj.Add("type", "Text");
                }
                else if (value.GetType() == typeof(JArray))
                {
                    attrObj.Add("value", (JArray) value);
                    attrObj.Add("type", "Text");
                }
                else
                {
                    if (isMapIds)
                    {
                        //verifies if the typeGeneric is a NgsiBaseModel if not throws an exception
                        VerifyIsNgsiModel(value.GetType());
                        var obj = (string) MapById(value, isEncoded);
                        attrObj.Add("value", obj);
                        attrObj.Add("type", "Text");
                    }
                    else
                    {
                        var obj = (JObject) MapByValue(value, isEncoded);
                        attrObj.Add("value", obj);
                        attrObj.Add("type", "StructuredValue");
                    }
                }

                break;
            case ("array"):
                JArray array;
                if (isMapIds)
                    array = MapListById(value, property, isEncoded);
                else
                    array = (JArray) MapListByValue(value, property, isEncoded);

                attrObj.Add("value", array);
                attrObj.Add("type", "Text");
                break;
            case ("date"):
                attrObj.Add("value", NgsiUtils.DatetimeToString((DateTime) value));
                attrObj.Add("type", "ISO8601");
                break;
            default:
                break;
        }


        return attrObj;
    }

    private static JArray MapListById(object value, PropertyInfo property, bool isEncoded)
    {
        var typeGeneric = property.PropertyType.GetGenericArguments()[0];

        //verifies if the typeGeneric is a NgsiBaseModel if not throws an exception
        VerifyIsNgsiModel(typeGeneric);

        var array = new JArray();
        foreach (var v in (IEnumerable) value)
        {
            string val = MapById(v, isEncoded);
            array.Add(val);
        }

        return array;
    }

    private static string MapById(object value, bool isEncoded)
    {
        var val = (string) value.GetType().GetProperty("id").GetValue(value);
        if (isEncoded)
            val = NgsiUtils.EncodeAttribute(val);
        return val;
    }

    private static JToken MapByValue(object value, bool isEncoded)
    {
        var val = JsonConvert.SerializeObject(value);
        if (isEncoded)
            val = NgsiUtils.EncodeAttribute(val);
        var obj = JToken.Parse(val);
        if (!value.GetType().Name.ToLower().Equals("string") && value.GetType().GetProperty("id") != null)
            ((JObject) obj).Remove("id");
        return obj;
    }

    private static JArray MapListByValue(object value, PropertyInfo property, bool isEncoded)
    {
        var typeGeneric = property.PropertyType.GetGenericArguments()[0];
        var val = JsonConvert.SerializeObject(value);
        if (isEncoded)
            val = NgsiUtils.EncodeAttribute(val);
        var array = JArray.Parse(val);
        if (!typeGeneric.Name.ToLower().Equals("string") && typeGeneric.GetProperty("id") != null)
            array = RemoveIdsFromObject(array);

        return array;
    }

    private static JArray RemoveIdsFromObject(JArray value)
    {
        foreach (var jToken in value)
        {
            var v = (JObject) jToken;
            v.Remove("id");
        }

        return value;
    }

    private static void VerifyIsNgsiModel(Type type)
    {
        if (type.GetProperty("id") == null)
            throw new Exception(
                "Properties that have the attribute NGSIMapIds, or that are Objects must be NGSIBaseModel compliant object as well");
        if (!type.GetProperty("id").PropertyType.Name.ToLower().Equals("string"))
            throw new Exception("The id of an Object property must be a string");
    }

    public override bool Equals(object obj)
    {
        if (this.GetType() != obj.GetType())
            return false;
        foreach (var property in this.GetType().GetProperties())
        {
            try
            {
                var isIgnore = property.GetCustomAttributes().Contains(IgnoreAttr);
                if (isIgnore)
                    continue;
                if (property.GetValue(this) == null && property.GetValue(obj) != null)
                {
                    return false;
                }

                if ((property.PropertyType.IsGenericType &&
                     property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) ||
                    property.PropertyType.Name.Equals("JArray"))
                {
                    foreach (var value in (IEnumerable) property.GetValue(this))
                    {
                        if (!((IEnumerable<object>) property.GetValue(obj)).Contains(value))
                            return false;
                    }
                }
                else if (property.GetValue(this) != null)
                {
                    if (!property.GetValue(this).Equals(property.GetValue(obj)))
                        return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        return true;
    }
}