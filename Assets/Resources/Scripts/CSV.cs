using UnityEngine;
using Unity;
using System.IO;
using System.Collections.Generic;

public class CSV
{
    public List<string[]> m_ArrayData;
    public CSV() { 
        m_ArrayData = new List<string[]>(); 
    }
    public string GetString(int row, int col)
    {
        return m_ArrayData[row][col];
    }

    public void LoadFile(string path)
    {
        m_ArrayData.Clear();
        StreamReader sr = null;
        Debug.Log("try to find "+path +"!");
        try
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            sr = new StreamReader(fs, System.Text.Encoding.Default);
            Debug.Log(path+" finded!");
        }
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
            return;
        }
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            m_ArrayData.Add(line.Split(','));
        }
        sr.Close();
        sr.Dispose();
    }
}
