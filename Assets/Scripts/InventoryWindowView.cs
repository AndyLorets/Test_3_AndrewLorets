using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryWindowView : MonoBehaviour
{
    [SerializeField] private Transform _playerSlotsContainer;
    [SerializeField] private Transform _containerSlotsContainer;
    [SerializeField] private InventorySlotView _slotPrefab;
    [SerializeField] private TextMeshProUGUI _playerInventoryTitle; // <-- ���������
    [SerializeField] private TextMeshProUGUI _containerInventoryTitle; // <-- ���������

    // ������� �� ����������, ����� ���������� ��� � ��� ����������
    public InventoryView PlayerInventoryView { get; private set; }
    public InventoryView ContainerInventoryView { get; private set; }


    private CanvasGroup _canvasGroup;
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        // �����: �� ������ ������� ������, ����� �� ���� "������" �� ������� ��������
        ClearPanels();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ������� �����. ��������� ���� � ����������� � �� ����������.
    /// </summary>
    public void Open(IInteractable player, IInteractable container) // <-- �������� ���������
    {
        IsOpen = true;
        gameObject.SetActive(true);

        ClearPanels();

        // --- ������ ��������� ---

        // 1. ������������� �������� � View ��� ������ (������)
        _playerInventoryTitle.text = player.ContainerName;
        PlayerInventoryView = new InventoryView(_playerSlotsContainer, _slotPrefab, player.Inventory);
        PlayerInventoryView.Initialize();

        // 2. ������������� �������� � View ��� ���������� (������ ���� �� ����)
        bool hasContainer = container != null;
        _containerSlotsContainer.gameObject.SetActive(hasContainer);
        _containerInventoryTitle.gameObject.SetActive(hasContainer);

        if (hasContainer)
        {
            _containerInventoryTitle.text = container.ContainerName;
            ContainerInventoryView = new InventoryView(_containerSlotsContainer, _slotPrefab, container.Inventory);
            ContainerInventoryView.Initialize();
        }

        // ������ ���������� ����
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1f, 0.3f);
    }

    public void Hide()
    {
        IsOpen = false;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            ClearPanels(); // ������� ����� ��������
        });
    }

    private void ClearPanels()
    {
        // ���������� ������ UI-�����
        foreach (Transform child in _playerSlotsContainer) Destroy(child.gameObject);
        foreach (Transform child in _containerSlotsContainer) Destroy(child.gameObject);

        // ������������ �� ������� ������ �������, ����� �������� ������ ������
        PlayerInventoryView?.Dispose();
        ContainerInventoryView?.Dispose();
        PlayerInventoryView = null;
        ContainerInventoryView = null;
    }
}