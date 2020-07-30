using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWaterDrop : MonoBehaviour {
    public ParticleSystem waterParticle;

    private ParticleSystem.MainModule waterMainModule;
    private void Start () {
        waterMainModule = waterParticle.main;
    }
    public void Drop () {
        waterMainModule.startLifetime = 3;
    }
    public void Stop () {
        waterMainModule.startLifetime = 0;
    }
}