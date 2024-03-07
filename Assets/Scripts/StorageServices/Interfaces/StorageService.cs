using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class StorageService : ISaver, ILoader
{
    abstract public void Load<T>(string key, Action<T> action);
    abstract public void Save(string key,object obj, Action<bool> action = null);
}