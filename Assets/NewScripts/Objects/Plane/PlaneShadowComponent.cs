using System.Collections.Generic;
using UnityEngine;

public class PlaneShadowComponent : MonoBehaviour, IPlaneComponent {
    public List<SpriteRenderer> listSpriteRenders = new List<SpriteRenderer> ();
    public void AttachToPlane (PlaneControl plane) {
        plane.Components?.Add (this);
        GenerateShadow (plane);
    }

    public void Effect (PlaneControl plane) {

    }

    public void UpdateEffect (PlaneControl plane) {
        Color tempColor;
        foreach (var shadow in listSpriteRenders) {
            shadow.transform.position = transform.position - GlobalShadow.Instance.ShadowDirect * GlobalShadow.Instance.ShadowLength * shadow.transform.localScale.x;
            shadow.transform.rotation = transform.rotation;
            shadow.transform.localScale = transform.localScale * GlobalShadow.Instance.ShadowSize;
            tempColor = shadow.color;
            tempColor.a = shadow.transform.localScale.x;
            shadow.color = tempColor;
        }
    }
    private void GenerateShadow (PlaneControl plane) {
        foreach (var graphic in plane.graphics) {
            var shadow = new GameObject ($"{graphic.name}-shadow");
            shadow.transform.position = plane.transform.position;
            var render = shadow.AddComponent<SpriteRenderer> ();
            render.sprite = graphic.sprite;
            render.color = Color.black;
            render.sortingLayerID = graphic.sortingLayerID;
            render.sortingLayerName = graphic.sortingLayerName;
            render.sortingOrder = graphic.sortingOrder - 5;
            render.transform.SetParent (transform);
            listSpriteRenders.Add (render);
        }
    }
}