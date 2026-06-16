using UnityEngine;

using System.Collections.Generic;

public static class PlayerKeyInventory
{
    private static HashSet<string> acquiredKeys = new HashSet<string>();

    public static void AddKey(string keyId)
    {
        if (string.IsNullOrWhiteSpace(keyId))
        {
            return;
        }

        acquiredKeys.Add(keyId);
    }

    public static bool HasKey(string keyId)
    {
        if (string.IsNullOrWhiteSpace(keyId))
        {
            return false;
        }

        return acquiredKeys.Contains(keyId);
    }

    public static List<string> GetAcquiredKeys()
{
    return new List<string>(acquiredKeys);
}

public static void SetAcquiredKeys(List<string> keys)
{
    acquiredKeys.Clear();

    if (keys == null)
    {
        return;
    }

    foreach (string keyId in keys)
    {
        if (!string.IsNullOrWhiteSpace(keyId))
        {
            acquiredKeys.Add(keyId);
        }
    }
}

public static void Clear()
{
    acquiredKeys.Clear();
}
}