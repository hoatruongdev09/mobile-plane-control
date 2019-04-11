using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ScrollSnaps;

public class ScrollViewSnap : MonoBehaviour
{

    private ScrollRect myScroll;
    private RectTransform viewPortPos;
    DirectionalScrollSnap scrollSnap;
    private RectTransform currentItem;
    private RectTransform lastItem;
    private Image lastImage;
    public GameObject pointToGroup;
    public Vector2 baseScale;
    private Vector2 startScale;
    public Vector2 upScale;
    private Color baseColor;
    private Color toColor;
    private List<Transform> layoutChild = new List<Transform>();
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        startScale = new Vector2(648f, 486f);
        foreach (Transform child in pointToGroup.transform)
        {
            EventTrigger trigger = child.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            trigger.triggers.Add(entry);

        }
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        // myScroll = GetComponent<ScrollRect>();
        // viewPortPos = myScroll.viewport;
        // myScroll.onValueChanged.AddListener(MyAction);
        Debug.Log("OnEnable");

        baseColor = new Color32(29, 29, 29, 255);
        toColor = new Color32(92, 189, 214, 255);
        scrollSnap = GetComponent<DirectionalScrollSnap>();
        scrollSnap.startItem = scrollSnap.content.GetChild(0).GetComponent<RectTransform>();
        lastItem = scrollSnap.startItem;
        currentItem = lastItem;
        // StartCoroutine(ScaleItemTo(scrollSnap.startItem, upScale, .5f));
        LeanTween.size(scrollSnap.startItem.GetComponent<RectTransform>(), upScale, .2f);
        lastImage = pointToGroup.transform.GetChild(0).GetComponent<Image>();
        StartCoroutine(ColorItemTo(lastImage, toColor, 0.5f));
        LeanTween.value(lastImage.gameObject, lastImage.color, toColor, 0.1f).setOnUpdate((Color val) =>
        {
            lastImage.color = val;
        });

        foreach (Transform child in scrollSnap.content)
        {
            layoutChild.Add(child);
        }

        scrollSnap.ScrollToSnapPosition(0, 0.1f, scrollSnap.GetInterpolator(scrollSnap.interpolator, 2f));
        ReOrderItem();
        LeanTween.value(lastImage.gameObject, lastImage.color, toColor, 1f).setOnComplete(onComplete =>
        {

            scrollSnap.UpdateLayout();
            scrollSnap.ScrollToSnapPosition(0, 0.1f, scrollSnap.GetInterpolator(scrollSnap.interpolator, 2f));
        });


    }
    public void SnapAnItemToCenter(int itemOrderIndex)
    {
        scrollSnap.ScrollToSnapPosition(itemOrderIndex, 0.1f, scrollSnap.GetInterpolator(scrollSnap.interpolator, 2f));
        ReOrderItem();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        Debug.Log("OnDisable");
        // scrollSnap.ScrollToSnapPosition(0, 0.1f, scrollSnap.GetInterpolator(scrollSnap.interpolator, 2f));
        for (int i = 0; i < layoutChild.Count; i++)
        {
            layoutChild[i].SetSiblingIndex(i);
            layoutChild[i].GetComponent<RectTransform>().sizeDelta = startScale;
        }
        currentItem = null;
        lastItem = null;
        layoutChild = null;
        layoutChild = new List<Transform>();
        foreach (Transform child in pointToGroup.transform)
        {
            child.GetComponent<Image>().color = baseColor;
        }
    }

    public void OnClosestSnap(int index)
    {
        scrollSnap.UpdateLayout();
        Debug.Log("ClosestSnap: " + index);
        scrollSnap.GetChildAtCalculateIndex(index, out currentItem);
        if (currentItem != lastItem)
        {
            // StartCoroutine(ScaleItemTo(lastItem, baseScale, .5f));
            LeanTween.size(lastItem.GetComponent<RectTransform>(), baseScale, .2f);
            lastItem = currentItem;
            // StartCoroutine(ScaleItemTo(currentItem, upScale, .5f));
            LeanTween.size(lastItem.GetComponent<RectTransform>(), upScale, .2f);
            MatchIndicator(index);
            Debug.Log(layoutChild.IndexOf(currentItem.transform) + " IndexOf");
            ReOrderItem();

        }

        Debug.Log(currentItem.gameObject.name + "on selected event");
    }

    IEnumerator ScaleItemTo(RectTransform rect, Vector2 to, float time)
    {

        float startTime = Time.time;
        while (Time.time < startTime + time)
        {

            rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, to, (Time.time - startTime) / time);
            yield return null;
        }
        rect.sizeDelta = to;
    }
    IEnumerator ColorItemTo(Image img, Color to, float time)
    {

        float startTime = Time.time;
        while (Time.time < startTime + time)
        {
            img.color = Color.Lerp(img.color, to, time);
            yield return null;
        }
        img.color = to;
    }

    public void OnPointerEnterDelegate(PointerEventData eventData)
    {
        scrollSnap.ScrollToSnapPosition(eventData.pointerEnter.transform.GetSiblingIndex(), 0.1f, scrollSnap.GetInterpolator(scrollSnap.interpolator, 2f));
        StartCoroutine(ColorItemTo(lastImage, baseColor, 0.1f));
        lastImage = eventData.pointerEnter.GetComponent<Image>();
        StartCoroutine(ColorItemTo(lastImage, toColor, 0.1f));

        // pointToGroup.transform.chi
    }
    void MatchIndicator(int index)
    {
        StartCoroutine(ColorItemTo(lastImage, baseColor, 0.1f));
        lastImage = pointToGroup.transform.GetChild(index).GetComponent<Image>();
        StartCoroutine(ColorItemTo(lastImage, toColor, 0.1f));
    }
    void ReOrderItem()
    {
        float siblingIndex = 0;
        int countTemp = 0;

        for (int i = 0; i < layoutChild.IndexOf(currentItem.transform); i++)
        {
            countTemp++;
            siblingIndex = layoutChild.IndexOf(currentItem.transform) - i;
            layoutChild[i].SetSiblingIndex(i);
            LeanTween.alpha(layoutChild[i].Find("Black").GetComponent<RectTransform>(), 184f / 255f, 0.1f);
            // Debug.Log(baseScale / siblingIndex + " baseScale");
            LeanTween.size(layoutChild[i].GetComponent<RectTransform>(), baseScale / siblingIndex, .2f);
        }
        currentItem.transform.SetAsLastSibling();
        LeanTween.alpha(currentItem.Find("Black").GetComponent<RectTransform>(), 0, 0.1f);

        for (int i = layoutChild.IndexOf(currentItem.transform) + 1; i < layoutChild.Count; i++)
        {
            countTemp++;
            siblingIndex = countTemp - layoutChild.IndexOf(currentItem.transform);
            layoutChild[i].SetSiblingIndex(currentItem.GetSiblingIndex() - countTemp);
            LeanTween.size(layoutChild[i].GetComponent<RectTransform>(), baseScale / siblingIndex, .2f);
            LeanTween.alpha(layoutChild[i].Find("Black").GetComponent<RectTransform>(), 184f / 255f, 0.1f);
        }
    }
    void ReColorItem()
    {
        float siblingIndex = 0;
        int countTemp = 0;
        for (int i = 0; i < layoutChild.IndexOf(currentItem.transform); i++)
        {
            countTemp++;
            siblingIndex = layoutChild.IndexOf(currentItem.transform) - i;
            // layoutChild[i].SetSiblingIndex(i);
            Debug.Log(Vector2.Distance(baseScale / siblingIndex, baseScale / 2) + "vector2 distance");
            StartCoroutine(ColorItemTo(layoutChild[i].Find("Black").GetComponentInChildren<Image>(), new Color32(0, 0, 0, 184), 0.5f));

            // StartCoroutine(ScaleItemTo(layoutChild[i].GetComponent<RectTransform>(), baseScale / siblingIndex, .5f));
        }
        // currentItem.transform.SetAsLastSibling();
        StartCoroutine(ColorItemTo(currentItem.Find("Black").GetComponentInChildren<Image>(), new Color32(0, 0, 0, 0), 0.1f));

        for (int i = layoutChild.IndexOf(currentItem.transform) + 1; i < layoutChild.Count; i++)
        {
            countTemp++;
            siblingIndex = countTemp - layoutChild.IndexOf(currentItem.transform);
            // layoutChild[i].SetSiblingIndex(currentItem.GetSiblingIndex() - countTemp);
            // StartCoroutine(ScaleItemTo(layoutChild[i].GetComponent<RectTransform>(), baseScale / siblingIndex, .5f));
            StartCoroutine(ColorItemTo(layoutChild[i].Find("Black").GetComponentInChildren<Image>(), new Color32(0, 0, 0, 184), 0.5f));
        }
    }
    public void ScrollToFirstPos()
    {
        scrollSnap.ScrollToSnapPosition(0, 0.1f, scrollSnap.GetInterpolator(scrollSnap.interpolator, 2f));
    }
}