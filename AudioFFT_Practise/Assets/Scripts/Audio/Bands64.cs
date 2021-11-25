using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bands64 : MonoBehaviour
{
    [SerializeField] AudioBands _audioBands;
    public Image barPrb;
    public float h_space;

    RectTransform[] bars;

    [Header("")]
    [SerializeField] bool _uesBuffer;
    [SerializeField] float maxScale = 400;

    // Start is called before the first frame update
    void Start()
    {
        bars = new RectTransform[_audioBands.frequencyCount];

        for (int i = 0; i < _audioBands.frequencyCount; i++)
            bars[i] = transform.GetChild(i).GetComponent<RectTransform>();
    }

    void Update()
    {
        for (int i = 0; i < _audioBands.frequencyCount; i++)
        {
            var v2 = bars[i].sizeDelta;
            if (_uesBuffer)
                v2.y = _audioBands._bandBuffer[i] * maxScale;
            else
                v2.y = _audioBands._freoBand[i] * maxScale;

            bars[i].sizeDelta = v2;
        }
    }

    [ContextMenu("Creat Base Bar")]
    private void CreatBaseBar()
    {
        DeleteBaseBar();

        float w = barPrb.rectTransform.rect.width;
        for (int i = 0; i < _audioBands.frequencyCount; i++)
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
