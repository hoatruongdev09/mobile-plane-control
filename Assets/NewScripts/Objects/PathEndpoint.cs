using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class PathEndpoint : MonoBehaviour {
    public NewScript.Path Path { get { return path; } }

    [SerializeField] private NewScript.Path path;
}