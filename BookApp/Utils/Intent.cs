using System;
using System.Collections.Generic;
using BookApp.Enums;

namespace BookApp.Utils;

public class Intent
{
    #region Members

    private readonly Dictionary<IntentName, object> ContentDict;
    private readonly object Lock = new object();

    #endregion
    
    #region Properties

    public int ItemsCount
    {
        get
        {
            return ContentDict.Count;
        }
    }
    
    public string Id { get; private set; }
    
    #endregion
    
    #region Ctor

    public Intent()
        : this(new Dictionary<IntentName, object>())
    {
        
    }
    
    public Intent(Dictionary<IntentName, object> newContent)
    {
        if (newContent != null)
        {
            ContentDict = new Dictionary<IntentName, object>(newContent);
        }
        else
        {
            ContentDict = new Dictionary<IntentName, object>();
        }

        Id = Guid.NewGuid().ToString();
    }

    public Intent(Intent intent)
        : this(intent?.ContentDict)
    {
        
    }

    #endregion
    
    #region Methods

    public void AddValue(IntentName name, object value)
    {
        lock (Lock)
        {
            ContentDict[name] = value;
        }
    }

    public T GetValue<T>(IntentName name, T defaultValue = default(T))
    {
        try
        {
            if (ContentDict.ContainsKey(name))
            {
                object value = ContentDict[name];
                T ret;
                {
                    ret = (T)value;
                }

                return ret;
            }

            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    public void DeleteValue(IntentName name)
    {
        if (ContentDict.ContainsKey(name))
        {
            ContentDict.Remove(name);
        }
    }
    
    #endregion
}