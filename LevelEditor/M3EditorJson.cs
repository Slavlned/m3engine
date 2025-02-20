using System.Collections.Generic;
using System.IO;
using UnityEngine;

// json утилиты для редактора
public class M3EditorJson {

    // Сохранение уровня в файл
    public void Save(M3Level level, string fileName)
    {
        // путь
        string savePath = Path.Combine(Application.persistentDataPath, fileName);
        // сериализованный уровень
        string serialized = JsonUtility.ToJson(level);

        // записываем текст
        File.WriteAllText(savePath, serialized);
    }

    // Загружаем уровень из файла
    public M3Level Load(string fileName)
    {
        // путь к файлу
        string savePath = Path.Combine(Application.persistentDataPath, fileName);
        // десереализуем уровень и возвращаем его
        return JsonUtility.FromJson<M3Level>(File.ReadAllText(savePath));
    }

    // Получаем список уровней
    public List<string> GetFileNames()
    {
        // получаем путь даты
        string savePath = Application.persistentDataPath;

        // получаем инфу о дериктории
        DirectoryInfo info = new DirectoryInfo(savePath);
        var fileInfo = info.GetFiles();

        // получаем имена
        List<string> names = new List<string>();
        for (int f = 0; f < fileInfo.Length; f++) { names.Add(fileInfo[f].Name); }

        // возвращаем
        return names;
    }
}