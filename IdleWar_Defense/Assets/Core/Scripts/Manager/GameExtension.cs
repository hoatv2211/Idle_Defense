using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameExtension
{
    public static void QuitGame()
    {
#if UNITY_EDITOR
        return;
#endif

        try
        {
            var pt = System.Diagnostics.Process.GetCurrentProcess().Threads;
            foreach (ProcessThread p in pt)
            {
                p.Dispose();
            }
        }
        catch (System.Exception ex)
        {
        }
        try
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        catch (System.Exception ex)
        {
        }
        Application.Quit();
    }

}
