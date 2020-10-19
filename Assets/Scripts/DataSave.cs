using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public int m_winValue;
    public int m_loseValue;

    public PlayerData(){ }
    public PlayerData(int winValue,int loseValue)
    {
        m_winValue = winValue;
        m_loseValue = loseValue;
    }
    public static PlayerData operator +(PlayerData left, PlayerData right)
        => new PlayerData(left.m_winValue + right.m_winValue, left.m_loseValue + right.m_loseValue); 
}

public class DataSave : MonoBehaviour
{

    public static string filePath = Application.persistentDataPath + "/" + ".player.json";
    public static void PlayerDataSave(GameSetStatus status)
    {
        PlayerData saveData = PlayerDataLode();
        if (status == GameSetStatus.Win)
        {
            saveData.m_winValue += 1;
        }
        else
        {
            saveData.m_loseValue += 1;
        }

        string json = JsonUtility.ToJson(saveData);

        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }

    public static PlayerData PlayerDataLode()
    {
        PlayerData data = new PlayerData(0, 0);
        if (File.Exists(filePath))
        {
            StreamReader streamReader = new StreamReader(filePath);
            string lodeData = streamReader.ReadToEnd();
            data = JsonUtility.FromJson<PlayerData>(lodeData);
            streamReader.Close();
        }
        return data;
    }
}

public enum GameSetStatus
{ 
    Win,
    Lose
}