using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ILoader
{
    public void Load<T>(string key, Action<T> action);
}