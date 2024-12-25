using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaseController : MonoBehaviour
{
    public static PurchaseController Instance { get; set; }
    public string RemoveAdID { get { return removeAdId; } }
    public string UnlockAllLevelsdID { get { return unlockAllLevel; } }
    public string UnlockLevelID { get { return unlockLevel; } }
    public bool IsInitialized
    {
        get { return StoreController != null && StoreProvider != null; }
    }
    private string removeAdId = "remove_ad";
    private string unlockAllLevel = "unlock_all_level";
    private string unlockLevel = "unlock_level_";
    public IStoreListener StoreListener { get; set; }
    public IStoreController StoreController { get; set; }
    public IExtensionProvider StoreProvider { get; set; }

    private void Awake()
    {
        var purchaseControll = FindObjectOfType<PurchaseController>();
        if (purchaseControll != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Initialize()
    {
        if (IsInitialized) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(removeAdId, ProductType.NonConsumable);
        UnityPurchasing.Initialize(StoreListener, builder);
    }

    public void Initialize(IStoreListener listener)
    {
        if (IsInitialized) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(removeAdId, ProductType.NonConsumable);
        builder.AddProduct(unlockAllLevel, ProductType.NonConsumable);
        StoreListener = listener;
        UnityPurchasing.Initialize(listener, builder);
    }
    public void Initialize(IStoreListener listener, string[] listLevelName)
    {
        if (IsInitialized) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(removeAdId, ProductType.NonConsumable);
        builder.AddProduct(unlockAllLevel, ProductType.NonConsumable);
        foreach (var level in listLevelName)
        {
            var normalLevelName = level.Replace(" ", ".");
            var productName = $"{unlockLevel}{normalLevelName.ToLower()}";
            Debug.Log($"init product: {unlockLevel}{normalLevelName.ToLower()}");
            if (productName == "unlock_level_island" || productName == "unlock_level_island.2")
            {
                Debug.Log($"not add product: {productName}");
                continue;
            }
            builder.AddProduct($"{unlockLevel}{normalLevelName.ToLower()}", ProductType.NonConsumable);
        }
        StoreListener = listener;
        UnityPurchasing.Initialize(listener, builder);
    }
    public void PurchaseRemoveAd()
    {
        Debug.Log("purchase ad in controller");
        var product = StoreController.products.WithID(RemoveAdID);
        StoreController.InitiatePurchase(product);
    }
    public void PurchaseUnlockAllLevels()
    {
        var product = StoreController.products.WithID(UnlockAllLevelsdID);
        StoreController.InitiatePurchase(product);
    }
    public void PurchaseLevel(string levelName)
    {
        var product = StoreController.products.WithID($"{unlockLevel}{levelName.Replace(" ", ".").ToLower()}");
        StoreController.InitiatePurchase(product);
    }
    public bool CheckIfUnlockAllLevelPurchased()
    {
#if USE_PURCHASE
        return StoreController.products.WithID (UnlockAllLevelsdID).hasReceipt;
#endif
        return true;
    }
    public bool CheckIfRemoveAdPurchased()
    {
        return StoreController.products.WithID(RemoveAdID).hasReceipt;
    }
}