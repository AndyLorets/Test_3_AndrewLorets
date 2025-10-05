using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class HighlightOnMouseOver : MonoBehaviour
{
    [Tooltip("����, � ������� ����� ������������ ������ ��� ��������� ����")]
    [SerializeField] private Color _highlightColor = Color.yellow;

    [Tooltip("��������, � ������� ����� �������� ����")]
    [SerializeField] private float _fadeDuration = 0.2f;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    private void OnMouseEnter()
    {
        // ������ ������ ���� �� ���� ���������
        _spriteRenderer.DOColor(_highlightColor, _fadeDuration);
    }

    private void OnMouseExit()
    {
        // ������ ���������� ������������ ����
        _spriteRenderer.DOColor(_originalColor, _fadeDuration);
    }
}