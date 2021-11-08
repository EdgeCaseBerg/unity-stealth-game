using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text numberOfCoinsText;
    public int coinsCollected;

    // Start is called before the first frame update
    void Start()
    {
        numberOfCoinsText.text = "0";
        GameObject.FindObjectOfType<Player>().OnCoinCollected += OnCoinCollected;
    }

    private void OnCoinCollected() {
        coinsCollected += 1;
        numberOfCoinsText.text = coinsCollected.ToString();
    }
}
