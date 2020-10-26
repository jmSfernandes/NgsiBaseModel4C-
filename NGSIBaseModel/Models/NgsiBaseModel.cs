using System;
using System.Collections;
using System.Collections.Generic;
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

                JToken attribute = entity[property.Name.ToLower()];
                if (attribute != null && !isIgnore)
                {
                    if (attribute.Type == JTokenType.Object && attribute["value"] != null)
                    {
                        JToken value = attribute["value"];
                        obj = SetValue<T>(obj, value, property, isEncoded);
                    }
                    else
                    {
                        obj = SetValue<T>(obj, attribute, property, isEncoded);
                    }
                }
            }

            return obj;
        }

        private static T SetValue<T>(Object obj, JToken value, PropertyInfo property, bool isEncoded)
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
                    DateTime dateTime= DateTime.Parse(value.ToString());
                    property.SetValue(obj, dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
                }
                else if (value.Type == JTokenType.Integer)
                {
                    property.SetValue(obj, Int32.Parse(value.ToString()));
                }
                else if (value.Type == JTokenType.Float && property.PropertyType==typeof(float))
                {
                    property.SetValue(obj,float.Parse(value.ToString()));
                }
                else if (value.Type == JTokenType.Float)
                {
                    property.SetValue(obj,double.Parse(value.ToString()));
                }
                else if (value.Type == JTokenType.Array)
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
                                    obj_sub = typeof(NgsiBaseModel).GetMethod("FromNgsi").MakeGenericMethod(itemType)
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
                else if (value.Type == JTokenType.Object)
                {
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
           
            Type mType = typeof(T);
            JObject json = new JObject();

            if (mType.GetProperty("id") != null)
            {
                json.Add("id",mType.GetProperty("id").GetValue(obj).ToString());
                json.Add("type", mType.Name.ToLower());
            }
            

           
            foreach (var property in typeof(T).GetProperties())
            {
                var isIgnore = property.GetCustomAttributes().Contains(new NGSIIgnore());
                var isEncoded = property.GetCustomAttributes().Contains(new NGSIEncode());
                if (!isIgnore && (property.Name != "id"))
                {
                    var value = obj.GetType().GetProperty(property.Name).GetValue(obj);
                    var property_type = property.PropertyType;
                    if (value != null)
                    {
                        if (property_type.IsGenericType && property_type.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var attrJSON = SetJsonAttributeComplex(value, property, "array", isEncoded);
                            json.Add(property.Name.ToLower(), attrJSON);
                        }
                        else if (property_type.IsClass && !property_type.Name.ToLower().Equals("string"))
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
            var value_json = JToken.Parse("{\"value\":\"" + value.ToString() + "\"}");
            var type = value_json["value"].Type;
            switch (type)
            {
                case (JTokenType.String):
                    attrObj.Add("value", value.ToString());
                    attrObj.Add("type", "Text");
                    break;
                case (JTokenType.Date):
                    attrObj.Add("value", $"{value.ToString():yyyy-MM-dd HH:mm:ss}");
                    attrObj.Add("type", "ISO8601");
                    break;
                case (JTokenType.Integer):
                    attrObj.Add("value", value.ToString());
                    attrObj.Add("type", "Integer");
                    break;
                case (JTokenType.Float):
                    attrObj.Add("value", value.ToString());
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
                    if (property.GetCustomAttributes().Contains(new NGSIMapIds()))
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
                    else
                    {
                        attrObj.Add("value", JObject.FromObject(value));
                        attrObj.Add("type", "Text");
                    }

                    break;
                case ("array"):
                    JArray array = new JArray();
                    var typeGeneric = property.PropertyType.GetGenericArguments()[0];
                    
                    if (property.GetCustomAttributes().Contains(new NGSIMapIds()))
                    {
                        if (typeGeneric.GetProperty("id") == null)
                            throw new Exception(
                                "Properties of the type Objects must be a NGSIBaseModel compliant object as well");
                        else if (!typeGeneric.GetProperty("id").PropertyType.Name.ToLower().Equals("string"))
                            throw new Exception("The id of an Object property must be a string");
                        foreach (var v in (IEnumerable) value)
                        {
                            string val_v = (string) v.GetType().GetProperty("id").GetValue(v);
                            if (isEncoded)
                                val_v = EncodeAttribute(val_v, property);
                            array.Add(val_v);
                        }
                    }
                    foreach (var v in (IEnumerable) value)
                    {
                        if(typeGeneric.Name.ToLower().Equals("string"))
                            array.Add(v);
                        else
                            array.Add( JObject.FromObject(v));
                    }
                   

                    attrObj.Add("value", array);
                    attrObj.Add("type", "Text");
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
            if(this.GetType() != obj.GetType())
                return false;
            foreach (var property in this.GetType().GetProperties()) {
                try {
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
                    else if(!property.GetValue(this).Equals(property.GetValue(obj))&& !isIgnore)
                        return false;

                } catch (Exception e) {
                    Console.WriteLine(e.StackTrace);
                    return false;
                }
            }
            return true;
            
        }
    }
}