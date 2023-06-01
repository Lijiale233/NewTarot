using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OpenAI;
using System.IO;
using System;

[Serializable]

public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}

public class SaveManager : Singleton<SaveManager>
{
    string sceneName = "";

    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            loadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        SaveMessagesToJson();
    }

    public void loadPlayerData()
    {
        LoadMessagesFromJson();
    }


    /// <summary>
    /// 保存JSON数据到本地的方法
    /// </summary>
    /// <param name="player">要保存的对象</param>
    void SaveMessagesToJson()
    {
        Debug.Log(ChatGPTManager.Instance.messages[0].Content);
        // 将列表转换为JSON字符串
        string json = JsonUtility.ToJson(new Serialization<ChatMessage>(ChatGPTManager.Instance.messages), true);
        Debug.Log(json);

        // 定义要保存的文件路径
        string filePath = Path.Combine(Application.persistentDataPath, "messages.json");
        
        // 将JSON字符串写入文件
        File.WriteAllText(filePath, json);
        Debug.Log("Messages saved to JSON file: " + filePath);
    }

    void LoadMessagesFromJson()
    {
        // 定义要读取的文件路径
        string filePath = Path.Combine(Application.persistentDataPath, "messages.json");

        // 检查文件是否存在
        if (File.Exists(filePath))
        {
            // 从文件中读取JSON字符串
            string json = File.ReadAllText(filePath);

            // 将JSON字符串转换为消息列表
            Serialization<ChatMessage> messages = JsonUtility.FromJson<Serialization<ChatMessage>>(json);

            foreach (ChatMessage message in messages.ToList())
            {
                Debug.Log("Role: " + message.Role + ", Content: " + message.Content);
            }
            Debug.Log("Messages loaded from JSON file: " + filePath);
        }

        

    }
}


