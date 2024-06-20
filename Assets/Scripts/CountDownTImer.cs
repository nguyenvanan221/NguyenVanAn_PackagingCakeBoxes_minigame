using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownTImer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private UIController uIController;
    private GameManager gameManager;

    private const int TIMEREMAINING = 45;

    private void Awake()
    {
        uIController = GetComponent<UIController>();
        gameManager= GetComponent<GameManager>();
    }

    public void StartCountdownTimer()
    {
        StopAllCoroutines();
        StartCoroutine(Countdown(TIMEREMAINING));
    }

    IEnumerator Countdown(int timeRemain)
    {
        while (timeRemain > 0)
        {
            timerText.text = timeRemain.ToString();
            yield return new WaitForSecondsRealtime(1);
            timeRemain--;
        }

        uIController.ShowUI(uIController.levelFailedUI);
        gameManager.RemoveLevel();
    }
}
