using DG.Tweening;
using TMPro;
using UnityEngine;

public class InteractionIndicator : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] private TMP_Text _interactionDirectiveText;

    [Header("DOTween attributes")]
    private Tween _appearingTween;
    [SerializeField] private float _appearingDuration;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(bool hasOreAttached)
    {
        gameObject.SetActive(true);
        AppearEffect();
        SetInteracitonDirectiveText(hasOreAttached);
    }

    public void DeInitialize()
    {
        DisappearEffect();
    }

    private void AppearEffect()
    {
        KillCurrentTween();
        _appearingTween = _rectTransform.DOScale(Vector3.one, _appearingDuration);
    }

    private void DisappearEffect()
    {
        KillCurrentTween();
        _appearingTween = _rectTransform.DOScale(Vector3.zero, _appearingDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
    
    private void KillCurrentTween()
    {
        if (_appearingTween != null && _appearingTween.IsPlaying())
        {
            _appearingTween.Kill();
        }
    }

    public void SetInteracitonDirectiveText(bool hasOreAttached)
    {
        _interactionDirectiveText.text = hasOreAttached ? "PLACE" : _interactionDirectiveText.text = "INTERACT";
    }
}
