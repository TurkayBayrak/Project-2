using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI matchCountText;
    [SerializeField] private TextMeshProUGUI startCountDownText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button playNextLevelButton;

    [SerializeField] private GameObject fogPanel;

    private const string levelString = "LEVEL";

    private void Start()
    {
        playButton.onClick.AddListener(StartCountdown);
        restartButton.onClick.AddListener(Restart);
        playNextLevelButton.onClick.AddListener(PlayNextLevel);
    }

    private void OnEnable()
    {
        EventManager.OnStackMatched += OnStackMatched;
        EventManager.OnGameOver += OnGameOver;
        EventManager.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        EventManager.OnStackMatched -= OnStackMatched;
        EventManager.OnGameOver -= OnGameOver;
        EventManager.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnStackMatched(int matchCount)
    {
        matchCountText.text = matchCount.ToString();
    }

    private void OnGameOver()
    {
        StartCoroutine(WaitBeforeRestartButton());
    }

    private void OnLevelCompleted()
    {
        StartCoroutine(WaitBeforeNextLevelButton());
    }

    private IEnumerator WaitBeforeRestartButton()
    {
        yield return new WaitForSeconds(1);
        fogPanel.SetActive(true);
        playButton.gameObject.SetActive(false);
        playNextLevelButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
    }

    private void StartCountdown()
    {
        startCountDownText.enabled = true;
        fogPanel.SetActive(false);
        matchCountText.enabled = true;
        levelText.enabled = true;
        levelText.text = levelString + " " + GameManager.instance.currentLevel;
        StartCoroutine(CountDownCo());
    }
    
    private IEnumerator CountDownCo()
    {
        startCountDownText.text = "3";
        yield return new WaitForSeconds(1);
        startCountDownText.text = "2";
        yield return new WaitForSeconds(1);
        startCountDownText.text = "1";
        yield return new WaitForSeconds(1);
        startCountDownText.text = "GO";
        yield return new WaitForSeconds(1);
        startCountDownText.enabled = false;
        GameManager.instance.SendFirstStack();
    }

    private void Restart()
    {
        DOTween.KillAll();
        GameManager.instance.ResetValuesOnRestart();
        StartCountdown();
    }

    private IEnumerator WaitBeforeNextLevelButton()
    {
        yield return new WaitForSeconds(2);
        fogPanel.SetActive(true);
        playButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        playNextLevelButton.gameObject.SetActive(true);
    }

    private void PlayNextLevel()
    {
        matchCountText.text = "0";
        GameManager.instance.currentLevel++;
        EventManager.NextLevelStarting();
        StartCountdown();
        GameManager.instance.ResetValuesOnRestart();
    }
}
