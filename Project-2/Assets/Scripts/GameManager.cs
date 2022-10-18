using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private Transform fallingStackTransform;
    [SerializeField] private Material[] stackMaterials;
    [SerializeField] private Transform firstStackTransform;
    [SerializeField] private Button playButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button playNextLevelButton;
    [SerializeField] private GameObject fogPanel;

    private MeshRenderer fallingStackMeshRenderer;
    private MeshRenderer currentStackMeshRenderer;
    private Transform previousStackTransform;
    private Transform currentStackTransform;
    private Transform finishTransform;
    private ObjectPooler ObjectPooler;
    private GameObject player;
    private Transform chibiTransform;
    private Vector3 playerStartPosition;
    private UiManager UiManager;

    private int perfectSeries;
    private int matchCount;
    private int index;
    private int materialIndex;
    private bool isOnTheRight;
    private bool failed;
    private bool playerFalling;
    private float checkPoint;
    private bool levelCompleted;
    private bool countdownEnded;

    private readonly Vector3 defaultVector3 = new Vector3(0, 0, 0);
    private readonly Vector3 defaultStackScale = new Vector3(3,1,3);

    public int CurrentLevel { get; private set; } = 1;

    [SerializeField] private Level[] levels;
    
    [Tooltip("0: impossible, 1: too easy")]
    [SerializeField] private float perfectMatchTolerance;
    
    
    private void Awake()
    {
        if (!instance)
            instance = this;
    }
    
    private void Start()
    {
        ObjectPooler = GetComponent<ObjectPooler>();
        player = GameObject.FindWithTag("Player");
        fallingStackMeshRenderer = fallingStackTransform.GetComponent<MeshRenderer>();
        playerStartPosition = player.transform.position;
        finishTransform = GameObject.FindWithTag("Finish").transform;
        chibiTransform = player.transform.GetChild(0).transform;
        UiManager = UiManager.instance;
        
        playButton.onClick.AddListener(StartCountdown);
        restartButton.onClick.AddListener(Restart);
        playNextLevelButton.onClick.AddListener(PlayNextLevel);
    }

    private void OnEnable()
    {
        EventManager.OnLevelCompleted += OnLevelCompleted;
        EventManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        EventManager.OnLevelCompleted -= OnLevelCompleted;
        EventManager.OnGameOver -= OnGameOver;
    }

    private void OnLevelCompleted()
    {
        levelCompleted = true;
        if (CurrentLevel != levels.Length)
            StartCoroutine(WaitBeforeNextLevelButton());
        else
        {
            fogPanel.SetActive(true);
            playButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(true);
            playNextLevelButton.gameObject.SetActive(false);
            UiManager.SetCongratsText(true);
        }
    }

    private void OnGameOver()
    {
        StartCoroutine(WaitBeforeRestartButton());
    }

    private void Update()
    {
        if (!countdownEnded) return;
        if (levelCompleted) return;
        
        player.transform.Translate(Vector3.forward * (Time.deltaTime * 1.75f));
        
        if (failed)
        {
            if (playerFalling) return;
            if (!(player.transform.position.z > checkPoint)) return;
            player.transform.DOMoveY(-30, 10);
            EventManager.GameOver();
            perfectSeries = 0;
            playerFalling = true;
            return;
        }

        if (player.transform.position.z > checkPoint)
        {
            if (playerFalling) return;
            player.transform.DOMoveY(-30, 10);
            EventManager.GameOver();
            perfectSeries = 0;
            failed = true;
            playerFalling = true;
        }

        if (!Input.GetMouseButtonDown(0)) return;
        if (matchCount == levels[CurrentLevel - 1].stackCount) return;
        SetStackAndSendNextOne();
    }
    
    private void SetStackAndSendNextOne()
    {
        DOTween.KillAll();
        PlaceStack();
    }

    private void PlaceStack()
    {
        var currentLocalScale = currentStackTransform.localScale;
        var previousLocalScale = previousStackTransform.localScale;
        var currentPosition = currentStackTransform.position;
        var previousPosX = previousStackTransform.position.x;
        var currentPosX = currentPosition.x;
        var currentRightX = currentPosition.x + currentLocalScale.x / 2;
        var previousLeftX = previousPosX - previousLocalScale.x / 2;
        var currentLeftX = currentPosition.x - currentLocalScale.x / 2;
        var previousRightX = previousPosX + previousLocalScale.x / 2;

        if (previousRightX < currentLeftX || previousLeftX > currentRightX) //No match condition
        {
            currentStackTransform.DOMoveY(-30, 5);
            failed = true;
            return;
        }

        matchCount++;
        UiManager.SetMatchCountText(matchCount);
        
        var currentPosZ = currentStackTransform.position.z;
        var currentScaleZ = currentStackTransform.localScale.z;
        checkPoint = currentPosZ + currentScaleZ / 2;
        if (matchCount == levels[CurrentLevel - 1].stackCount)
            checkPoint += 10;

        
        if (Mathf.Abs(currentPosition.x - previousPosX) < perfectMatchTolerance) //Perfect match condition
        {
            currentStackTransform.position = new Vector3(previousPosX, currentPosition.y, currentPosition.z);
            AudioManager.instance.PlayNote(perfectSeries);
            perfectSeries++;
        }
        else //Matched but not perfect
        {
            perfectSeries = 0;
            AudioManager.instance.PlayCutEffect();
            if (currentPosX < previousPosX) //Current stack is on the left side of the previous stack
            {
                var newScaleX = currentRightX - previousLeftX;
                var newScale = new Vector3(newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.localScale = new Vector3(currentLocalScale.x - newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.position = new Vector3(previousLeftX - fallingStackTransform.localScale.x / 2, currentPosition.y, currentPosition.z);
                fallingStackTransform.eulerAngles = defaultVector3;
                fallingStackTransform.DORotate(new Vector3(0, 0, 30), 1);
                currentStackTransform.localScale = newScale;
                var newPosX = previousLeftX + newScaleX / 2;
                var newPos = new Vector3(newPosX, currentPosition.y, currentPosition.z);
                currentStackTransform.position = newPos;
                player.transform.DOMoveX(newPosX, 1);
            }
            else //Current stack is on the right side of the previous stack
            {
                var newScaleX = previousRightX - currentLeftX;
                var newScale = new Vector3(newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.localScale = new Vector3(currentLocalScale.x - newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.position = new Vector3(previousRightX + fallingStackTransform.localScale.x / 2, currentPosition.y, currentPosition.z);
                fallingStackTransform.eulerAngles = defaultVector3;
                fallingStackTransform.DORotate(new Vector3(0, 0, -30), 1);
                currentStackTransform.localScale = newScale;
                var newPosX = previousRightX - newScaleX / 2;
                var newPos = new Vector3(newPosX, currentPosition.y, currentPosition.z);
                player.transform.DOMoveX(newPosX, 1);
                currentStackTransform.position = newPos;
            }
            fallingStackTransform.DOMoveY(-30, 8);
            fallingStackMeshRenderer.material = currentStackMeshRenderer.material;
        }
        GetNextStack();
    }

    private void GetNextStack()
    {
        if (matchCount == levels[CurrentLevel - 1].stackCount) return;
        
        if (index != ObjectPooler.PooledObjects.Count - 1)
            index++;
        else
            index = 0;
        
        var stack = ObjectPooler.GetPooledObject("Stack", index);

        previousStackTransform = currentStackTransform;

        stack.transform.localScale = previousStackTransform.localScale;
        currentStackTransform = stack.transform;
        SetStackMaterial();

        stack.SetActive(true);
        SendNextStack();
    }

    private void SendNextStack()
    {
        var zAdjustmentPre = previousStackTransform.localScale.z / 2;
        var zAdjustmentCurrent = currentStackTransform.localScale.z / 2;
        var totalAdjustment = zAdjustmentPre + zAdjustmentCurrent;

        var previousStackPosition = previousStackTransform.position;

        var multiplier = isOnTheRight ? 1 : -1;
        
        var newPosition = new Vector3(previousStackPosition.x + 20 * multiplier, -0.5f, previousStackPosition.z + totalAdjustment);
        currentStackTransform.position = newPosition;

        currentStackTransform.DOMoveX(previousStackPosition.x - 20 * multiplier, 6);
        isOnTheRight = !isOnTheRight;
    }
    
    private void SendFirstStack()
    {
        chibiTransform.DORotate(defaultVector3, 0.5f);
        SetFinishStackPosition();
        countdownEnded = true;
        levelCompleted = false;
        var stack = ObjectPooler.GetPooledObject("Stack", 0);
        currentStackTransform = stack.transform;
        stack.SetActive(true);

        previousStackTransform = firstStackTransform;
        var localScale = previousStackTransform.localScale;
        var zAdjustmentPre = localScale.z / 2;
        var zAdjustmentCurrent = stack.transform.localScale.z / 2;
        var totalAddition = zAdjustmentPre + zAdjustmentCurrent;

        var position = previousStackTransform.position;
        var pos = new Vector3(position.x - 20, -0.5f, position.z + totalAddition);
        stack.transform.position = pos;
        isOnTheRight = !isOnTheRight;
        
        var previousPosZ = position.z;
        var previousScaleZ = localScale.z;
        checkPoint = previousPosZ + previousScaleZ / 2;

        stack.transform.DOMoveX(120, 6);
        SetStackMaterial();
    }
    
    private void SetStackMaterial()
    {
        currentStackMeshRenderer = currentStackTransform.GetComponent<MeshRenderer>();
        currentStackMeshRenderer.material = stackMaterials[materialIndex];

        if (materialIndex != stackMaterials.Length - 1)
            materialIndex++;
        else
            materialIndex = 0;
    }

    private void ResetValuesOnRestart()
    {
        foreach (var stack in ObjectPooler.PooledObjects)
        {
            stack.transform.position = defaultVector3;
            stack.transform.localScale = defaultStackScale;
        }
        
        fallingStackTransform.position = defaultVector3;
        player.transform.position = playerStartPosition;
        
        chibiTransform.position = new Vector3(100, 0, 0);
        
        matchCount = 0;
        index = 0;
        materialIndex = 0;
        isOnTheRight = false;
        failed = false;
        playerFalling = false;
        checkPoint = 0;
        countdownEnded = false;
        perfectSeries = 0;
    }

    private void SetFinishStackPosition()
    {
        var totalStackLength = levels[CurrentLevel - 1].stackCount * 3;
        var firstStackEndPoint = firstStackTransform.position.z + firstStackTransform.localScale.z / 2;
        var finishPointZ = finishTransform.localScale.z / 2;

        var totalLength = totalStackLength + firstStackEndPoint + finishPointZ;

        finishTransform.position = new Vector3(100, -0.5f, totalLength);
    }
    
    private void StartCountdown()
    {
        fogPanel.SetActive(false);
        UiManager.SetTexts();
        StartCoroutine(CountDownCo());
    }

    private IEnumerator CountDownCo()
    {
        UiManager.SetCountdownText("3");
        yield return new WaitForSeconds(1);
        UiManager.SetCountdownText("2");
        yield return new WaitForSeconds(1);
        UiManager.SetCountdownText("1");
        yield return new WaitForSeconds(1);
        UiManager.SetCountdownText("GO");
        yield return new WaitForSeconds(1);
        UiManager.SetCountdownTextDisabled();
        SendFirstStack();
    }

    private IEnumerator WaitBeforeRestartButton()
    {
        yield return new WaitForSeconds(1);
        fogPanel.SetActive(true);
        playButton.gameObject.SetActive(false);
        playNextLevelButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
    }

    private IEnumerator WaitBeforeNextLevelButton()
    {
        yield return new WaitForSeconds(2);
        fogPanel.SetActive(true);
        playButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        playNextLevelButton.gameObject.SetActive(true);
    }

    private void Restart()
    {
        DOTween.KillAll();
        ResetValuesOnRestart();
        fogPanel.SetActive(false);
        UiManager.SetTexts();
        UiManager.SetCongratsText(false);
        CurrentLevel = 1;
        EventManager.LevelStarting();
        StartCountdown();
    }

    private void PlayNextLevel()
    {
        CurrentLevel++;
        EventManager.LevelStarting();
        fogPanel.SetActive(false);
        UiManager.SetTexts();
        StartCountdown();
        ResetValuesOnRestart();
    }
}

[System.Serializable]
public class Level
{
    public int stackCount;
}
