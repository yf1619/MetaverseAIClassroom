using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextReplacer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TMP_Text[] texts = this.GetComponentsInChildren<TMP_Text>(true);
        foreach (var item in texts)
        {
            GameObject go = item.gameObject;
            go.AddComponent<Text>().text = item.text;
            DestroyImmediate(item);
            
        }
        // foreach (var item in this.GetComponentsInChildren<TMP_InputField>())
        // {
        //     item.gameObject.AddComponent<InputField>().text = item.text;
        //     //Destroy(item);
        // }
    }
    // void Update()
    // {
    //     time += Time.deltaTime;
    //     if (time < 3f)
    //     {
    //         foreach (var item in this.GetComponentsInChildren<TMP_Text>())
    //         {
    //             item.font = newAsset;
    //         }
    //         foreach (var item in this.GetComponentsInChildren<TMP_InputField>())
    //         {
    //             item.fontAsset = newAsset;
    //         }
    //     }
    // }
}
