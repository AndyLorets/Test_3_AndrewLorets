using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class HighlightOnMouseOver : MonoBehaviour
{
    [Tooltip("Цвет, в который будет окрашиваться спрайт при наведении мыши")]
    [SerializeField] private Color _highlightColor = Color.yellow;

    [Tooltip("Скорость, с которой будет меняться цвет")]
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
        // Плавно меняем цвет на цвет подсветки
        _spriteRenderer.DOColor(_highlightColor, _fadeDuration);
    }

    private void OnMouseExit()
    {
        // Плавно возвращаем оригинальный цвет
        _spriteRenderer.DOColor(_originalColor, _fadeDuration);
    }
}