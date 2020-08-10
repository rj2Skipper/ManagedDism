// Copyright (c). All rights reserved.
//
// Licensed under the MIT license.
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

// http://www.amazedsaint.com/2009/10/json-serialization-and-de-serialization.html

/// <summary>
/// public static class JsonSerializerHelper
/// </summary>
public static class JsonSerializerHelper
{
    /// <summary>
    /// Adds an extension method to a string
    /// </summary>
    /// <typeparam name="TObj">The expected type of Object</typeparam>
    /// <param name="json">Json string data</param>
    /// <param name="encoding">Encoding to use for desiralization</param>
    /// <returns>The deserialized object graph</returns>
    public static TObj JsonDeserialize<TObj>(this string json, Encoding encoding)
    {
        using (MemoryStream mstream = new MemoryStream(encoding.GetBytes(json)))
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(
                typeof(TObj),
                new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ssZ") });

            return (TObj)serializer.ReadObject(mstream);
        }
    }

    /// <summary>
    /// Adds an extension method to a string
    /// </summary>
    /// <typeparam name="TObj">The expected type of Object</typeparam>
    /// <param name="json">Json string data</param>
    /// <returns>The deserialized object graph</returns>
    /// <remarks>UTF8 Encoding is used</remarks>
    public static TObj JsonDeserialize<TObj>(this string json)
    {
        return JsonSerializerHelper.JsonDeserialize<TObj>(json, Encoding.UTF8);
    }

    /// <summary>
    /// Serialize the object to Json string
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <returns>Serialized string</returns>
    /// <remarks>UTF8 Encoding is used</remarks>
    public static string JsonSerialize(this object obj)
    {
        return JsonSerializerHelper.JsonSerialize(obj, Encoding.UTF8);
    }

    /// <summary>
    /// Serialize the object to Json string
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <param name="encoding">Encoding to use for serialization</param>
    /// <returns>Serialized string</returns>
    public static string JsonSerialize(this object obj, Encoding encoding)
    {
        using (MemoryStream mstream = new MemoryStream())
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(
                obj.GetType(),
                new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture) });
            serializer.WriteObject(mstream, obj);
            mstream.Position = 0;
            using (StreamReader reader = new StreamReader(mstream, encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Serialize the object to Json string
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <param name="indentLevel">indentation level, count of spaces</param>
    /// <param name="reflectionDepth">how deep to reflect through object</param>
    /// <returns>Serialized string</returns>
    public static string Object2String(this object obj, int indentLevel, int reflectionDepth)
    {
        if (reflectionDepth < 0)
        {
            return "<reflection depth exceeded>";
        }

        Type objType = obj.GetType();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string indent = new string(' ', indentLevel * 2);
        sb.AppendLine(obj.ToString());
        System.Reflection.PropertyInfo[] props = objType.GetProperties();
        foreach (System.Reflection.PropertyInfo prop in props)
        {
            try
            {
                if (prop.PropertyType.IsArray)
                {
                    try
                    {
                        sb.AppendLine(string.Format("{0}{1}[]: ", indent, prop.Name));
                        Array arr = (Array)prop.GetValue(obj, null);
                        foreach (object a in arr)
                        {
                            sb.AppendLine(a.Object2String(indentLevel + 1, reflectionDepth - 1));
                        }
                    }
                    catch
                    {
                    }
                }
                else if (prop.PropertyType.IsEnum)
                {
                    sb.AppendLine(string.Format("{0}{1}: {2}", indent, prop.Name, prop.GetValue(obj, null)));
                }
                else if (prop.PropertyType.IsValueType)
                {
                    sb.AppendLine(string.Format("{0}{1}: {2}", indent, prop.Name, prop.GetValue(obj, null)));
                }
                else if (!prop.PropertyType.IsGenericParameter)
                {
                    sb.AppendLine(string.Format("{0}{1}: {2}", indent, prop.Name, prop.GetValue(obj, null)));
                }
                else
                {
                    sb.Append(string.Format("{0}{1}: ", indent, prop.Name));
                    sb.AppendLine(prop.GetValue(obj, null).Object2String(indentLevel + 1, reflectionDepth - 1));
                }
            }
            catch
            {
            }
        }

        return sb.ToString();
    }
}