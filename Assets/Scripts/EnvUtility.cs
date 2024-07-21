using CandyCoded.env;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnvUtility 
{
    public static string AskForKey(APIType type)
    {
        LoadEnv();
        if (type == APIType.OpenAI)
        {
            return System.Environment.GetEnvironmentVariable("OpenAI");
        }
        else if (type == APIType.Region)
        {
            return System.Environment.GetEnvironmentVariable("Region");
        }
        else if (type == APIType.Azure)
        {
            return System.Environment.GetEnvironmentVariable("AzureKey");
        }
        else
        {
            return null;
        }

    }
    public static void LoadEnv()
    {
        foreach(var dict in env.variables)
        {
            System.Environment.SetEnvironmentVariable(dict.Key, dict.Value);
        }
    }

}
