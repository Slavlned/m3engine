using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// сервис сохранений
public class StorageService : MonoBehaviour, Service
{
    public static StorageService Instance;
    public static StorageService GetInstance() => Instance;

    public void Intialize(Service from)
    {
        // чекаем инстанс на нулл
        if (Instance == null)
        {
            // запрещаем уничтожения при загрузке сцены
            DontDestroyOnLoad(this.gameObject);
            // синглтон
            Instance = this;
        }
        // эксцепшен
        else
        {
            // уничтожаем
            Destroy(this.gameObject);
        }
    }

    public void Save<T>(string key, T value)
    {
        if (value is string || value is bool)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }
        else if (value is float)
        {
            PlayerPrefs.SetFloat(key, (float)(object)value);
        }
        else if (value is int)
        {
            PlayerPrefs.SetInt(key, (int)(object)value);
        }
        else if (value is long)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }        
        else
        {
            // json
            PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
        }
    }

    public object Get<T>(string key)
    {
        if (typeof(T) == typeof(float))
        {
            return PlayerPrefs.GetFloat(key);
        }
        else if (typeof(T) == typeof(int))
        {
            return PlayerPrefs.GetInt(key);
        }
        else if (typeof(T) == typeof(bool))
        {
            if (PlayerPrefs.HasKey(key))
            {
                return (bool.Parse(PlayerPrefs.GetString(key)));
            }
            else
            {
                return false;
            }
        }
        else if (typeof(T) == typeof(string))
        {
            return PlayerPrefs.GetString(key);
        }
        else if (typeof(T) == typeof(long))
        {
            if (PlayerPrefs.HasKey(key))
            {
                return long.Parse(PlayerPrefs.GetString(key));
            }
            else
            {
                return 0L;
            }
        }         
        else
        {
            // json
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
        }
    }

    public List<T> GetList<T>(string key)
    {
        if (typeof(T) == typeof(float))
        {
            if (PlayerPrefs.GetString(key) == "") { return new List<T>(); }
            var list = PlayerPrefs.GetString(key).Split(",");
            var result = new List<float>();

            foreach (string s in list)
            {
                if (s != "")
                {
                    result.Add(float.Parse(s));
                }
            }

            return result as List<T>;
        }
        else if (typeof(T) == typeof(int))
        {
            if (PlayerPrefs.GetString(key) == "") { return new List<T>(); }
            var list = PlayerPrefs.GetString(key).Split(",");
            var result = new List<int>();

            foreach (string s in list)
            {
                if (s != "")
                {
                    result.Add(int.Parse(s));
                }
            }

            return result as List<T>;
        }
        else if (typeof(T) == typeof(bool))
        {
            if (PlayerPrefs.GetString(key) == "") { return new List<T>(); }
            var list = PlayerPrefs.GetString(key).Split(",");
            var result = new List<bool>();

            foreach (string s in list)
            {
                if (s != "")
                {
                    result.Add(bool.Parse(s));
                }
            }

            return result as List<T>;
        }
        else if (typeof(T) == typeof(string))
        {
            if (PlayerPrefs.GetString(key) == "") { return new List<T>(); }
            return PlayerPrefs.GetString(key).Split(",").ToList() as List<T>;
        }
        else
        {
            throw new Exception(
                "Incorrect Type! " +
                "Available Types For Storage Service Load (int, bool, string, float) " +
                "Use BufferedStorageService instead"
            );
        }
    }

    public void SaveList<T>(string key, List<T> value)
    {
        // конвертируем в строку
        StringBuilder builder = new StringBuilder();

        foreach (T item in value)
        {
            builder.Append(item.ToString() + ",");
        }
        
        // сохраняем
        PlayerPrefs.SetString(key, builder.ToString());
    }
}