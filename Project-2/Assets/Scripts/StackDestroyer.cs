using UnityEngine;

public class StackDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Stack"))
        {
            other.gameObject.SetActive(false);
            
        }
    }
}
