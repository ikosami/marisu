using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public TextMeshProUGUI nameText; // メッセージを表示するTextMeshProUGUI
    public TextMeshProUGUI messageText; // メッセージを表示するTextMeshProUGUI
    public Image[] charaImages; // キャラクター表示用のImage
    public Image backgroundImage; // 背景表示用のImage
    public string csvFileName = "test"; // CSVファイル名

    private List<Command> commands = new List<Command>();

    class Command
    {
        public string action;
        public string detailType;
        public string content;
    }

    private int currentCommandIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        LoadCommandsFromCSV();
        if (commands.Count > 0)
        {
            ExecuteCommand(commands[currentCommandIndex]);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            Next();
        }
    }

    void Next()
    {
        currentCommandIndex++;

        if (currentCommandIndex < commands.Count)
        {
            ExecuteCommand(commands[currentCommandIndex]);
        }
    }

    void LoadCommandsFromCSV()
    {
        string path = Path.Combine(Application.dataPath, "Game/Resources/Story/" + csvFileName + ".csv");
        if (File.Exists(path))
        {
            string fileContent = File.ReadAllText(path);
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] fields = line.Split(',');
                    if (fields.Length > 2)
                    {
                        Command command = new Command();
                        command.action = fields[0];
                        command.detailType = fields[1];
                        command.content = fields[2];
                        commands.Add(command); // アクション, 詳細種類, 内容をリストに追加
                    }
                }
            }
        }
        else
        {
            Debug.LogError("CSVファイルが見つかりません: " + path);
        }
    }

    void ExecuteCommand(Command command)
    {
        Debug.Log("ExecuteCommand: " + command.action); // 追加
        switch (command.action)
        {
            case "セリフ":
                Debug.Log("セリフ: " + command.detailType + " " + command.content); // 追加
                StartCoroutine(DisplayMessage(command.detailType, command.content));
                break;
            case "表示変更":
                ChangeCharacterSprite(command.detailType, command.content);
                Next();
                break;
            default:
                Debug.LogWarning("未知のアクション: " + command.action);

                Next();
                break;
        }
    }

    IEnumerator DisplayMessage(string detailType, string message)
    {
        isTyping = true;
        messageText.text = "";

        nameText.transform.parent.gameObject.SetActive(detailType != string.Empty);
        nameText.text = detailType;

        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(0.05f); // 1文字ごとの待機時間
        }

        isTyping = false;
    }

    void ChangeCharacterSprite(string detailType, string spriteName)
    {



        Sprite newSprite = Resources.Load<Sprite>("Image/" + spriteName);
        if (newSprite != null)
        {
            switch (detailType)
            {
                case "左":
                    charaImages[0].sprite = newSprite;
                    break;
                case "中":
                    charaImages[1].sprite = newSprite;
                    break;
                case "右":
                    charaImages[2].sprite = newSprite;
                    break;
                case "背景":
                    backgroundImage.sprite = newSprite;
                    break;
                default:
                    Debug.LogWarning("未知のキャラクター位置: " + detailType);
                    break;
            }
        }
        else
        {
            Debug.LogError("キャラクター画像が見つかりません: " + spriteName);
        }
    }

    void ChangeBackgroundSprite(string detailType, string spriteName)
    {
    }
}
