#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class MapFileGenerator : MonoBehaviour {
    public Sprite mapImage;
    public LevelDataInfo levelDataInfo;

    public bool enemies;
    public bool fire;
    public bool cloud;
    public bool tornado;
    public bool fuel;
    public string mapData;

    public void GenerateMap () {
        try {
            LevelDataModel levelModel = new LevelDataModel ();
            levelDataInfo.levelImage = GetFileNameWithOutString (AssetDatabase.GetAssetPath (mapImage));
            levelModel.info = levelDataInfo;
            levelModel.sceneBackground = GetLevelBackground ();
            levelModel.levelBackground = GetBackground ();
            levelModel.fire = fire;
            levelModel.enemies = enemies;
            levelModel.cloud = cloud;
            levelModel.tornado = tornado;
            levelModel.fuel = fuel;
            levelModel.planeId = CreatePlanesData ();
            levelModel.airport = CreateAirportData ();
            mapData = JsonUtility.ToJson (levelModel);
        } catch (Exception e) {
            Debug.LogError (e);
        }
    }
    private MapImageModel GetBackground () {
        var data = new MapImageModel ();
        var graphicController = FindObjectOfType<MapGraphicController> ();
        data.background = GetFileNameWithOutString (AssetDatabase.GetAssetPath (graphicController.mapBackground.sprite));
        data.position = VectorModel.FromVector3 (graphicController.mapBackground.transform.position);
        data.scale = VectorModel.FromVector3 (graphicController.mapBackground.transform.localScale);
        data.rotation = graphicController.mapBackground.transform.rotation.eulerAngles.z;
        return data;
    }
    private MapImageModel GetLevelBackground () {
        var data = new MapImageModel ();
        var graphicController = FindObjectOfType<MapGraphicController> ();
        data.background = GetFileNameWithOutString (AssetDatabase.GetAssetPath (graphicController.sceneBackground.sprite));
        data.position = VectorModel.FromVector3 (graphicController.sceneBackground.transform.position);
        data.scale = VectorModel.FromVector3 (graphicController.sceneBackground.transform.localScale);
        data.rotation = graphicController.sceneBackground.transform.rotation.eulerAngles.z;
        return data;
    }
    private int[] CreatePlanesData () {
        var listPlane = FindObjectsOfType<PlaneControl> ();
        List<int> listPlanes = new List<int> ();
        foreach (var plane in listPlane) {
            var id = plane.name.Replace ("plane", "");
            listPlanes.Add (int.Parse (id));
        }
        return listPlanes.ToArray ();
    }
    private AirportDataModel[] CreateAirportData () {
        var airports = FindObjectsOfType<Airport> ();
        var airportData = new AirportDataModel[airports.Length];
        for (int i = 0; i < airports.Length; i++) {
            var airport = airports[i];
            airportData[i] = new AirportDataModel () {
                name = airport.name,
                type = (airport.GetType () == typeof (Helipad)) ? "round" : "long",
                position = new VectorModel () {
                x = airport.transform.position.x,
                y = airport.transform.position.y,
                },
                rotation = airport.transform.rotation.eulerAngles.z,
                planeTag = airport.PlaneTag,
                color = $"#{ColorUtility.ToHtmlStringRGB (airport.ColorTag)}",
                scale = new VectorModel {
                x = airport.transform.localScale.x,
                y = airport.transform.localScale.y
                },
            };
        }
        return airportData;
    }
    private string GetFileNameWithOutString (string fileLocation) {
        return Path.GetFileNameWithoutExtension (fileLocation);
    }
}
#endif