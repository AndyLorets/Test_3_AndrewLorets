using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;


public enum InventoryPanelType
{
    Player,
    Container
}


/// <summary>
/// [VIEW] - ������� MonoBehaviour ��� ����� ���� ���������.
/// �������� ��:
/// 1. �����������/������� ����� ����.
/// 2. �������� �������� ������������� (InventoryView) ��� ����� � ������ �������.
/// 3. ��������� ���������� ��� ������ ������.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class InventoryWindowView : MonoBehaviour
{
    [Header("���������� ��� ������")]
    [SerializeField] private Transform _playerSlotsContainer;
    [SerializeField] private Transform _containerSlotsContainer;

    [Header("UI ��������")]
    [SerializeField] private InventorySlotView _slotPrefab;
    [SerializeField] private TextMeshProUGUI _playerInventoryTitle;
    [SerializeField] private TextMeshProUGUI _containerInventoryTitle;
    [SerializeField] private Button _closeBtn;

    [Header("������ ����������")]
    [SerializeField] private Button _playerSortTypeBtn;
    [SerializeField] private Button _playerSortNameBtn;
    [SerializeField] private Button _playerSortQuantityBtn;
    [Space(5)]
    [SerializeField] private Button _containerSortTypeBtn;
    [SerializeField] private Button _containerSortNameBtn;
    [SerializeField] private Button _containerSortQuantityBtn;

    public InventoryView PlayerInventoryView { get; private set; }
    public InventoryView ContainerInventoryView { get; private set; }

    private Tween _activeFadeTween;
    private CanvasGroup _canvasGroup;

    /// <summary>
    /// ����, ������������, ������� �� ���� � ������ ������.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// �������, ������� ���������� ��� ������� �� ������ ����������.
    /// </summary>
    public event Action<InventoryPanelType, SortCriteria> OnSortButtonClicked;
    public event System.Action OnCloseButtonClicked;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        ClearPanels();
        InitButtons();
        gameObject.SetActive(false);
    }

    private void InitButtons()
    {
        _playerSortTypeBtn.onClick.AddListener(() => OnSortButtonClicked?.Invoke(InventoryPanelType.Player, SortCriteria.ByType));
        _playerSortNameBtn.onClick.AddListener(() => OnSortButtonClicked?.Invoke(InventoryPanelType.Player, SortCriteria.ByName));
        _playerSortQuantityBtn.onClick.AddListener(() => OnSortButtonClicked?.Invoke(InventoryPanelType.Player, SortCriteria.ByQuantity));

        _containerSortTypeBtn.onClick.AddListener(() => OnSortButtonClicked?.Invoke(InventoryPanelType.Container, SortCriteria.ByType));
        _containerSortNameBtn.onClick.AddListener(() => OnSortButtonClicked?.Invoke(InventoryPanelType.Container, SortCriteria.ByName));
        _containerSortQuantityBtn.onClick.AddListener(() => OnSortButtonClicked?.Invoke(InventoryPanelType.Container, SortCriteria.ByQuantity));

        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(() => OnCloseButtonClicked?.Invoke());
    }

    /// <summary>
    /// �������� ����� ��� �������� � ������������� ���� � ������� �������.
    /// </summary>
    /// <param name="player">������������� ������ ������, ��������������� ��������� � ���.</param>
    /// <param name="container">������������� ������ ���������� (����� ���� null).</param>
    public void Open(IInteractable player, IInteractable container)
    {
        if (IsOpen) return; 

        IsOpen = true;

        gameObject.SetActive(true);
        _activeFadeTween?.Kill();

        ClearPanels();

        _playerInventoryTitle.text = player.ContainerName;
        PlayerInventoryView = new InventoryView(_playerSlotsContainer, _slotPrefab, player.Inventory);
        PlayerInventoryView.Initialize();

        bool hasContainer = container != null;
        _containerSlotsContainer.gameObject.SetActive(hasContainer);
        _containerInventoryTitle.gameObject.SetActive(hasContainer);
        _containerSortTypeBtn.transform.parent.gameObject.SetActive(hasContainer);

        if (hasContainer)
        {
            _containerInventoryTitle.text = container.ContainerName;
            ContainerInventoryView = new InventoryView(_containerSlotsContainer, _slotPrefab, container.Inventory);
            ContainerInventoryView.Initialize();
        }

        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _activeFadeTween = _canvasGroup.DOFade(1f, 0.3f);
    }

    /// <summary>
    /// ������ ��������� � �������� ���� ���������.
    /// </summary>
    public void Hide()
    {
        if (!IsOpen) return;

        IsOpen = false;

        _activeFadeTween?.Kill();

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _activeFadeTween = _canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            ClearPanels();
        });
    }

    /// <summary>
    /// "�������". ���������� ��� ��������� UI-����� � ������������ �� ������� �������.
    /// </summary>
    private void ClearPanels()
    {
        foreach (Transform child in _playerSlotsContainer) Destroy(child.gameObject);
        foreach (Transform child in _containerSlotsContainer) Destroy(child.gameObject);

        PlayerInventoryView?.Dispose();
        ContainerInventoryView?.Dispose();
        PlayerInventoryView = null;
        ContainerInventoryView = null;
    }
}