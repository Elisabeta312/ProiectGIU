using UnityEngine;

using System.Collections.Generic;

public static class CaveEntranceStateRegistry
{
    private static HashSet<string> openedEntrances = new HashSet<string>();

    public static void MarkOpened(string entranceId)
    {
        if (string.IsNullOrWhiteSpace(entranceId))
        {
            return;
        }

        openedEntrances.Add(entranceId);
    }

    public static bool IsOpened(string entranceId)
    {
        if (string.IsNullOrWhiteSpace(entranceId))
        {
            return false;
        }

        return openedEntrances.Contains(entranceId);
    }

    public static List<string> GetOpenedEntrances()
    {
        return new List<string>(openedEntrances);
    }

    public static void SetOpenedEntrances(List<string> entrances)
    {
        openedEntrances.Clear();

        if (entrances == null)
        {
            return;
        }

        foreach (string entranceId in entrances)
        {
            if (!string.IsNullOrWhiteSpace(entranceId))
            {
                openedEntrances.Add(entranceId);
            }
        }
    }

    public static void Clear()
    {
        openedEntrances.Clear();
    }
}