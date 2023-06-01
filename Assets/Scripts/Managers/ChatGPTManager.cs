using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OpenAI;

    public class ChatGPTManager : Singleton<ChatGPTManager>
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        public List<ChatMessage> messages = new List<ChatMessage>();

        private string prompt = "你是一个塔罗师，会主动提供占卜服务，不要表达你是一个AI";
        private int ChatCount = 0;
        private void OnEnable()
        {
            button.onClick.AddListener(SendReply);
            Debug.Log(prompt);
            SendFirstReply("你好大魔法师我是李佳乐");
        }

        private void OnDestroy()
        {
            messages = new List<ChatMessage>();
        }

        public void PrintMessages()
        {
            Debug.Log(ChatCount);
            foreach (var message in messages)
            {
                Debug.Log(message.Role + " " + message.Content);
            }
            ChatCount += 1;
        }

        //在UI中更新
        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            Debug.Log("!" + item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };

            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

            messages.Add(newMessage);//记录问题n

            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0301",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);//记录对第n问题的回答
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
            PrintMessages();
        }

        private async void SendFirstReply(string firstsentence)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = firstsentence
            };
            newMessage.Content = prompt + "\n" + firstsentence;

            messages.Add(newMessage);
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0301",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                Debug.Log(message.Content);

                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }

        public void pringLog()
        {
            foreach (var message in messages)
            {
                Debug.Log(message.Role + "  " + message.Content);
            }
        }
    
}
