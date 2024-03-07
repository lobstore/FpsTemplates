using Newtonsoft.Json;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class JSONStorageService : StorageService
{
    public override void Load<T>(string key, Action<T> action)
    {
        using (var fileStream = new StreamReader(key + ".json")) 
        {
            var dataJson = fileStream.ReadToEnd();
            var data = JsonConvert.DeserializeObject<T>(dataJson);
            action.Invoke(data);
        }
    }

    public override void Save(string key, object obj, Action<bool> action = null)
    {
        using (var fileStream = new StreamWriter(key + ".json"))
        {
            fileStream.Write(GetJsonFromClass(obj));
        }
        action?.Invoke(true);
    }

    private string GetJsonFromClass(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }

}