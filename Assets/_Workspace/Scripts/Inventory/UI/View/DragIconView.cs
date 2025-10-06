using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [VIEW] - MonoBehaviour, управляющий иконкой, которая следует за курсором мыши
/// во время перетаскивания предмета (drag & drop).
/// </summary>
[RequireComponent(typeof(Image))] // Гарантирует, что на этом GameObject всегда будет компонент Image.
public class DragIconView : MonoBehaviour
{
    private Image _iconImage;

    /// <summary>
    /// Инициализация компонента при его создании.
    /// </summary>
    private void Awake()
    {
        _iconImage = GetComponent<Image>();

        Hide();
    }

    /// <summary>
    /// Вызывается каждый кадр. Обновляет позицию иконки.
    /// </summary>
    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// Показывает иконку и устанавливает для нее нужный спрайт предмета.
    /// </summary>
    /// <param name="icon">Спрайт предмета, который нужно отобразить.</param>
    public void Show(Sprite icon)
    {
        _iconImage.sprite = icon;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Прячет иконку, делая ее неактивной.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}