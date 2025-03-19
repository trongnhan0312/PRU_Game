using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NPCDialog : MonoBehaviour
{
    [System.Serializable]
    public struct DialogEntry
    {
        public string speakerName;
        public Sprite speakerSprite;
        public float spriteWidth;
        public float spriteHeight;
        [TextArea(3, 5)] public string dialogText;
        public AudioClip voiceClip;
    }

    public DialogEntry[] dialogEntries;
    private int dialogIndex;

    public GameObject dialogPanel;
    public Text dialogText;
    public Text nameNpc;
    public Image imageNpc;
    public AudioSource audioSource;

    private bool readyToSpeak;
    private bool isTyping;
    private GameObject[] enemies;
    public GameObject nextLv;
    public bool IsBossKilled { get; set; }
    [SerializeField] private NPCDialog npcAfterBossPrefab;
    [SerializeField] public GameObject newTowerPrefab;
    [SerializeField] public GameObject creditPrefab;
    [SerializeField] private Collider2D npcBlocker; // Thêm Collider để chặn di chuyển

    private PlayerController playerController;

    void Start()
    {
        dialogPanel.SetActive(false);
        dialogIndex = 0;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (IsBossKilled)
        {
            gameObject.SetActive(true);
        }

        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
            nextLv.SetActive(false);
        }

        playerController = FindObjectOfType<PlayerController>();

        // Đảm bảo blocker được bật từ đầu
        if (npcBlocker != null)
        {
            npcBlocker.gameObject.SetActive(true);
        }
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
        if (collision.CompareTag("Player") && IsBossKilled)
        {
            readyToSpeak = true;
        }
    }

    public void ShowCredit()
    {
        if (creditPrefab != null)
        {
            creditPrefab.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            readyToSpeak = false;
            if (dialogPanel.activeSelf)
            {
                EndConversation();
            }
        }
    }

    private void StartConversation()
    {
        if (playerController != null)
        {
            playerController.isInDialog = true;
        }

        dialogPanel.SetActive(true);
        dialogIndex = 0;

        // **Chặn người chơi đi tiếp bằng cách bật collider**
        if (npcBlocker != null)
        {
            npcBlocker.gameObject.SetActive(true);
        }

        StartCoroutine(ShowDialog());
    }

    private void ContinueConversation()
    {
        dialogIndex++;
        if (dialogIndex < dialogEntries.Length)
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
        if (playerController != null)
        {
            playerController.isInDialog = false;
        }
        dialogPanel.SetActive(false);
        dialogIndex = 0;
        gameObject.SetActive(false);

        // **Mở đường đi tiếp bằng cách tắt collider**
        if (npcBlocker != null)
        {
            npcBlocker.gameObject.SetActive(false);
        }

        if (npcAfterBossPrefab != null)
        {
            npcAfterBossPrefab.gameObject.SetActive(true);
            npcAfterBossPrefab.IsBossKilled = false;
        }

        if (newTowerPrefab != null)
        {
            newTowerPrefab.SetActive(true);
        }

        if (creditPrefab != null)
        {
            ShowCredit();
        }

        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(true);
            nextLv.SetActive(true);
        }
    }

    private IEnumerator ShowDialog()
    {
        isTyping = true;
        dialogText.text = "";
        nameNpc.text = dialogEntries[dialogIndex].speakerName;
        imageNpc.sprite = dialogEntries[dialogIndex].speakerSprite;

        if (dialogEntries[dialogIndex].spriteWidth > 0 && dialogEntries[dialogIndex].spriteHeight > 0)
        {
            imageNpc.rectTransform.sizeDelta = new Vector2(dialogEntries[dialogIndex].spriteWidth, dialogEntries[dialogIndex].spriteHeight);
        }

        if (dialogEntries[dialogIndex].voiceClip != null && audioSource != null)
        {
            audioSource.clip = dialogEntries[dialogIndex].voiceClip;
            audioSource.Play();
        }

        foreach (char letter in dialogEntries[dialogIndex].dialogText)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
    }

    private void SkipTyping()
    {
        StopAllCoroutines();
        dialogText.text = dialogEntries[dialogIndex].dialogText;
        isTyping = false;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
