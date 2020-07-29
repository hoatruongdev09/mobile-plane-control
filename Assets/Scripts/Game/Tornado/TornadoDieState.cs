using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TornadoDieState : TornadoState {
    public TornadoDieState (Tornado tornado, StateMachine stateMachine) : base (tornado, stateMachine) {

    }
    public override void Enter () {
        Debug.Log ("enter die state");
        Dying ();
    }

    public override void Exit () {

    }
    private void Dying () {
        Tornado.transform.LeanScale (Vector3.zero, 1f).setOnComplete (() => {
            this.Tornado.onTornadoDie?.Invoke (this.Tornado);
            // Tornado.Destroy (Tornado.gameObject);
        });
    }
}