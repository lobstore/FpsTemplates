
using System;
public interface ISaver
{
    public void Save(string key, object obj, Action<bool> action = null);
}
