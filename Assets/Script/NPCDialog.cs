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
        public float spriteWidth;  // Add width input
        public float spriteHeight; // Add height input
        [TextArea(3, 5)] public string dialogText;
        public AudioClip voiceClip; // Thêm âm thanh hội thoại
    }

    public DialogEntry[] dialogEntries;
    private int dialogIndex;

    public GameObject dialogPanel;
    public Text dialogText;
    public Text nameNpc;
    public Image imageNpc;
    public AudioSource audioSource; // Thêm AudioSource

    private bool readyToSpeak;
    private bool isTyping;
    private GameObject[] enemies;
    public GameObject nextLv;
    public bool IsBossKilled { get; set; }
    [SerializeField] private NPCDialog npcAfterBossPrefab;
    [SerializeField]  public GameObject newTowerPrefab;
    [SerializeField] public GameObject creditPrefab;

    private PlayerController playerController;

    void Start()
    {
        Debug.Log("Day ne: 1111111111");

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
        Debug.Log("Day ne: " + playerController.name);
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
            creditPrefab.SetActive(true); // Kích hoạt credit
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
        Debug.Log("Check");

        if (playerController != null)
        {
            Debug.Log("Vo");

            playerController.isInDialog = true;  // Bắt đầu hội thoại
        }
        Debug.Log("END");

        dialogPanel.SetActive(true);
        dialogIndex = 0;

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
            playerController.isInDialog = false;  // Kết thúc hội thoại
        }
        dialogPanel.SetActive(false);
        dialogIndex = 0;
        gameObject.SetActive(false);


        if (npcAfterBossPrefab != null)
        {
            //NPCDialog newNpc = Instantiate(npcAfterBossPrefab, new Vector3(5, 0, 0), Quaternion.identity); // Vị trí của NPC mới
            npcAfterBossPrefab.gameObject.SetActive(true); // Kích hoạt NPC mới
            npcAfterBossPrefab.IsBossKilled = false; // Đánh dấu NPC mới
        }
        if (newTowerPrefab != null)
        {
            // Thay thế tower cũ bằng tower mới
            newTowerPrefab.SetActive(true);
        }
        if(creditPrefab != null)
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

        // Set sprite size based on width and height values
        if (dialogEntries[dialogIndex].spriteWidth > 0 && dialogEntries[dialogIndex].spriteHeight > 0)
        {
            imageNpc.rectTransform.sizeDelta = new Vector2(dialogEntries[dialogIndex].spriteWidth, dialogEntries[dialogIndex].spriteHeight);
        }

        // Play voice clip if available
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

        // Dừng âm thanh nếu có
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
