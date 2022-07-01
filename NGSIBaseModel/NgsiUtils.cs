using System;
using System.Web;
using Newtonsoft.Json.Linq;

namespace NGSIBaseModel;

public static class NgsiUtils
{
    public static JToken DecodeAttribute(JToken value)
    {
        var decoded = HttpUtility.UrlDecode(value.ToString());
        return (JToken) new JValue(decoded);
    }

    public static string EncodeAttribute(string value)
    {
        var encoded = HttpUtility.UrlEncode(value);
        encoded = encoded.Replace("(", "%28").Replace(")", "%29").Replace("+", "%20");
        return encoded;
    }

    public static string DatetimeToString(DateTime value)
    {
        return $"{value.ToUniversalTime():yyyy-MM-dd'T'HH:mm:ss'Z'}";
    }

    public static DateTime StringToDatetime(string value)
    {
        return DateTime.Parse($"{value:yyyy-MM-dd'T'HH:mm:ss'Z'}");
    }

    public static bool IsDatetime(string value)
    {
        try
        {
            StringToDatetime(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}