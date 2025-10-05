using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragIconView : MonoBehaviour
{
    private Image _iconImage;

    private void Awake()
    {
        _iconImage = GetComponent<Image>();
        Hide();
    }

    void Update()
    {
        // »конка следует за курсором
        if (gameObject.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void Show(Sprite icon)
    {
        _iconImage.sprite = icon;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}