using UnityEngine;
using Zenject;
using DG.Tweening;

public class ItemPickupController : IInitializable
{
    private readonly PlayerInventoryHolder _playerInventoryHolder;
    private readonly Transform _playerTransform;

    public ItemPickupController(PlayerInventoryHolder playerHolder, [Inject(Id = "PlayerTransform")] Transform playerTransform)
    {
        _playerInventoryHolder = playerHolder;
        _playerTransform = playerTransform;
    }

    /// <summary>
    /// Подписываемся на глобальное событие запроса подбора предмета.
    /// </summary>
    public void Initialize()
    {
        WorldItem.OnItemPickupRequested += HandleItemPickupRequest;
    }

    /// <summary>
    /// Обрабатывает запрос на подбор предмета.
    /// </summary>
    private void HandleItemPickupRequest(WorldItem worldItem, System.Action<bool> onPickupAttempted)
    {
        bool wasAdded = _playerInventoryHolder.Inventory.TryAddItem(worldItem.ItemData, worldItem.Quantity);

        if (wasAdded)
            AnimateItemPickup(worldItem, onPickupAttempted);
        else
            onPickupAttempted?.Invoke(false);
    }


    /// <summary>
    /// Анимирует полет 2D-объекта на сцене к персонажу с эффектом "подпрыгивания".
    /// </summary>
    private void AnimateItemPickup(WorldItem worldItem, System.Action<bool> onPickupAttempted)
    {
        if (worldItem == null || worldItem.gameObject == null)
        {
            onPickupAttempted?.Invoke(false);
            return;
        }

        if (worldItem.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            spriteRenderer.sortingOrder += 10;

        Vector3 startPos = worldItem.transform.position;
        Vector3 peakPos = startPos + Vector3.up * 0.5f; 
        Vector3 endPos = _playerTransform.position + Vector3.up * 0.5f; 

        Sequence seq = DOTween.Sequence();

        seq.Append(
            worldItem.transform
                .DOJump(peakPos, 0.8f, 1, 0.45f)
                .SetEase(Ease.OutQuad)
        );
        seq.Join(
            worldItem.transform
                .DORotate(new Vector3(0, 0, 360), 0.4f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutSine)
        );

        seq.AppendInterval(0.05f);

        seq.Append(
            worldItem.transform
                .DOMove(endPos, 0.5f)
                .SetEase(Ease.InCubic)
        );
        seq.Join(
            worldItem.transform
                .DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InCubic)
        );

        seq.OnComplete(() =>
        {
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder -= 10;

            onPickupAttempted?.Invoke(true);
        });
    }

}