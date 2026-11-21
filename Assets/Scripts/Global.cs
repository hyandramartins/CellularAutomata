using System;
using UnityEngine;

public static class Global
{
    public static string GenerateID()
    {
        string id = Guid.NewGuid().ToString();

        return id;
    }
}
