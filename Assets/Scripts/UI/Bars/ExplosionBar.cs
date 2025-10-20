using TMPro;
using UnityEngine;

public class ExplosionBar : UIBar
{
    [SerializeField] private TMP_Text _explosionText;

    public void Initialize(float currentValue, float maxValue)
    {
        _explosionText.gameObject.SetActive(true);
        gameObject.SetActive(true);
        UpdateBar(currentValue, maxValue);
    }

    public override void ResetBar()
    {
        base.ResetBar();
        _explosionText.gameObject.SetActive(false);
    }
}
