using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public static class JsonHelper
{
    private class Vector3Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector3 vector = (Vector3)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(vector.x);
            writer.WritePropertyName("y");
            writer.WriteValue(vector.y);
            writer.WritePropertyName("z");
            writer.WriteValue(vector.z);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = Newtonsoft.Json.Linq.JObject.Load(reader);
            return new Vector3((float)obj["x"], (float)obj["y"], (float)obj["z"]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }
    }
    
    public static readonly JsonSerializerSettings Settings = new()
    {
        Converters = { new Vector3Converter() },
        NullValueHandling = NullValueHandling.Ignore
    };
    
    
    public static readonly YamlDotNet.Serialization.IDeserializer yamlDeserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
    public static readonly YamlDotNet.Serialization.ISerializer yamlSerializer =
        new YamlDotNet.Serialization.SerializerBuilder().Build();
        
    public static string YamlToJson(string yaml)
    {
        var yamlObject = yamlDeserializer.Deserialize<object>(yaml);
        return JsonConvert.SerializeObject(yamlObject);
    }

    public static string JsonToYaml(string json)
    {
        var jsonToken = JToken.Parse(json);
        var jsonObject = jsonToken.Type switch
        {
            JTokenType.Object => jsonToken.ToObject<object>(),
            JTokenType.Array => jsonToken.ToObject<List<object>>(),
            _ => throw new InvalidOperationException("Invalid JSON token type.")
        };
        return yamlSerializer.Serialize(jsonObject);
    }

    public static T ReadFile<T>(string path)
    {
        var content = File.ReadAllText(path);
        var fileExtension = Path.GetExtension(path);
        string jsonContent = "";
        if (fileExtension == ".json")
        {
            jsonContent = content;
        }
        else if (fileExtension == ".yaml" || fileExtension == ".yml")
        {
            jsonContent = YamlToJson(content);
        }
        else
        {
            Assert.IsFalse(true);
        }
        return JsonConvert.DeserializeObject<T>(jsonContent, Settings);
    }

    public static void WriteFile<T>(string path, T obj)
    {
        var jsonContent = JsonConvert.SerializeObject(obj, Settings);
        File.WriteAllText(path, jsonContent);
    }

    public static string PrettyString<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}