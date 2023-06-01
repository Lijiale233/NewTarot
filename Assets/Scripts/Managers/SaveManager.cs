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
    /// ����JSON���ݵ����صķ���
    /// </summary>
    /// <param name="player">Ҫ����Ķ���</param>
    void SaveMessagesToJson()
    {
        Debug.Log(ChatGPTManager.Instance.messages[0].Content);
        // ���б�ת��ΪJSON�ַ���
        string json = JsonUtility.ToJson(new Serialization<ChatMessage>(ChatGPTManager.Instance.messages), true);
        Debug.Log(json);

        // ����Ҫ������ļ�·��
        string filePath = Path.Combine(Application.persistentDataPath, "messages.json");
        
        // ��JSON�ַ���д���ļ�
        File.WriteAllText(filePath, json);
        Debug.Log("Messages saved to JSON file: " + filePath);
    }

    void LoadMessagesFromJson()
    {
        // ����Ҫ��ȡ���ļ�·��
        string filePath = Path.Combine(Application.persistentDataPath, "messages.json");

        // ����ļ��Ƿ����
        if (File.Exists(filePath))
        {
            // ���ļ��ж�ȡJSON�ַ���
            string json = File.ReadAllText(filePath);

            // ��JSON�ַ���ת��Ϊ��Ϣ�б�
            Serialization<ChatMessage> messages = JsonUtility.FromJson<Serialization<ChatMessage>>(json);

            foreach (ChatMessage message in messages.ToList())
            {
                Debug.Log("Role: " + message.Role + ", Content: " + message.Content);
            }
            Debug.Log("Messages loaded from JSON file: " + filePath);
        }

        

    }
}


