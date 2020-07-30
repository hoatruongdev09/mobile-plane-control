using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NewScript {
    public class Path : MonoBehaviour {
        public PlaneControl Controller { get; set; }
        public float pointDistance = 3;
        public int CurrentPointCount {
            get { return lineVisual.positionCount; }
            set { if (value >= 0) { lineVisual.positionCount = value; } }
        }

        public PathEndpoint endPoint;
        public LineRenderer lineVisual;
        public void DeactivateEndPoint (bool action) {
            endPoint.gameObject.SetActive (!action);
            endPoint.transform.parent = null;
            try {
                endPoint.transform.position = GetLastpoint ();
            } catch (Exception e) {
                endPoint.transform.position = Vector3.zero;
            }
        }
        public void Clear () {
            CurrentPointCount = 1;
        }
        public void AddPoint (Vector3 position) {
            position.z = 0;
            if (CurrentPointCount <= 1) {
                lineVisual.positionCount++;
                lineVisual.SetPosition (CurrentPointCount - 1, position);
                return;
            }
            try {
                float distance = (position - GetLastpoint ()).sqrMagnitude;
                if (distance >= pointDistance * pointDistance) {
                    lineVisual.positionCount++;
                    lineVisual.SetPosition (CurrentPointCount - 1, position);
                }
            } catch (Exception e) {
                Debug.Log (e.Message);
            }
        }
        public Vector3 GetLastpoint () {
            if (CurrentPointCount == 0) {
                throw new Exception ("Point count is zero");
            }
            return lineVisual.GetPosition (CurrentPointCount - 1);
        }
        public Vector3 GetStartPoint () {
            try {
                return lineVisual.GetPosition (0);
            } catch (Exception e) {
                throw e;
            }
        }
        public Vector3 GetNextPoint (int index) {
            Vector3 position;
            if (index + 1 >= CurrentPointCount) {
                throw new Exception ($"{index + 1} out of bound {CurrentPointCount}");
            }
            position = lineVisual.GetPosition (index + 1);
            return position;
        }

        public void SetFirstIndexPosition (Vector3 position) {
            if (CurrentPointCount <= 0) {
                return;
            }
            lineVisual.SetPosition (0, position);
        }
        public void SetPosition (int index, Vector3 position) {
            if (index >= CurrentPointCount) {
                return;
            }
            lineVisual.SetPosition (index, position);
        }
        private void OnDisable () {
            try {
                Destroy (endPoint.gameObject);
            } catch (Exception e) {

            }
        }

    }

}