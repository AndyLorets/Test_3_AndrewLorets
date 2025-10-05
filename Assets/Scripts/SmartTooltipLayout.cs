using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SmartTooltipLayout : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private float _minWidth = 150f;
    [SerializeField] private float _maxWidth = 300f;

    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        // Добавляем LayoutElement, если его нет. Он нужен для взаимодействия с LayoutGroup.
        _layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
    }

    // Этот метод будет вызываться из TooltipView, когда текст обновится
    public void UpdateLayout()
    {
        // Получаем предпочтительную ширину текста, как если бы он был в одну строку
        float preferredWidth = _descriptionText.GetPreferredValues().x;

        // Ограничиваем ширину между min и max
        float clampedWidth = Mathf.Clamp(preferredWidth, _minWidth, _maxWidth);

        // Устанавливаем эту ширину для нашего корневого объекта TooltipPanel.
        // Vertical Layout Group увидит это и передаст эту ширину всем дочерним элементам,
        // заставляя текст переноситься на новые строки, если это необходимо.
        _layoutElement.preferredWidth = clampedWidth;
    }
}