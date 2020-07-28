using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportManager : MonoBehaviour {
    public List<Airport> Airports {
        get { return airPorts; }
        set { airPorts = value; }
    }

    [SerializeField] private List<Airport> airPorts;

    public Airport RandomAirport () {
        return airPorts[UnityEngine.Random.Range (0, airPorts.Count)];
    }
}