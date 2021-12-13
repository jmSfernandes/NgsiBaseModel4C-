using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json.Linq;

namespace NGSIBaseModel.Models
{
    public class NgsiBaseModel
    {
        public static T FromNgsi<T>(JToken entity)
        {
            if (entity.Contains("type"))
            {
                if (!typeof(T).Name.ToLower().Equals(entity["type"].ToString()))
                {
                    throw new Exception("The type of NGSI Entity must be the same as the class");
                }
            }

            T obj = Activator.CreateInstance<T>();
            foreach (var property in typeof(T).GetProperties())
            {
                var isIgnore = property.GetCustomAttributes().Contains(new NGSIIgnore());
                var isEncoded = property.GetCustomAttributes().Contains(new NGSIEncode());
                var isJObject = property.GetCustomAttributes().Contains(new NGSIJObject());
                var isJArray = property.GetCustomAttributes().Contains(new NGSIJArray());

                JToken attribute = entity[property.Name.ToLower()];
                if (attribute != null && !isIgnore)
                {
                    if (attribute.Type == JTokenType.Object && attribute["value"] != null)
                    {
                        JToken value = attribute["value"];
                        obj = SetValue<T>(obj, value, property, isEncoded, isJObject, isJArray);
                    }
                    else
                    {
                        obj = SetValue<T>(obj, attribute, property, isEncoded, isJObject, isJArray);
                    }
                }
            }

            return obj;
        }

        private static T SetValue<T>(Object obj, JToken value, PropertyInfo property, bool isEncoded, bool isJObject,
            bool isJArray)
        {
            if (value != null)
            {
                if (value.Type == JTokenType.String)
                {
                    if (isEncoded)
                        value = DecodeAttribute(value, property);

                    property.SetValue(obj, value.ToString());
                    /*if (property.Name.ToLower().Equals("picture"))
                        property.SetValue(obj, setPicture(value.ToString()));
                    else*/
                }
                else if (value.Type == JTokenType.Date)
                {
                    DateTime dateTime = StringToDatetime(value.ToString());
                    if (property.GetCustomAttributes().Contains(new NGSIDateTime()))
                        property.SetValue(obj, dateTime);
                    else
                        property.SetValue(obj, DatetimeToString(dateTime));
                }
                else if (value.Type == JTokenType.Integer)
                {
                    property.SetValue(obj, Int32.Parse(value.ToString()));
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
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var itemType = type.GetGenericArguments()[0]; // use this...
                            var array = Activator.CreateInstance(type);
                            var name = itemType.Name;
                            foreach (JToken v in value)
                            {
                                var val = v;
                                if (isEncoded)
                                    val = DecodeAttribute(val, property);

                                if (itemType.Name.Equals("String"))
                                {
                                    array.GetType().GetMethod("Add").Invoke(array, new[] {val.ToString()});
                                }
                                else
                                {
                                    Object obj_sub = Activator.CreateInstance(itemType);
                                    if (itemType.BaseType != null && itemType.BaseType == typeof(NgsiBaseModel))
                                    {
                                        obj_sub = typeof(NgsiBaseModel).GetMethod("FromNgsi")
                                            .MakeGenericMethod(itemType)
                                            .Invoke(typeof(NgsiBaseModel), new object[] {val});
                                    }
                                    else
                                    {
                                        obj_sub.GetType().GetProperty("id").SetValue(obj_sub, val.ToString());
                                    }

                                    array.GetType().GetMethod("Add").Invoke(array, new object[] {obj_sub});
                                }
                            }

                            property.SetValue(obj, array);
                        }
                    }
                }
                else if (value.Type == JTokenType.Object)
                {
                    if (isJObject)
                        property.SetValue(obj, value.ToString());
                    else
                        obj = SetObjectProperty(obj, value, property, isEncoded);
                }
            }

            return (T) obj;
        }

        private static object SetObjectProperty(object obj, JToken value, PropertyInfo property, bool isEncoded)
        {
            if (isEncoded)
                value = DecodeAttribute(value, property);
            Type objType = property.PropertyType;
            Object obj_sub = Activator.CreateInstance(objType);
            if (objType.BaseType != null && objType.BaseType == typeof(NgsiBaseModel))
            {
                obj_sub = typeof(NgsiBaseModel).GetMethod("FromNgsi").MakeGenericMethod(objType)
                    .Invoke(obj, new object[] {value});
                property.SetValue(obj, obj_sub);
            }

            return obj;
        }

        public static JObject ToNgsi<T>(Object obj)
        {
            string _id = null;
            if (!typeof(T).Name.ToLower().Equals(obj.GetType().Name.ToLower()))
            {
                throw new Exception("The type of NGSI Entity must be the same as the class");
            }

            if (obj.GetType().GetProperty("id") == null)
            {
                throw new Exception(
                    "The model is not compliant with the NGSIBaseModel it must implement an id property");
            }
            else
            {
                _id = (string) typeof(T).GetProperty("id").GetValue(obj);
            }

            JObject json = new JObject();
            json.Add("id", _id);
            json.Add("type", typeof(T).Name.ToLower());

            foreach (var property in typeof(T).GetProperties())
            {
                var isIgnore = property.GetCustomAttributes().Contains(new NGSIIgnore());
                var isEncoded = property.GetCustomAttributes().Contains(new NGSIEncode());
                var isJObject = property.GetCustomAttributes().Contains(new NGSIJObject());
                var isJArray = property.GetCustomAttributes().Contains(new NGSIJArray());
                var isDate = property.GetCustomAttributes().Contains(new NGSIDateTime());
                if (!isIgnore && (property.Name != "id"))
                {
                    var value = obj.GetType().GetProperty(property.Name).GetValue(obj);
                    var propertyType = property.PropertyType;
                    if (value != null)
                    {
                        if (isJObject)
                            value = JObject.Parse((string) value);
                        else
                            isJObject = propertyType.Name.ToLower().Equals("jobject");
                        if (isJArray)
                            value = JArray.Parse((string) value);
                        else
                            isJArray = propertyType.Name.ToLower().Equals("jarray");


                        if (propertyType.IsClass && (isJArray || isJObject))
                        {
                            var attrJSON = SetJsonAttributeComplex(value, property, "object", isEncoded);
                            json.Add(property.Name.ToLower(), attrJSON);
                        }

                        else if (propertyType.IsAnsiClass && isDate)
                        {
                            var attrJSON = SetJsonAttributeComplex(value, property, "date", isEncoded);
                            json.Add(property.Name.ToLower(), attrJSON);
                        }
                        else if (propertyType.IsGenericType &&
                                 propertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var attrJSON = SetJsonAttributeComplex(value, property, "array", isEncoded);
                            json.Add(property.Name.ToLower(), attrJSON);
                        }
                        else if (propertyType.IsClass && !propertyType.Name.ToLower().Equals("string"))
                        {
                            var attrJSON = SetJsonAttributeComplex(value, property, "object", isEncoded);
                            json.Add(property.Name.ToLower(), attrJSON);
                        }
                        else
                        {
                            var attrJSON = SetJsonAttributeSimple(value, property, isEncoded);
                            json.Add(property.Name.ToLower(), attrJSON);
                        }
                    }
                }
            }

            return json;
        }


        private static JToken SetJsonAttributeSimple(Object value, PropertyInfo property, bool isEncoded)
        {
            JObject attrObj = new JObject();
            if (isEncoded)
                value = EncodeAttribute(value.ToString(), property);

            switch (JToken.FromObject(value).Type)
            {
                case (JTokenType.String):
                    attrObj.Add("value", value.ToString());
                    if (IsDatetime(value.ToString()))
                        attrObj.Add("type", "ISO8601");
                    else
                        attrObj.Add("type", "Text");
                    break;
                case (JTokenType.Date):
                    attrObj.Add("value", $"{value.ToString():yyyy-MM-dd HH:mm:ss}");
                    attrObj.Add("type", "ISO8601");
                    break;
                case (JTokenType.Integer):
                    attrObj.Add("value", (Int32) value);
                    attrObj.Add("type", "Integer");
                    break;
                case (JTokenType.Float):
                    attrObj.Add("value", (float) value);
                    attrObj.Add("type", "Float");
                    break;
                default:
                    break;
            }

            return attrObj;
        }


        private static JToken SetJsonAttributeComplex(Object value, PropertyInfo property, String type, bool isEncoded)
        {
            JObject attrObj = new JObject();

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
                        if (value.GetType().GetProperty("id") == null)
                            throw new Exception(
                                "Properties of the type Objects must be a NGSIBaseModel compliant object as well");
                        else if (!value.GetType().GetProperty("id").PropertyType.Name.ToLower().Equals("string"))
                            throw new Exception("The id of an Object property must be a string");
                        string val = (string) value.GetType().GetProperty("id").GetValue(value);
                        if (isEncoded)
                            val = EncodeAttribute(val, property);
                        attrObj.Add("value", val);
                        attrObj.Add("type", "Text");
                    }

                    break;
                case ("array"):
                    JArray array = new JArray();
                    var type_generic = property.PropertyType.GetGenericArguments()[0];
                    if (!type_generic.Name.ToLower().Equals("string"))
                    {
                        if (type_generic.GetProperty("id") == null)
                            throw new Exception(
                                "Properties of the type Objects must be a NGSIBaseModel compliant object as well");
                        else if (!type_generic.GetProperty("id").PropertyType.Name.ToLower().Equals("string"))
                            throw new Exception("The id of an Object property must be a string");
                    }

                    foreach (var v in (IEnumerable) value)
                    {
                        string val_v;
                        if (type_generic.Name.ToLower().Equals("string"))
                            val_v = (string) v;
                        else
                            val_v = (string) v.GetType().GetProperty("id").GetValue(v);
                        if (isEncoded)
                            val_v = EncodeAttribute(val_v, property);
                        array.Add(val_v);
                    }

                    attrObj.Add("value", array);
                    attrObj.Add("type", "Text");
                    break;
                case ("date"):
                    attrObj.Add("value", DatetimeToString((DateTime) value));
                    attrObj.Add("type", "ISO8601");
                    break;
                default:
                    break;
            }


            return attrObj;
        }


        private static JToken DecodeAttribute(JToken value, PropertyInfo property)
        {
            var decoded = HttpUtility.UrlDecode(value.ToString());
            return (JToken) new JValue(decoded);
        }

        private static string EncodeAttribute(string value, PropertyInfo property)
        {
            var encoded = HttpUtility.UrlEncode(value.ToString());
            encoded = encoded.Replace("(", "%28").Replace(")", "%29").Replace("+", "%20");
            return encoded;
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;
            foreach (var property in this.GetType().GetProperties())
            {
                try
                {
                    var isIgnore = property.GetCustomAttributes().Contains(new NGSIIgnore());
                    if (property.PropertyType.IsGenericType &&
                        property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        foreach (var value in (IEnumerable) property.GetValue(this))
                        {
                            if (!((IEnumerable<object>) property.GetValue(obj)).Contains(value))
                                return false;
                        }
                    }
                    else if (!property.GetValue(this).Equals(property.GetValue(obj)) && !isIgnore)
                        return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    return false;
                }
            }

            return true;
        }

        public static string DatetimeToString(DateTime value)
        {
            return $"{value.ToUniversalTime():yyyy-MM-dd'T'HH:mm:ss'Z'}";
        }

        public static DateTime StringToDatetime(string value)
        {
            return DateTime.Parse(
                $"{value:yyyy-MM-dd HH:mm:ss}", CultureInfo.InvariantCulture).ToUniversalTime();
        }

        public static bool IsDatetime(string value)
        {
            try
            {
                StringToDatetime(value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}