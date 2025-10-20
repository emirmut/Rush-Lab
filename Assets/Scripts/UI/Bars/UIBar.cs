using DG.Tweening;
using UnityEngine;

public class UIBar : MonoBehaviour
{
    protected RectTransform RectTransform;

    [Header("DOTween attributes")]
    [SerializeField] protected float BarUpdateDuration;

    public void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void UpdateBar(float currentValue, float maxValue)
    {
        if (currentValue < 0f)
        {
            currentValue = 0f;
        }

        if (this is ExplosionBar)
        {
            BarUpdateDuration = maxValue;
        }

        RectTransform.DOScaleX(currentValue / maxValue, BarUpdateDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (currentValue <= 0)
            {
                if (this is HealthBar)
                {
                    Time.timeScale = 0f;
                }
            }
        });
    }

    public virtual void ResetBar()
    {
        gameObject.SetActive(false);
        RectTransform.localScale = Vector3.one;
    }
}
