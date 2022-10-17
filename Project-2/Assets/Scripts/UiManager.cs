using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    
    [SerializeField] private TextMeshProUGUI matchCountText;
    public TextMeshProUGUI startCountDownText;
    [SerializeField] private TextMeshProUGUI levelText;
    
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
}
