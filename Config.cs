using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using Godot;

public class Config
{
    private Dictionary<string, object> _configValues;

    private List<string> _requiredKeys;

    public Config(List<string> requiredKeys)
    {
        _configValues = new Dictionary<string, object>();
        _requiredKeys = requiredKeys;
    }

    public void LoadValuesFromFile(string path, char separator = ':')
    {
        string data = File.ReadAllText(path);

        string[] lines = data.Split('\n');

        foreach (string line in lines)
        {
            string[] pair = line.Split(separator);
            _configValues.Add(pair[0], pair[1]);

            GD.Print(pair[0] + " => " + pair[1]);

            _requiredKeys.Remove(pair[0]);
        }

        foreach (string key in _requiredKeys)
        {
            _configValues.Add(key, Main.Rand.Next(0, int.MaxValue));
        }
    }

    public bool ContainsKey(string key) =>
        _configValues.ContainsKey(key);

    public T GetValueAs<T>(string key) =>
        (T)Convert.ChangeType(_configValues[key], typeof(T));
}