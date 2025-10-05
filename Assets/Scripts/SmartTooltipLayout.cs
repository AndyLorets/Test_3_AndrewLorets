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
        // ��������� LayoutElement, ���� ��� ���. �� ����� ��� �������������� � LayoutGroup.
        _layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
    }

    // ���� ����� ����� ���������� �� TooltipView, ����� ����� ���������
    public void UpdateLayout()
    {
        // �������� ���������������� ������ ������, ��� ���� �� �� ��� � ���� ������
        float preferredWidth = _descriptionText.GetPreferredValues().x;

        // ������������ ������ ����� min � max
        float clampedWidth = Mathf.Clamp(preferredWidth, _minWidth, _maxWidth);

        // ������������� ��� ������ ��� ������ ��������� ������� TooltipPanel.
        // Vertical Layout Group ������ ��� � �������� ��� ������ ���� �������� ���������,
        // ��������� ����� ������������ �� ����� ������, ���� ��� ����������.
        _layoutElement.preferredWidth = clampedWidth;
    }
}