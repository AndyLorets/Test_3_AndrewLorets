using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// [VIEW] - Главный MonoBehaviour для всего окна инвентаря.
/// Отвечает за:
/// 1. Отображение/скрытие всего окна.
/// 2. Создание дочерних представлений (InventoryView) для левой и правой панелей.
/// 3. Установку заголовков для каждой панели.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class InventoryWindowView : MonoBehaviour
{
    [Header("Контейнеры для слотов")]
    [SerializeField] private Transform _playerSlotsContainer;
    [SerializeField] private Transform _containerSlotsContainer;

    [Header("UI Элементы")]
    [SerializeField] private InventorySlotView _slotPrefab;
    [SerializeField] private TextMeshProUGUI _playerInventoryTitle;
    [SerializeField] private TextMeshProUGUI _containerInventoryTitle;
    [SerializeField] private Button _closeBtn;
    public InventoryView PlayerInventoryView { get; private set; }
    public InventoryView ContainerInventoryView { get; private set; }

    private Tween _activeFadeTween;
    private CanvasGroup _canvasGroup;

    /// <summary>
    /// Флаг, показывающий, открыто ли окно в данный момент.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// Первоначальная настройка при запуске.
    /// </summary>
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        ClearPanels();
        gameObject.SetActive(false);

        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(Hide);
    }

    /// <summary>
    /// Основной метод для открытия и инициализации окна с нужными данными.
    /// </summary>
    /// <param name="player">Интерактивный объект игрока, предоставляющий инвентарь и имя.</param>
    /// <param name="container">Интерактивный объект контейнера (может быть null).</param>
    public void Open(IInteractable player, IInteractable container)
    {
        if (IsOpen) return; 

        IsOpen = true;
        GameState.IsUiOpen = true;
        gameObject.SetActive(true);
        _activeFadeTween?.Kill();

        ClearPanels();

        _playerInventoryTitle.text = player.ContainerName;
        PlayerInventoryView = new InventoryView(_playerSlotsContainer, _slotPrefab, player.Inventory);
        PlayerInventoryView.Initialize();

        bool hasContainer = container != null;
        _containerSlotsContainer.gameObject.SetActive(hasContainer);
        _containerInventoryTitle.gameObject.SetActive(hasContainer);

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
    /// Плавно закрывает и скрывает окно инвентаря.
    /// </summary>
    public void Hide()
    {
        if (!IsOpen) return;

        GameState.IsUiOpen = false;
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
    /// "Уборщик". Уничтожает все созданные UI-слоты и отписывается от событий моделей.
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