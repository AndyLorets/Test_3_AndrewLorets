using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [VIEW] - MonoBehaviour, ����������� �������, ������� ������� �� �������� ����
/// �� ����� �������������� �������� (drag & drop).
/// </summary>
[RequireComponent(typeof(Image))] // �����������, ��� �� ���� GameObject ������ ����� ��������� Image.
public class DragIconView : MonoBehaviour
{
    private Image _iconImage;

    /// <summary>
    /// ������������� ���������� ��� ��� ��������.
    /// </summary>
    private void Awake()
    {
        _iconImage = GetComponent<Image>();

        Hide();
    }

    /// <summary>
    /// ���������� ������ ����. ��������� ������� ������.
    /// </summary>
    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// ���������� ������ � ������������� ��� ��� ������ ������ ��������.
    /// </summary>
    /// <param name="icon">������ ��������, ������� ����� ����������.</param>
    public void Show(Sprite icon)
    {
        _iconImage.sprite = icon;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ������ ������, ����� �� ����������.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}