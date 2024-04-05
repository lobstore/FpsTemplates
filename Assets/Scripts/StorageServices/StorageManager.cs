using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class StorageManager
{
    #region Events
    public readonly UnityEvent OnSaved = new();
    public readonly UnityEvent OnLoad = new();
    #endregion

    private List<IDataPersistance> dataPersistances;
    private StorageService StorageService { get; set; }
    private GameData GameData;

    //bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";
    string saveName = "save";
    string saveKeyPrefix = "save_";
    public string FolderPath
    {
        get
        {
            return Application.persistentDataPath;
        }
    }

    public StorageManager(StorageService storageService, List<IDataPersistance> dataPersistances)
    {
        StorageService = storageService;
        GameData = new GameData();
        this.dataPersistances = dataPersistances;
    }

    public void SaveGameData()
    {
        string key;
        key = BuildPathWithoutExtension(saveName);
        foreach (var data in dataPersistances)
        {
            data.SaveData(GameData);
        }
        StorageService.Save(key, GameData);
        Debug.Log($"saved to {FolderPath}");
        OnSaved.Invoke();
    }

    public void LoadGameData()
    {
        string key;
        key = BuildPathWithoutExtension(saveName);
        StorageService.Load<GameData>(key, e => GameData = e);
        foreach (var data in dataPersistances)
        {
            data.LoadData(GameData);
        }
        OnLoad.Invoke();
    }

    #region Methods For Generating New SaveName
    private string GenerateNewFileName()
    {
        if (Directory.Exists(FolderPath))
        {
            // Получение списка файлов в папке
            string[] files = Directory.GetFiles(FolderPath);

            // Фильтрация только файлов с заданным форматом имени
            var numberedFiles = files.Where(file => IsNumberedFile(file)).ToList();

            if (numberedFiles.Any())
            {
                // Нахождение максимального номера среди существующих файлов
                int maxNumber = numberedFiles.Max(file => GetFileNumber(file));

                // Увеличение номера для нового файла
                int newNumber = maxNumber + 1;

                // Формирование нового имени файла
                string newFileName = saveKeyPrefix + newNumber;
                return BuildPathWithoutExtension(newFileName);
            }
            else
            {
                // Если файлов нет, начнем с 1
                return BuildPathWithoutExtension(saveKeyPrefix + "1");
            }
        }
        else
        {
            Console.WriteLine("Указанная папка не существует.");
            return null;
        }
    }
    private string GetLatestFile()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(FolderPath);

        if (!directoryInfo.Exists)
        {
            throw new DirectoryNotFoundException($"Directory not found: {FolderPath}");
        }

        FileInfo latestFile = null;
        DateTime latestCreationTime = DateTime.MinValue;

        foreach (FileInfo fileInfo in directoryInfo.GetFiles())
        {
            if (fileInfo.CreationTime > latestCreationTime)
            {
                latestFile = fileInfo;
                latestCreationTime = fileInfo.CreationTime;
            }
        }

        return Path.GetFileNameWithoutExtension(latestFile.FullName);
    }
    private string BuildPathWithoutExtension(string key)
    {
        return Path.Combine(FolderPath, key);
    }
    private bool IsNumberedFile(string file)
    {
        // Проверка, что файл имеет нужный формат имени
        return Path.GetFileName(file).StartsWith(saveKeyPrefix);
    }

    private int GetFileNumber(string file)
    {
        // Извлечение номера из имени файла
        string fileName = Path.GetFileNameWithoutExtension(file);
        return int.Parse(fileName.Substring(saveKeyPrefix.Length));
    }
    #endregion

    #region Encryption
    private string Encrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
    #endregion
    public void RegisterPersistent(IDataPersistance dataPersistance)
    {
        dataPersistances.Add(dataPersistance);
    }

    public void UnregisterPersistent(IDataPersistance dataPersistance)
    {
        dataPersistances.Remove(dataPersistance);
    }

}
