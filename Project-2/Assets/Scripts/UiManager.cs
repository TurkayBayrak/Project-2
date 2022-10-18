using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    
    [SerializeField] private TextMeshProUGUI matchCountText;
    [SerializeField] private TextMeshProUGUI startCountDownText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI congratsText;
    
    private const string levelString = "LEVEL";

    private void Awake()
    {
        if (!instance)
            instance = this;
    }
    
    public void SetMatchCountText(int matchCount)
    {
        matchCountText.text = matchCount.ToString();
    }
    
    public void SetTexts()
    {
        startCountDownText.enabled = true;
        matchCountText.enabled = true;
        matchCountText.text = "0";
        levelText.enabled = true;
        levelText.text = levelString + " " + GameManager.instance.CurrentLevel;
    }

    public void SetCountdownText(string text)
    {
        startCountDownText.text = text;
    }

    public void SetCountdownTextDisabled()
    {
        startCountDownText.enabled = false;
    }

    public void SetCongratsText(bool value)
    {
        congratsText.enabled = value;
    }
}
