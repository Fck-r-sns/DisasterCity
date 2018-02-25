using UnityEngine;

public static class DebugUtilities
{
    public static void Print(params object[] args)
    {
        string text = Time.frameCount.ToString();
        foreach (var arg in args)
            text += " " + arg.ToString();
        Debug.Log(text);
    }
}
