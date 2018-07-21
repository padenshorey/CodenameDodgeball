using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickThrowIcon : MonoBehaviour
{
    public enum QuickThrowIconState
    {
        Empty,
        Charging,
        Ready
    }


    public QuickThrowIconState state = QuickThrowIconState.Empty;
    public Image fill;
    public Image ball;
    public float charge = 0f;
    public float chargeStartTime;

    public void Update()
    {
        if(state == QuickThrowIconState.Charging)
        {
            charge = fill.fillAmount = (Time.time - chargeStartTime) / GameManager.instance.gamePreferences.timeForQuickThrowRecharge;
            if(charge >= 1f)
            {
                FinishCharge();
            }
        }
    }

    public void StartCharge()
    {
        state = QuickThrowIconState.Charging;
        chargeStartTime = Time.time;
    }

    public void FinishCharge()
    {
        state = QuickThrowIconState.Ready;
        AudioManager.instance.PlaySFX(AudioManager.AudioSFX.Pop);
        GetComponent<Animator>().SetTrigger("Ready");
        ball.enabled = true;
        fill.fillAmount = 0f;
        charge = 0;
    }

    public void Reset()
    {
        fill.fillAmount = 0f;
        ball.enabled = false;
        state = QuickThrowIconState.Empty;
    }

    public void SetIconState(QuickThrowIcon q)
    {
        state = q.state;
        ball.enabled = q.ball.enabled;
        fill.fillAmount = q.fill.fillAmount;
        chargeStartTime = q.chargeStartTime;
        charge = q.charge;
    }
}
