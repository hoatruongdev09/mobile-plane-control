using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TornadoLiveState : TornadoState {

    private Vector3 randomPosition;
    public TornadoLiveState (Tornado tornado, StateMachine stateMachine) : base (tornado, stateMachine) {

    }
    public override void Enter () {
        Debug.Log ("enter live state");
        randomPosition = MapManager.Instance.GetRandomPosition ();
        MoveToPosition ();
    }

    public override void Exit () {
        LeanTween.cancel (Tornado.gameObject);
    }
    private void MoveToPosition () {
        Vector3 currentPosition = Tornado.transform.position;
        float leanTime = (randomPosition - currentPosition).magnitude / 2;
        Debug.Log ($"leantime: {leanTime}");
        LeanTween.value (Tornado.gameObject, currentPosition, randomPosition, leanTime).setOnUpdate ((Vector3 value) => {
            Tornado.transform.position = value;
        }).setOnComplete (() => {
            StateMachine.ChangeState (Tornado.liveState);
        });
    }

}