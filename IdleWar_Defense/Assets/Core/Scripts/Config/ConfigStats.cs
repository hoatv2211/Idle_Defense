using UnityEngine;
using Utilities.Service.RFirebase;
using Utilities.Services;
using System;
using System.Linq;
using FoodZombie;
using Zirpl.CalcEngine;
using Random = System.Random;

public class ConfigStats
{
    static string GetFormulaValue(string formulaName)
    {
        return GameData.Instance.formulaDefinitions.Where(
                e => e.stat == formulaName
                ).FirstOrDefault().formula;
    }

    private static float GetElementRate(int elementAttacker, int elementTarget)
    {
        var formulaElementDamage = GameData.Instance.formulaElementDamageDefinitions.Where(
            e => e.element == elementAttacker
        ).FirstOrDefault();
        if (elementTarget == IDs.ELEMENT_ELECTRIC) return formulaElementDamage.electric;
        else if (elementTarget == IDs.ELEMENT_METALIC) return formulaElementDamage.metalic;
        else if (elementTarget == IDs.ELEMENT_NITROGEN) return formulaElementDamage.nitrogen;
        else if (elementTarget == IDs.ELEMENT_LAVA) return formulaElementDamage.lava;
        else return 100f;
    }

    public static float GetStat(float statBase, float statAdd, int level)
    {
        var calculator = new CalculationEngine();
        calculator.Variables["statBase"] = statBase;
        calculator.Variables["statAdd"] = statAdd;
        calculator.Variables["level"] = level;

        return Convert.ToSingle(calculator.Evaluate(GetFormulaValue("stat")));
    }

    public static int GetStat(int statBase, int statAdd, int level)
    {
        var calculator = new CalculationEngine();
        calculator.Variables["statBase"] = statBase;
        calculator.Variables["statAdd"] = statAdd;
        calculator.Variables["level"] = level;

        return Convert.ToInt32(calculator.Evaluate(GetFormulaValue("stat")));
    }

    public static float[] GetStat(float[] statBases, float[] statAdds, int level)
    {
        var calculator = new CalculationEngine();
        float[] output = new float[statBases.Length];
        for (int i = 0; i < statBases.Length; i++)
        {
            float statBase = statBases[i];
            float statAdd = statAdds[i];
            calculator.Variables["statBase"] = statBase;
            calculator.Variables["statAdd"] = statAdd;
            calculator.Variables["level"] = level;
            output[i] = Convert.ToSingle(calculator.Evaluate(GetFormulaValue("stat")));
        }
        return output;

        //return Convert.ToInt32(calculator.Evaluate(GetFormulaValue("stat")));
    }

    public static int GetXSpeed()
    {
        return int.Parse(GetFormulaValue("xSpeed"));
    }

    public static float GetPercentOutRangeDamage()
    {
        return float.Parse(GetFormulaValue("percentOutRangeDamage")) / 100f;
    }

    public static float GetBonusDamageFormationByElement()
    {
        return float.Parse(GetFormulaValue("bonusDamageFormationByElement")) / 100f;
    }

    public static int GetPower(float HP, float damage, float armor, float attackSpeed, float critRate, float accuracy, float dodge, float critDamage)
    {
        var calculator = new CalculationEngine();
        calculator.Variables["HP"] = HP;
        calculator.Variables["damage"] = damage;
        calculator.Variables["armor"] = armor;
        calculator.Variables["A"] = GetFormulaValue("a");
        calculator.Variables["attackSpeed"] = attackSpeed;
        calculator.Variables["critRate"] = critRate;
        calculator.Variables["accuracy"] = accuracy;
        calculator.Variables["dodge"] = dodge;
        calculator.Variables["critDamage"] = critDamage;

        int result = Convert.ToInt32(calculator.Evaluate(GetFormulaValue("power")));
        return result;
    }

    public static int GetPowerGear(float HP, float damage, float armor, float attackSpeed, float critRate, float accuracy, float dodge, float critDamage)
    {
        var calculator = new CalculationEngine();
        calculator.Variables["HP"] = HP;
        calculator.Variables["damage"] = damage;
        calculator.Variables["armor"] = armor;
        calculator.Variables["A"] = GetFormulaValue("a");
        calculator.Variables["attackSpeed"] = attackSpeed;
        calculator.Variables["critRate"] = critRate;
        calculator.Variables["accuracy"] = accuracy;
        calculator.Variables["dodge"] = dodge;
        calculator.Variables["critDamage"] = critDamage;

        int result = Convert.ToInt32(calculator.Evaluate(GetFormulaValue("powerGear")));
        return result;
    }

    public static float GetDamInBattle(ref bool isCrit, float damageAttacker, float armorTarget, float critRate, float critDamage,
        int elementAttacker, int elementTarget, float accuracyAttacker, float dodgeTarget)
    {
        var realDamageCalculator = new CalculationEngine();
        realDamageCalculator.Variables["damageAttacker"] = damageAttacker;
        realDamageCalculator.Variables["armorTarget"] = armorTarget;
        realDamageCalculator.Variables["a"] = GetFormulaValue("a");
        var realDamage = Convert.ToSingle(realDamageCalculator.Evaluate(GetFormulaValue("realDamage")));
        var minDamage = damageAttacker * 0.2f;
        if (realDamage <= minDamage) realDamage = minDamage;

        var criticalDamageCalculator = new CalculationEngine();
        criticalDamageCalculator.Variables["realDamage"] = realDamage;
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r <= (critRate / 100f))
        {
            criticalDamageCalculator.Variables["critDamage"] = critDamage;
            isCrit = true;
        }
        else
        {
            criticalDamageCalculator.Variables["critDamage"] = 100f;
            isCrit = false;
        }
        var criticalDamage = Convert.ToSingle(criticalDamageCalculator.Evaluate(GetFormulaValue("criticalDamage")));

        var strongerElementDamageCalculator = new CalculationEngine();
        strongerElementDamageCalculator.Variables["criticalDamage"] = criticalDamage;
        var elementRate = GetElementRate(elementAttacker, elementTarget);
        strongerElementDamageCalculator.Variables["elementRate"] = elementRate;
        var strongerElementDamage = Convert.ToSingle(strongerElementDamageCalculator.Evaluate(GetFormulaValue("elementDamage")));

        var dodgeTargetRate = (dodgeTarget - accuracyAttacker) / 100f;
        if (dodgeTargetRate > 0f)
        {
            r = UnityEngine.Random.Range(0f, 1f);
            if (r <= dodgeTargetRate)
            {
                return 0f;
            }
        }

        return strongerElementDamage;
    }
}

[System.Serializable]
public class FormulaDefinition
{
    public string stat;
    public string formula;
}

[System.Serializable]
public class FormulaElementDamageDefinition : IComparable<FormulaElementDamageDefinition>
{
    public int element;
    public float electric;
    public float metalic;
    public float nitrogen;
    public float lava;

    public int CompareTo(FormulaElementDamageDefinition other)
    {
        return element.CompareTo(other.element);
    }
}