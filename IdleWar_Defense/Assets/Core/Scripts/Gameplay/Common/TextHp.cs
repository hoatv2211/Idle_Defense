using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHp : MonoBehaviour
{
    public const int TEXT_REGEN_HP = 1;
    public const int TEXT_DAMAGE_HP = 2;

    public AnimationCurve animCurve;
    public AnimationCurve animCurveMoveX;
    public AnimationCurve animCurveYHeal;
    public TextMeshPro txtHp;
    float defauntScale = 0.4f;
    float scaleOutAdd = 0.1f;
    float scaleOutAddNonCrit = 0.1f;
    float scaleOutAddCrit = 0.45f;
    float timeScaleOut = 0.2f;
    float currentTimeScaleOut = 0.2f;

    float scaleIn = 0.3f;
    float timeScaleIn = 0.5f;
    float currentTimeScaleIn = 0.5f;

    Vector3 posStart;

    public Color colorTL = new Color(0.902f, 0, 0, 1);
    public Color colorTR = new Color(0.9019608f, 0, 0, 1);
    public Color colorBL = new Color(1f, 0.3686278f, 0, 1);
    public Color colorBR = new Color(1f, 0.1490196f, 0, 1);

    VertexGradient gradientNonCrit;

    public Color colorTLCrit = new Color(0.902f, 0, 0, 1);
    public Color colorTRCrit = new Color(0.9019608f, 0, 0, 1);
    public Color colorBLCrit = new Color(1f, 0.3686278f, 0, 1);
    public Color colorBRCrit = new Color(1f, 0.1490196f, 0, 1);

    VertexGradient gradientCrit;

    public Color colorTLHeal = new Color(0.1081827f, 1, 0, 1);
    public Color colorTRHeal = new Color(0.1067045f, 1, 0, 1);
    public Color colorBLHeal = new Color(0, 0.9999995f, 1, 1);
    public Color colorBRHeal = new Color(0, 1f, 0.9728527f, 1f);

    VertexGradient gradientHeal;

    bool isRight;
    int typeAttack;
    private void Awake()
    {
        gradientNonCrit = new VertexGradient(colorTL, colorTR, colorBL, colorBR);
        gradientCrit = new VertexGradient(colorTLCrit, colorTRCrit, colorBLCrit, colorBRCrit);
        gradientHeal = new VertexGradient(colorTLHeal, colorTRHeal, colorBLHeal, colorBRHeal);
    }

    static int countLoop = 0;
    public void SetInfo(float hp, bool crit = false, int typeAttack = TEXT_DAMAGE_HP)
    {
        timeScaleOut = 0.2f;
        currentTimeScaleOut = timeScaleOut;

        timeScaleIn = 0.5f;
        currentTimeScaleIn = timeScaleIn;

        float yAdd = 0;
        if (countLoop == 0)
        {
            countLoop++;
        }
        else if (countLoop == 1)
        {
            countLoop++;
            yAdd = 0.2f;
        }
        else if (countLoop == 2)
        {
            countLoop = 0;
            yAdd = 0.4f;
        }
        /*if(isRight) */transform.position = new Vector3(transform.position.x/* + Random.Range(-0.3f, 0.1f)*/, transform.position.y + yAdd, transform.position.z);
        //else transform.position = new Vector3(transform.position.x/* + Random.Range(-0.1f, 0.3f)*/, transform.position.y + yAdd, transform.position.z);

        if (hp <= 0f) txtHp.text = "miss";
        else txtHp.text = hp.ToString("0") + "";
        posStart = transform.position;
        transform.localScale = new Vector3(defauntScale, defauntScale, 1f);

        Color c = txtHp.color;
        c.a = 1f;
        txtHp.color = c;

        isRight = Random.Range(0, 2) == 0;
        this.typeAttack = typeAttack;

        if (typeAttack == TEXT_REGEN_HP)
        {
            txtHp.colorGradient = gradientHeal;
            scaleOutAdd = scaleOutAddNonCrit;
        }
        else
        {
            if (crit)
            {
                txtHp.colorGradient = gradientCrit;

                scaleOutAdd = scaleOutAddCrit;
            }
            else
            {
                txtHp.colorGradient = gradientNonCrit;

                scaleOutAdd = scaleOutAddNonCrit;
            }
        }
    }

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;
        currentTimeScaleOut -= delta;
        if (currentTimeScaleOut <= 0f)
        {
            currentTimeScaleOut = 0f;

            currentTimeScaleIn -= delta;
            if (currentTimeScaleIn <= 0f)
            {
                currentTimeScaleIn = 0f;
                CompleteMove();
            }
            else
            {
                ScaleInUpdate(currentTimeScaleIn / timeScaleIn);
            }
        }
        else
        {
            ScaleOutUpdate(currentTimeScaleOut / timeScaleOut);
        }

        UpdateMove((currentTimeScaleIn + currentTimeScaleOut) / (timeScaleIn + timeScaleOut));
    }

    void CompleteMove()
    {
        gameObject.SetActive(false);
    }

    void UpdateMove(float rate)
    {
        float y = 0;
        float x = 0;
        if (typeAttack == TEXT_REGEN_HP)
        {
            y = posStart.y + 0.4f * animCurveYHeal.Evaluate((1f - rate));
            x = posStart.x;
        }
        else
        {
            y = posStart.y - 0.7f * animCurve.Evaluate(1f - rate);
            //float x= posStart.x + 1f * animCurve.Evaluate((1f - rate));
            x = 0;
            if (isRight)
            {
                x = posStart.x + 0.7f * animCurveMoveX.Evaluate(1f - rate);
            }
            else
            {
                x = posStart.x - 0.7f * animCurveMoveX.Evaluate(1f - rate);
            }
        }

        transform.position = new Vector3(x, y, posStart.z);
    }
    
    void ScaleOutUpdate(float rate)
    {
        transform.localScale = new Vector3(defauntScale + scaleOutAdd * (1f - rate), defauntScale + scaleOutAdd * (1f - rate), 1f);
    }
    
    void ScaleInUpdate(float rate)
    {
        float scaleN = (defauntScale + scaleOutAdd) - scaleIn * (1f - rate);
        transform.localScale = new Vector3(scaleN, scaleN, 1f);

        Color c = txtHp.color;
        c.a = rate;
        txtHp.color = c;
    }
}
