using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Bands64 : MonoBehaviour
{
    [SerializeField] AudioFFT64 _audioBands;
    public GameObject barPrb;
    public Transform tsf_bar;

    public float h_space;

    Transform[] bars;

    [SerializeField] Gradient _gradient;
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
        //for (int i = 0; i < _audioBands.frequencyCount; i++)
        //{
        //    var v2 = bars[i].sizeDelta;
        //    if (_uesBuffer)
        //        v2.y = 1 + _audioBands._bandBuffer[i] * maxScale;
        //    else
        //        v2.y = 1 + _audioBands._freoBand[i] * maxScale;

        //    bars[i].sizeDelta = v2;
        //}
    }


    [ContextMenu("Creat Base Bar")]
    private void CreatBaseBar()
    {
        DeleteBaseBar();

        float w = barPrb.GetComponent<MeshRenderer>().bounds.size.x;
        float count = (int)_audioBands.frequencyCount;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(barPrb, tsf_bar);
            go.name = "Band_" + i.ToString("00");
            go.transform.position = tsf_bar.position + (Vector3.right * (w + h_space) * (i - count / 2f));
            float time = 1f / count;

            var r = go.GetComponent<MeshRenderer>();
            var m = new Material(r.sharedMaterial);
            m.SetColor("_AlbdeoColor", _gradient.Evaluate(time * i));
            r.sharedMaterial = m;
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
