using DG.Tweening;
using UnityEngine;

/// <summary>
/// [GAMEPLAY / UTILITY] - Универсальный компонент для визуальной реакции объекта на действия мыши.
/// Отвечает за:
/// 1. Плавную подсветку объекта при наведении курсора.
/// 2. Анимацию "нажатия" (упругое сжатие) при клике.
/// Этот скрипт ничего не знает об инвентаре и может быть использован на любом 2D-объекте.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class HighlightOnMouseOver : MonoBehaviour
{
    [Header("Настройки подсветки")]
    [SerializeField] private Color _highlightColor = Color.yellow;
    [SerializeField] private float _fadeDuration = 0.2f;

    [Header("Настройки анимации нажатия")]
    [Tooltip("Насколько сильно объект сожмется при клике")]
    [SerializeField] private Vector3 _punchScale = new Vector3(-0.1f, -0.1f, 0);
    [Tooltip("Как долго будет длиться анимация нажатия")]
    [SerializeField] private float _punchDuration = 0.3f;
    [Tooltip("Насколько 'вибрирующей' будет анимация")]
    [SerializeField] private int _punchVibrato = 5;
    [Tooltip("Насколько 'эластичной' будет анимация (от 0 до 1)")]
    [SerializeField] private float _punchElasticity = 1f;


    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Vector3 _originalScale;
    private Tween _currentPunchTween; 

    /// <summary>
    /// Инициализация, кэширование начальных значений.
    /// </summary>
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        _originalScale = transform.localScale;
    }

    /// <summary>
    /// Срабатывает при наведении курсора на коллайдер объекта.
    /// </summary>
    private void OnMouseEnter()
    {
        if (GameState.IsUiOpen) return;

        _spriteRenderer.DOColor(_highlightColor, _fadeDuration);
    }

    /// <summary>
    /// Срабатывает, когда курсор покидает коллайдер объекта.
    /// </summary>
    private void OnMouseExit()
    {
        _spriteRenderer.DOColor(_originalColor, _fadeDuration);
    }

    /// <summary>
    /// Срабатывает при клике на коллайдер объекта.
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