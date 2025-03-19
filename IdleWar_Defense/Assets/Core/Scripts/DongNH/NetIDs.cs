using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetIDs
{
    public static string[] IDs
    {
        get
        {
            string filePath = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
            filePath = filePath.Replace(".cs", ".txt");
            string text = File.ReadAllText(filePath);
            char[] delims = new[] { '\r', '\n' };
            string[] output = text.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            List<string> _output = new List<string>();
            foreach (var item in output)
            {
                if (!_output.Contains(item.Trim().ToLower()))
                    _output.Add(item.Trim().ToLower());
                else
                    Debug.LogError("XXX " + item.Trim());
            }
            Debug.Log("get " + _output.Count + " network");
            return _output.ToArray();
        }
    }
}
