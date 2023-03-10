using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPrefab 
{
    public GameObject Prefab;
    public string Path;

    public NetworkPrefab(GameObject obj, string path)
    {
        Prefab = obj;
        Path = ReturnPrefabPathModified(path);
        //Assets/Resources/File.prefab  // 7ext length. 7 on resources
        //we want...Resources/File
    } 

    private string ReturnPrefabPathModified(string path)
    {
        int extensionLength = System.IO.Path.GetExtension(path).Length;
        int additionalLength = 10;
        int startIndex = path.ToLower().IndexOf("resources");

        if (startIndex == -1)
            return string.Empty;
        else
            return path.Substring(startIndex + additionalLength, path.Length - (additionalLength + startIndex + extensionLength));
        
    }
}
