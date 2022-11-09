using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileInterface
{
    static StreamReader SR;
    static StreamWriter SW;
    public static void LoadFile(string folderPath, string fileName, string fileType, bool isReader, bool createFile)
    {
        CloseFile();
        string path = @""+Application.dataPath + "/" + folderPath + "/" + fileName + "." + fileType;
        Debug.Log("Loading file at:\n" + path);
        if (!File.Exists(path))
        {
            Debug.Log("Path Does not Exist");
            if (createFile)
            {
                File.Create(path);
                Debug.Log("Creating new path");
            }
        }
        if (isReader)
        {
            Debug.Log("Creating SR");
            SR = new StreamReader(path);
        }
        else
        {
            Debug.Log("Creating SW");
            SW = new StreamWriter(path);
        }
    }
    public static string ReadLine(bool All)
    {
        if (SR == null) return string.Empty;
        if(SR.EndOfStream) return string.Empty;
        if (All)
            return SR.ReadToEnd();
        return SR.ReadLine();
    }
    public static void Write(string addition, bool newLine)
    {
        if (SW == null) return;
        Debug.Log("Writing:\n" + addition);
        if(newLine)
            SW.WriteLine(addition);
        else
            SW.Write(addition);
    }
    public static void CloseFile()
    {
        SR?.Close();
        SR = null;
        SW?.Close();
        SW = null;
    }
}
