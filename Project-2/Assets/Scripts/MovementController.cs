using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private Transform stackParentTransform;
    [SerializeField] private Transform backGroundParentTransform;

    private Transform playerTransform;
    private GameManager GameManager;

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        GameManager = GameManager.instance;
    }

    private void Update()
    {
        // stackParentTransform.Translate(Vector3.back * (Time.deltaTime * 3f));
        // backGroundParentTransform.Translate(Vector3.back * (Time.deltaTime * 3f));
        if (!GameManager.countdownEnded) return;
        if (GameManager.levelCompleted) return;
        playerTransform.Translate(Vector3.forward * (Time.deltaTime * 1.8f));
    }
}
