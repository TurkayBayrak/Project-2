using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private static readonly int IsDancing = Animator.StringToHash("isDancing");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.OnLevelCompleted += OnLevelCompleted;
        EventManager.OnNextLevelStarting += OnNextLevelStarting;
    }

    private void OnDisable()
    {
        EventManager.OnLevelCompleted -= OnLevelCompleted;
        EventManager.OnNextLevelStarting -= OnNextLevelStarting;
    }

    private void OnLevelCompleted()
    {
        SetPlayerDancing(true);
    }

    private void OnNextLevelStarting()
    {
        SetPlayerDancing(false);
    }

    private void SetPlayerDancing(bool value)
    {
        animator.SetBool(IsDancing, value);
    }
}
