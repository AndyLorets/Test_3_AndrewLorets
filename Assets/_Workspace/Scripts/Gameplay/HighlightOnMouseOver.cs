using DG.Tweening;
using UnityEngine;

/// <summary>
/// [GAMEPLAY / UTILITY] - ������������� ��������� ��� ���������� ������� ������� �� �������� ����.
/// �������� ��:
/// 1. ������� ��������� ������� ��� ��������� �������.
/// 2. �������� "�������" (������� ������) ��� �����.
/// ���� ������ ������ �� ����� �� ��������� � ����� ���� ����������� �� ����� 2D-�������.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class HighlightOnMouseOver : MonoBehaviour
{
    [Header("��������� ���������")]
    [SerializeField] private Color _highlightColor = Color.yellow;
    [SerializeField] private float _fadeDuration = 0.2f;

    [Header("��������� �������� �������")]
    [Tooltip("��������� ������ ������ �������� ��� �����")]
    [SerializeField] private Vector3 _punchScale = new Vector3(-0.1f, -0.1f, 0);
    [Tooltip("��� ����� ����� ������� �������� �������")]
    [SerializeField] private float _punchDuration = 0.3f;
    [Tooltip("��������� '�����������' ����� ��������")]
    [SerializeField] private int _punchVibrato = 5;
    [Tooltip("��������� '����������' ����� �������� (�� 0 �� 1)")]
    [SerializeField] private float _punchElasticity = 1f;


    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Vector3 _originalScale;
    private Tween _currentPunchTween; 

    /// <summary>
    /// �������������, ����������� ��������� ��������.
    /// </summary>
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        _originalScale = transform.localScale;
    }

    /// <summary>
    /// ����������� ��� ��������� ������� �� ��������� �������.
    /// </summary>
    private void OnMouseEnter()
    {
        if (GameState.IsUiOpen) return;

        _spriteRenderer.DOColor(_highlightColor, _fadeDuration);
    }

    /// <summary>
    /// �����������, ����� ������ �������� ��������� �������.
    /// </summary>
    private void OnMouseExit()
    {
        _spriteRenderer.DOColor(_originalColor, _fadeDuration);
    }

    /// <summary>
    /// ����������� ��� ����� �� ��������� �������.
    /// </summary>
    private void OnMouseDown()
    {
        if (GameState.IsUiOpen) return;

        _currentPunchTween?.Kill();
        _currentPunchTween = transform.DOPunchScale(_punchScale, _punchDuration, _punchVibrato, _punchElasticity)
                                      .SetRelative(true) 
                                      .OnComplete(() => transform.localScale = _originalScale); 
    }
}