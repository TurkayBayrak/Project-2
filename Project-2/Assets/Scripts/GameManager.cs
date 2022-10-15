using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    private bool isGameOver;

    private bool isFirstStack;
    private Transform _previousStackTransform;
    private Transform _currentStackTransform;
    private int _index;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetStackAndSendNextOne();
        }
    }

    private void Start()
    {
        StartCoroutine(WaitCo());
    }

    private void SetStackAndSendNextOne()
    {
        DOTween.KillAll();
        CutStack();
        GetNextStack();
    }

    private void CutStack()
    {
        if (_currentStackTransform.position.x > 0)
        {
            var position = _currentStackTransform.position;
            var localScale = _currentStackTransform.localScale;

            var newPositionX = position.x - position.x / 2;
            var newScaleX = localScale.x - position.x;

            var newPos = new Vector3(newPositionX, position.y, position.z);
            var newScale = new Vector3(newScaleX, localScale.y, localScale.z);
            
            _currentStackTransform.position = newPos;
            _currentStackTransform.localScale = newScale;
        }
        else
        {
            var position = _currentStackTransform.position;
            var localScale = _currentStackTransform.localScale;

            var newPositionX = position.x + position.x / 2;
            var newScaleX = localScale.x + position.x;

            var newPos = new Vector3(newPositionX, position.y, position.z);
            var newScale = new Vector3(newScaleX, localScale.y, localScale.z);
            
            _currentStackTransform.position = newPos;
            _currentStackTransform.localScale = newScale;
        }
    }

    private void GetNextStack()
    {
        if (_index != parentTransform.childCount - 1)
            _index++;
        else
            _index = 0;
        
        var stack = ObjectPooler.instance.GetPooledObject("Stack", _index);

        _previousStackTransform = _currentStackTransform;

        stack.transform.localScale = _previousStackTransform.localScale;
        _currentStackTransform = stack.transform;

        stack.SetActive(true);
        SendNextStack();
    }

    private void SendNextStack()
    {
        // var totalStackCount = parentTransform.childCount;

        // previousStackTransform = currentStackTransform.GetSiblingIndex() != 0 ? parentTransform.GetChild(currentStackTransform.GetSiblingIndex() - 1) : parentTransform.GetChild(totalStackCount - 1);

        var zAdjustmentPre = _previousStackTransform.localScale.z / 2;
        var zAdjustmentCurrent = _currentStackTransform.localScale.z / 2;
        var totalAddition = zAdjustmentPre + zAdjustmentCurrent;

        var position = _previousStackTransform.position;
        var pos = new Vector3(position.x - 20, -0.5f, position.z + totalAddition);
        _currentStackTransform.position = pos;

        _currentStackTransform.DOMoveX(20, 3);
    }

    IEnumerator WaitCo()
    {
        yield return new WaitForSeconds(1f);
        var stack = ObjectPooler.instance.GetPooledObject("Stack", 0);
        _currentStackTransform = stack.transform;
        stack.SetActive(true);

        _previousStackTransform = GameObject.Find("FirstStack").transform;
        var zAdjustmentPre = _previousStackTransform.localScale.z / 2;
        var zAdjustmentCurrent = stack.transform.localScale.z / 2;
        var totalAddition = zAdjustmentPre + zAdjustmentCurrent;

        var position = _previousStackTransform.position;
        var pos = new Vector3(position.x - 20, -0.5f, position.z + totalAddition);
        stack.transform.position = pos;

        stack.transform.DOMoveX(20, 3);
    }
}
