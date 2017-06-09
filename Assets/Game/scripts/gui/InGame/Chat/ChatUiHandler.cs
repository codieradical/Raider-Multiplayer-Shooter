using Raider.Game.GUI.Components;
using Raider.Game.GUI.StartMenu;
using Raider.Game.Player;
using Raider.Game.Scene;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Raider.Game.GUI.Screens
{
	public class ChatUiHandler : MonoBehaviour
    {
        #region Singleton Setup

        public static ChatUiHandler instance;
        Animator animatorInstance;
        public InputField chatInputField;

        // Use this for initialization
        void Awake()
        {
            //if (instance != null)
            //    Debug.LogWarning("More than one ChatUiHandler instance");
            instance = this;
            animatorInstance = GetComponent<Animator>();
        }

        void OnDestroy()
        {
            instance = null;
        }

        #endregion

        public bool IsOpen
        {
            get { return animatorInstance.GetBool("open"); }
            set { animatorInstance.SetBool("open", value); }
        }

        public GameObject fullLogContainer;
        public GameObject shortLogContainer;
        public int shortLogSize;
        public Font font;
        public int fontSize;
        public Color outlineColor;
        public RuntimeAnimatorController fadeOutController;

        // Use this for initialization
        void Start()
        {
            if (fullLogContainer == null)
                Debug.LogError("The chat ui handler has no text container to add to!");

            if(fullLogContainer.transform.childCount < 1)
                LoadChatHistory();
        }

        void LoadChatHistory()
        {
            //This loads the log the wrong way around.
            //foreach (string message in ChatManager.chatLog)
            //    AddMessageToFullLog(message);

            for(int index = PlayerChatManager.chatLog.Count - 1; index > -1; --index)
            {
                AddMessageToFullLog(PlayerChatManager.chatLog.ToArray()[index]);
            }
        }

        public void AddMessageToFullLog(string message)
        {
            //If the chat container is full, get deleting.
            if (fullLogContainer.transform.childCount >= PlayerChatManager.maxChatHistory)
                Destroy(fullLogContainer.transform.GetChild(0).gameObject);

            //Initialize the new message object.
            GameObject newLine = new GameObject();
            newLine.AddComponent<RectTransform>();
            newLine.transform.SetParent(fullLogContainer.transform, false);

            ContentSizeFitter newLineContentFitter = newLine.AddComponent<ContentSizeFitter>();
            newLineContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            newLine.AddComponent<Outline>().effectColor = outlineColor;

            Text newLineText = newLine.AddComponent<Text>();
            newLineText.fontSize = fontSize;
            newLineText.font = font;
            newLineText.supportRichText = true;

            //Give it the message.
            newLineText.text = message;
        }

        public void AddMessageToShortLog(string message)
        {
            //If the chat container is full, get deleting.
            if (shortLogContainer.transform.childCount >= shortLogSize)
                Destroy(shortLogContainer.transform.GetChild(0).gameObject);

            //Initialize the new message object.
            GameObject newLine = new GameObject();
            newLine.AddComponent<RectTransform>();
            newLine.transform.SetParent(shortLogContainer.transform, false);

            ContentSizeFitter newLineContentFitter = newLine.AddComponent<ContentSizeFitter>();
            newLineContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            newLine.AddComponent<Outline>().effectColor = outlineColor;

            CanvasGroup cg = newLine.AddComponent<CanvasGroup>();
            cg.interactable = false;
            cg.blocksRaycasts = false;

            Text newLineText = newLine.AddComponent<Text>();
            newLineText.fontSize = fontSize;
            newLineText.font = font;
            newLineText.supportRichText = true;

            newLine.AddComponent<Animator>().runtimeAnimatorController = fadeOutController;

            //Give it the message.
            newLineText.text = message;
        }

        public void SendNewMessage(InputField input)
        {
            if (input.text != "")
                StartCoroutine(SendNewMessage(input.text));

            CloseChatInput();
        }

        IEnumerator SelectInputObject()
        {
            yield return 0; //Wait a frame...
            chatInputField.OnPointerClick(new PointerEventData(EventSystem.current));
            EventSystem.current.SetSelectedGameObject(chatInputField.gameObject);
        }

        IEnumerator SendNewMessage(string message)
        {
            yield return 0; //Wait a frame...

            if (PlayerData.localPlayerData != null)
                PlayerData.localPlayerData.GetComponent<PlayerChatManager>().CmdSendMessage(message, PlayerData.localPlayerData.PlayerSyncData.id);
        }

        public void OpenChatInput()
        {
            //If the player is in an options menu, don't let them chat.
            if (PlayerData.localPlayerData == null || (StartMenuHandler.instance != null && StartMenuHandler.instance.IsOpen) || OptionsPaneHandler.IsOpen())
            {
                return;
            }

            //If the player is on the mainmenu but not in lobby, don't let them chat.
            if(Scenario.InLobby && (MenuManager.instance.CurrentMenu == MainmenuController.instance.MainMenuScreen.GetComponent<Menu>() && GametypeButtons.instance.IsOpen))
            {
                return;
            }

            //Change the value of the text input to reset the size of the caret.
            //This prevents the input height being too large after sending a multi-line message.
            chatInputField.text = " ";
            chatInputField.text = "";

            IsOpen = true;

			if(!Scenario.InLobby)
				PlayerData.localPlayerData.localPlayerController.PausePlayer();

			StartCoroutine(SelectInputObject());
        }

        public void CloseChatInput()
        {
            chatInputField.text = "";
            if(!EventSystem.current.alreadySelecting)
                EventSystem.current.SetSelectedGameObject(null);

            if (!Scenario.InLobby)
                PlayerData.localPlayerData.localPlayerController.UnpausePlayer();

            IsOpen = false;
        }
    }
}