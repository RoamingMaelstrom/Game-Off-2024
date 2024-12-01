using System.Collections;
using System.Collections.Generic;
using SOEvents;
using UnityEngine;
using UnityEngine.UI;

public class TechDependencyLines : MonoBehaviour
{
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] GameObject horizontalLine;
    [SerializeField] GameObject verticalLine;

    [SerializeField] TechTree techTree;

    [SerializeField] RectTransform[] bands;

    [SerializeField] List<LineTodGrouping> lines = new();

    private bool isSetup = false;

    private void Awake() {
        unlockTechEvent.AddListener(SetLineColouring);
    }

    private void SetLineColouring(TechObjectDisplay arg0) {
        SetLineColouring();
    }

    private void CreateLines() {
        isSetup = true;
        bands = new RectTransform[4];
        int b = 0;
        foreach (var band in techTree.bands)
        {
            if (b >= 4) break;
            bands[b] = band.gameObject.GetComponent<RectTransform>();
            BuildDependencyLines(bands[b], band.techDisplays);
            b++;
        }
    }

    private void Update() {
        if (!isSetup) {
            CreateLines();
            SetLineColouring();
        }
    }

    private IEnumerator SetLineColouringDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SetLineColouring();
    }

    private void OnEnable() {
        if (!isSetup) CreateLines();
        SetLineColouring();
        StartCoroutine(SetLineColouringDelayed(0.001f));
    }

    private void SetLineColouring() {
        foreach (var grouping in lines)
        {
            if (grouping.tOD.techUnlockStatusEncoded > 16 || grouping.dependent.techUnlockStatusEncoded > 16) grouping.lineImg.color = new Color(1, 1, 1, 0.8f);
            else grouping.lineImg.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    private void BuildDependencyLines(RectTransform band, TechObjectDisplay[] tODs) {
        for (int i = tODs.Length - 1; i >= 0; i--)
        {
            if (tODs[i] == null) continue;
            TechObjectDisplay tOD = tODs[i];
            if (tOD.dependentTechs.Length == 0) continue;
            if (tOD.dependentTechs.Length == 1) Build1Line(band, tOD, tOD.dependentTechs[0]);
            if (tOD.dependentTechs.Length == 2) BuildDoubleDependenceLine(band, tOD, tOD.dependentTechs[0], tOD.dependentTechs[1]);
        }
    }

    private void Build1Line(RectTransform band, TechObjectDisplay tOD, TechObjectDisplay dependent) {
        if (dependent.transform.parent != tOD.transform.parent) BuildSimpleCrossingLine(band, tOD, dependent);
        else if (Mathf.Abs(tOD.border.rectTransform.anchoredPosition.y - dependent.border.rectTransform.anchoredPosition.y) < 5) BuildSimpleLine(band, tOD, dependent);
        else BuildComplexLine(band, tOD, dependent);
    }

    private void BuildSimpleCrossingLine(RectTransform band, TechObjectDisplay tOD, TechObjectDisplay dependent) {
        Vector2 todLeft = tOD.border.rectTransform.rect.min + tOD.border.rectTransform.anchoredPosition;
        Vector2 dependentRight = dependent.border.rectTransform.rect.max + dependent.border.rectTransform.anchoredPosition - new Vector2(408, 0);
        lines.Add(new(CreateHorizontalLine(band, todLeft, dependentRight), tOD, dependent));
    }

    private void BuildSimpleLine(RectTransform band, TechObjectDisplay tOD, TechObjectDisplay dependent) {
        Vector2 todLeft = tOD.border.rectTransform.rect.min + tOD.border.rectTransform.anchoredPosition;
        Vector2 dependentRight = dependent.border.rectTransform.rect.max + dependent.border.rectTransform.anchoredPosition;
        lines.Add(new(CreateHorizontalLine(band, todLeft, dependentRight), tOD, dependent));
    }

    private void BuildComplexLine(RectTransform band, TechObjectDisplay tOD, TechObjectDisplay dependent) {
        Vector2 todLeft = tOD.border.rectTransform.anchoredPosition;
        todLeft.x += tOD.border.rectTransform.rect.min.x;
        Vector2 dependentRight = dependent.border.rectTransform.anchoredPosition;
        dependentRight.x += dependent.border.rectTransform.rect.max.x;

        lines.Add(new(CreateHorizontalLine(band, dependentRight, dependentRight + new Vector2(24, 0)), tOD, dependent));
        Vector2 start = new Vector2(dependentRight.x + 24, todLeft.y);
        lines.Add(new(CreateHorizontalLine(band, start, todLeft), tOD, dependent));
        lines.Add(new(CreateVerticalLine(band, start, dependentRight + new Vector2(24, 0)), tOD, dependent));
    }

    private void BuildDoubleDependenceLine(RectTransform band, TechObjectDisplay tOD, TechObjectDisplay dependent1, TechObjectDisplay dependent2) {
        Build1Line(band, tOD, dependent1);
        Build1Line(band, tOD, dependent2);
    }

    private Image CreateHorizontalLine(RectTransform parent, Vector2 start, Vector2 end, int minLength=24) {
        GameObject line = Instantiate(horizontalLine, Vector2.zero, Quaternion.identity, parent);
        Image lineImg = line.GetComponent<Image>();
        lineImg.rectTransform.sizeDelta = new Vector2(Mathf.Max(Mathf.Abs(end.x - start.x) - (Mathf.Abs(end.x - start.x) % minLength), minLength), lineImg.rectTransform.sizeDelta.y);
        lineImg.rectTransform.anchoredPosition = Vector2.Lerp(start, end, 0.5f) - new Vector2(lineImg.rectTransform.sizeDelta.x / 2f, minLength / 4f);
        return lineImg;
    }

    private Image CreateVerticalLine(RectTransform parent, Vector2 start, Vector2 end, int minHeight=24) {
        GameObject line = Instantiate(verticalLine, Vector2.zero, Quaternion.identity, parent);
        Image lineImg = line.GetComponent<Image>();
        lineImg.rectTransform.sizeDelta = new Vector2(lineImg.rectTransform.sizeDelta.x, Mathf.Max(Mathf.Abs(end.y - start.y) - (Mathf.Abs(end.y - start.y) % minHeight), minHeight));
        lineImg.rectTransform.anchoredPosition = Vector2.Lerp(start, end, 0.5f) - new Vector2(minHeight / 4f, lineImg.rectTransform.sizeDelta.y / 2f);
        return lineImg;
    }

}


[System.Serializable] 
public class LineTodGrouping
{
    public Image lineImg;
    public TechObjectDisplay tOD;
    public TechObjectDisplay dependent;

    public LineTodGrouping(Image lineImg, TechObjectDisplay tOD, TechObjectDisplay dependent) {
        this.lineImg = lineImg;
        this.tOD = tOD;
        this.dependent = dependent;
    }
}
