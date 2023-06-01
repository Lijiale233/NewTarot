using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChatGLMHandller : MonoBehaviour
{
    [SerializeField]private string apiUrl = "YOUR_API_URL"; // ��YOUR_API_URL�滻Ϊʵ�ʵ�API��ַ
    /// <summary>
    /// ��ʷ�Ի�
    /// </summary>
    [SerializeField] private List<List<string>> m_History = new List<List<string>>();

    static void printText(string x)
    {
        Debug.Log(x);
    }
    Action<String> callback = printText;
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="_postWord"></param>
    /// <param name="_callback"></param>
    /// <returns></returns>
    public IEnumerator GetPostData(string _postWord, System.Action<string> _callback)
    {
        Debug.Log("doing post");
        string jsonPayload = JsonConvert.SerializeObject(new RequestData
        {
            prompt = _postWord,
            history= m_History
        });
        
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("doing post2");
            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                string _msg = request.downloadHandler.text;
                ResponseData response = JsonConvert.DeserializeObject<ResponseData>(_msg);

                //��¼��ʷ�Ի�
                m_History = response.history;
                Debug.Log(response.response);
                //�ص�
                _callback(response.response);

            }

        }
    }

    private void Start()
    {
        Debug.Log("test");
        StartCoroutine(GetPostData("hello", callback));
    }
    #region ���Ķ���

    [Serializable]
    private class RequestData
    {
        public string prompt;
        public List<List<string>> history;
    }

    [Serializable]
    private class ResponseData
    {
        public string response;
        public List<List<string>> history;
        public int status;
        public string time;
    }

    #endregion


}
