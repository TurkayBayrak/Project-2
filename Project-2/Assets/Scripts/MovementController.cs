using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private Transform stackParentTransform;
    [SerializeField] private Transform backGroundParentTransform;

    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        // stackParentTransform.Translate(Vector3.back * (Time.deltaTime * 3f));
        // backGroundParentTransform.Translate(Vector3.back * (Time.deltaTime * 3f));
        
        playerTransform.Translate(Vector3.forward * (Time.deltaTime * 3.2f));
    }
}
