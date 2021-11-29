using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioBars : MonoBehaviour
{
    [SerializeField] AudioRawDataBar audioRawDataBar;
    [SerializeField] Gradient _gradient;
    [SerializeField] Transform tsf_bar;

    public Image barPrb;
    [Range(0, 1)]
    public float h_space;
    public int dataSize;
    [Range(1, 200)]
    public float maxScale;
    RectTransform[] bars;

    [SerializeField] EChannel eChannel;
    enum EChannel
    {
        Stereo, Left, Right,
    }

    // Start is called before the first frame update
    void Start()
    {
        bars = new RectTransform[dataSize];

        for (int i = 0; i < tsf_bar.childCount; i++)
            bars[i] = tsf_bar.GetChild(i).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            var v2 = bars[i].sizeDelta;
            v2.y = 1 + audioRawDataBar._samples[i] * 20 * maxScale;
            bars[i].sizeDelta = v2;
        }
    }

    [ContextMenu("Creat Base Bar")]
    private void CreatBaseBar()
    {
        DeleteBaseBar();

        float w = barPrb.rectTransform.rect.width;
        float count = (int)dataSize;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(barPrb, tsf_bar);
            go.name = "Hz_" + i.ToString("000");
            go.transform.position = tsf_bar.position + (Vector3.right * (w + h_space) * (i - count / 2f));
            float time = 1f / count;
            go.GetComponent<Image>().color = _gradient.Evaluate(time * i);
        }
    }

    [ContextMenu("Delte Base Bar")]
    private void DeleteBaseBar()
    {
        int count = tsf_bar.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(tsf_bar.GetChild(i).gameObject);
        }
    }
}
