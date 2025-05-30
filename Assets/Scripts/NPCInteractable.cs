using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    public UnityEvent firstTalkEN;
    public UnityEvent giveQuestEN;
    public UnityEvent firstTalkNO;
    public UnityEvent giveQuestNO;
    public UnityEvent thanksEN;

    [SerializeField] public TextBubble textBubble;


    public bool hasSpoken = false;
    public bool hasQuests = false;
    bool gaveValidationRecently=false;

    private GameSettings gameSettings;
    private QuestManager questManager;
    private PlayerController playerController;
    private ValidationBar validationBar;

    void Start()
    {
        gameSettings = FindFirstObjectByType<GameSettings>();
        if (gameSettings == null)
        {
            Debug.LogError("GameSettings not found in the scene!");
        }
        questManager = FindFirstObjectByType<QuestManager>();
        if (gameSettings == null)
        {
            Debug.LogError("Quest manager not found in the scene!");
        }
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Pop in a player dummy!");
        }
        validationBar = FindFirstObjectByType<ValidationBar>();

    }

    public void Interact(Transform interactorTransform)
    {
        if (hasSpoken && !textBubble.isTalking && (CompareTag("Torch NPC")) && questManager.torchComplete)//super specific
        {
            //hasQuests = false;
            Debug.Log("Torches done...for now");
            //thanksEN.Invoke();
            StartCoroutine(ThankPlayer());

        }
        if (hasSpoken && !textBubble.isTalking && (CompareTag("Tutorial NPC")) && questManager.tutorialComplete)//super specific
        {
            //hasQuests = false;
            Debug.Log("Tutorial doneski");
            StartCoroutine(ThankPlayer());
        }
        if (hasSpoken && !textBubble.isTalking && (CompareTag("Fetch NPC")) && questManager.fetchComplete)//super specific
        {
            //hasQuests = false;
            Debug.Log("Fetch quest done");
            StartCoroutine(ThankPlayer());
        }

        if (!hasSpoken && !textBubble.isTalking)
        {
            if (gameSettings.isEnglish)
            {
                firstTalkEN.Invoke();
            }
            if (gameSettings.isNorwegian)
            {
                firstTalkNO.Invoke();
            }

            StartCoroutine(FirstTalk());
        }
        if (hasSpoken && !textBubble.isTalking && !hasQuests)
        {
            StartCoroutine(QuestTalk());
        }
    }

    IEnumerator FirstTalk()
    {
        while (!textBubble.isTalking)
        {
            yield return null;
        }
        while (textBubble.isTalking)
        {
            yield return null;
        }
        hasSpoken = true;
    }

    IEnumerator QuestTalk()
    {
        Debug.Log("gibbin ze questos");
        gaveValidationRecently = false;

        if (gameSettings.isEnglish)
        {
            giveQuestEN.Invoke();

        }
        if (gameSettings.isNorwegian)
        {
            giveQuestNO.Invoke();
        }
        hasQuests = true;
        yield break;
    }
    IEnumerator ThankPlayer()
    {
        if (!gaveValidationRecently)
        {
            gaveValidationRecently = true;
            thanksEN.Invoke();
            playerController.currentFlightStamina = playerController.maxFlightStamina;
            if(questManager.currentValidation>5)
            {
                questManager.maxValidation+=2;
                questManager.currentValidation++;
                validationBar.GrowBy10Percent();
                yield return new WaitForSeconds(5);//cooldown so we don't end up in a quest loop
                hasQuests = false;
                yield break;
            }
            else
            {
                questManager.currentValidation++;
                yield return new WaitForSeconds(5);//cooldown so we don't end up in a quest loop
                hasQuests = false;
                yield break;
            }
        }
        else
        {
            yield break;
        }
    }
}
