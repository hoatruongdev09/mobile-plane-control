using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TornadoRiseState : TornadoState {
    public TornadoRiseState (Tornado tornado, StateMachine stateMachine) : base (tornado, stateMachine) {

    }
    public override void Enter () {
        Tornado.transform.localScale = Vector3.zero;
        SpawnController.Instance.CreateWarningSign (Tornado.transform.position, 3);
        RisingUp ();
    }
    public override void Exit () {

    }
    private void RisingUp () {
        Tornado.transform.LeanScale (Vector3.one, 1f).setEaseInBack ().setDelay (2).setOnComplete (() => {
            StateMachine.ChangeState (Tornado.liveState);
        });
    }
}