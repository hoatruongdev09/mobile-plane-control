using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour {
    public ICollisionCheckerDelegate CollisionCheckerDelegate { get; set; }
    public ITriggerCheckerDelegate TriggerCheckerDelegate { get; set; }
    public object OwnedInfo { get; set; }
    private void OnCollisionEnter2D (Collision2D other) {
        CollisionCheckerDelegate?.OnCollisionEnter2D (this, other);
    }
    private void OnCollisionExit2D (Collision2D other) {
        CollisionCheckerDelegate?.OnCollisionExit2D (this, other);
    }
    private void OnCollisionStay2D (Collision2D other) {
        CollisionCheckerDelegate?.OnCollisionStay2D (this, other);
    }
    private void OnTriggerEnter2D (Collider2D other) {
        TriggerCheckerDelegate?.OnTriggerEnter2D (this, other);
    }
    private void OnTriggerStay2D (Collider2D other) {
        TriggerCheckerDelegate?.OnTriggerStay2D (this, other);
    }
    private void OnTriggerExit2D (Collider2D other) {
        TriggerCheckerDelegate?.OnTriggerStay2D (this, other);
    }
}
public interface ICollisionCheckerDelegate {
    void OnCollisionEnter2D (ColliderChecker checker, Collision2D other);
    void OnCollisionExit2D (ColliderChecker checker, Collision2D other);
    void OnCollisionStay2D (ColliderChecker checker, Collision2D other);
}
public interface ITriggerCheckerDelegate {
    void OnTriggerEnter2D (ColliderChecker checker, Collider2D other);
    void OnTriggerStay2D (ColliderChecker checker, Collider2D other);
    void OnTriggerExit2D (ColliderChecker checker, Collider2D other);
}