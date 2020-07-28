using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour, ICollideChecker {
    public ICollisionCheckerDelegate CollisionCheckerDelegate { get; set; }
    public ITriggerCheckerDelegate TriggerCheckerDelegate { get; set; }
    public object OwnedInfo { get; set; }
    private void OnCollisionEnter2D (Collision2D other) {
        CollisionCheckerDelegate?.OnCheckerCollisionEnter2D (this, other);
    }
    private void OnCollisionExit2D (Collision2D other) {
        CollisionCheckerDelegate?.OnCheckerCollisionExit2D (this, other);
    }
    private void OnCollisionStay2D (Collision2D other) {
        CollisionCheckerDelegate?.OnCheckerCollisionStay2D (this, other);
    }
    private void OnTriggerEnter2D (Collider2D other) {
        TriggerCheckerDelegate?.OnCheckerTriggerEnter2D (this, other);
    }
    private void OnTriggerStay2D (Collider2D other) {
        TriggerCheckerDelegate?.OnCheckerTriggerStay2D (this, other);
    }
    private void OnTriggerExit2D (Collider2D other) {
        TriggerCheckerDelegate?.OnCheckerTriggerExit2D (this, other);
    }
}
public interface ICollideChecker {
    ICollisionCheckerDelegate CollisionCheckerDelegate { get; set; }
    ITriggerCheckerDelegate TriggerCheckerDelegate { get; set; }
    object OwnedInfo { get; set; }
}
public interface ICollisionCheckerDelegate {
    void OnCheckerCollisionEnter2D (ColliderChecker checker, Collision2D other);
    void OnCheckerCollisionExit2D (ColliderChecker checker, Collision2D other);
    void OnCheckerCollisionStay2D (ColliderChecker checker, Collision2D other);
}
public interface ITriggerCheckerDelegate {
    void OnCheckerTriggerEnter2D (ColliderChecker checker, Collider2D other);
    void OnCheckerTriggerStay2D (ColliderChecker checker, Collider2D other);
    void OnCheckerTriggerExit2D (ColliderChecker checker, Collider2D other);
}