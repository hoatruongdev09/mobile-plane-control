using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour {
    public float LifeTime { get { return lifeTime; } set { lifeTime = value; } }
    public OnTornadoDie onTornadoDie { get; set; }
    private float lifeTime = 15;
    public float distanceToChangePosition = 2;
    private StateMachine stateMachine;
    public TornadoRiseState riseState;
    public TornadoLiveState liveState;
    public TornadoDieState dieState;
    public delegate void OnTornadoDie (Tornado tornado);
    private void Start () {
        stateMachine = new StateMachine ();
        riseState = new TornadoRiseState (this, stateMachine);
        liveState = new TornadoLiveState (this, stateMachine);
        dieState = new TornadoDieState (this, stateMachine);
        stateMachine.Start (riseState);
        StartCoroutine (DelayToDie ());
    }

    private IEnumerator DelayToDie () {
        float lifeBonusTime = Random.Range (5f, 10f);
        yield return new WaitForSeconds (lifeTime + lifeBonusTime);
        stateMachine.ChangeState (dieState);
    }
    public void DestroySelf () {
        Destroy (gameObject);
    }
}