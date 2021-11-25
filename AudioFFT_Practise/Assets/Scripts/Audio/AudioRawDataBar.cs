using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioRawDataBar : MonoBehaviour
{
    [SerializeField] AudioBands _audioBands;
    public Image barPrb;
    public float h_space;
    public float maxScale = 400;

    RectTransform[] bars;

    // Start is called before the first frame update
    void Start()
    {
        bars = new RectTransform[_audioBands.spectrumDtatSize];

        for (int i = 0; i < _audioBands.spectrumDtatSize; i++)
            bars[i] = transform.GetChild(i).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _audioBands.spectrumDtatSize; i++)
        {
            var v2 = bars[i].sizeDelta;
            v2.y = _audioBands._samplesLeft[i] * maxScale;
            bars[i].sizeDelta = v2;
        }
    }

    [ContextMenu("Creat Base Bar")]
    private void CreatBaseBar()
    {
        DeleteBaseBar();

        float w = barPrb.rectTransform.rect.width;
        for (int i = 0; i < _audioBands.spectrumDtatSize; i++)
        {
            var go = Instantiate(barPrb, this.transform);
            go.name = "Hz_" + i.ToString("000");
            go.transform.position = transform.position + (Vector3.right * i * (w + h_space));
        }
    }
    [ContextMenu("Delte Base Bar")]
    private void DeleteBaseBar()
    {
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
