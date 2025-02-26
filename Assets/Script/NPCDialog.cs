    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
using UnityEngine.InputSystem; // Thêm thư viện
public class NPCDialog : MonoBehaviour
    {
        public string[] dialogNPC;
        private int dialogIndex;

        public GameObject dialogPanel;
        public Text dialogText;

        public Text nameNpc;
        public Image imageNpc;
        public Sprite spritesNpc;

        private bool readyToSpeak;
        private bool isTyping;

        void Start()
        {
            dialogPanel.SetActive(false);
            dialogIndex = 0;
            imageNpc.sprite = spritesNpc;
        }

    void Update()
    {
        if (readyToSpeak && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!dialogPanel.activeSelf)
            {
                StartConversation();
            }
            else if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                ContinueConversation();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                readyToSpeak = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                readyToSpeak = false;
                if (dialogPanel.activeSelf) // Chỉ kết thúc khi đang hội thoại
                {
                    EndConversation();
                }
            }
        }


        private void StartConversation()
        {
            dialogPanel.SetActive(true);
            dialogIndex = 0;
            StartCoroutine(ShowDialog());
        }

        private void ContinueConversation()
        {
            dialogIndex++;
            if (dialogIndex < dialogNPC.Length)
            {
                StartCoroutine(ShowDialog());
            }
            else
            {
                EndConversation();
            }
        }

        private void EndConversation()
        {
            dialogPanel.SetActive(false);
            dialogIndex = 0;
            gameObject.SetActive(false); // Làm NPC biến mất
        }


        private IEnumerator ShowDialog()
        {
            isTyping = true;
            dialogText.text = "";
            foreach (char letter in dialogNPC[dialogIndex])
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
            isTyping = false;
        }

        private void SkipTyping()
        {
            StopAllCoroutines();
            dialogText.text = dialogNPC[dialogIndex];
            isTyping = false;
        }
    }