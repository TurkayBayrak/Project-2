using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineStateDrivenCamera stateDrivenCamera;
    private Animator animator;

    [SerializeField] private CinemachineVirtualCamera dancingCam;
    private CinemachinePOV dancingCamPov;

    private void Awake()
    {
        stateDrivenCamera = GetComponent<CinemachineStateDrivenCamera>();
        animator = GetComponent<Animator>();
        dancingCamPov = dancingCam.GetCinemachineComponent<CinemachinePOV>();
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
        animator.CrossFade("DancingCam", 0);
    }

    public void RotateDancingCam()
    {
        dancingCamPov.m_HorizontalAxis.m_InputAxisValue = 0.15f;
    }

    private void OnNextLevelStarting()
    {
        animator.CrossFade("RunnerCam", 0);
    }
}
