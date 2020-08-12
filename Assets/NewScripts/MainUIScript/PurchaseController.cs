using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaseController : MonoBehaviour {
    public static PurchaseController Instance { get; set; }
    public string RemoveAdID { get { return removeAdId; } }
    public bool IsInitialized {
        get { return StoreController != null && StoreProvider != null; }
    }
    private string removeAdId = "remove_ad";
    public IStoreListener StoreListener { get; set; }
    public IStoreController StoreController { get; set; }
    public IExtensionProvider StoreProvider { get; set; }

    private void Awake () {
        var purchaseControll = FindObjectOfType<PurchaseController> ();
        if (purchaseControll != this) {
            Destroy (gameObject);
        } else {
            DontDestroyOnLoad (gameObject);
        }
        if (Instance == null) {
            Instance = this;
        }
    }
    public void Initialize () {
        if (IsInitialized) { return; }
        var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
        builder.AddProduct (removeAdId, ProductType.NonConsumable);
        UnityPurchasing.Initialize (StoreListener, builder);
    }
    public void Initialize (IStoreListener listener) {
        if (IsInitialized) { return; }
        var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
        builder.AddProduct (removeAdId, ProductType.NonConsumable);
        StoreListener = listener;
        UnityPurchasing.Initialize (listener, builder);
    }
    public void PurchaseRemoveAd () {
        Debug.Log ("purchase ad in controller");
        var product = StoreController.products.WithID (RemoveAdID);
        StoreController.InitiatePurchase (product);
    }
    public bool CheckIfRemoveAdPurchased () {
        return StoreController.products.WithID (removeAdId).hasReceipt;
    }
}