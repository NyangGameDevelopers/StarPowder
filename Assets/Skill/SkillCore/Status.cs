using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Status : MonoBehaviour
{
    public enum LiteralKey
    {
        ID,
        Name,
    }

    public enum NumericKey
    {
        Health,
        Stamina,
        Mana,
    }


    public Container<LiteralKey, string> Literals { get; private set; }
    public Container<NumericKey, float> Numerics { get; private set; }
    void Awake()
    {
        Literals = new Container<LiteralKey, string>();
        Numerics = new Container<NumericKey, float>();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool PayMana(float mana)
    {
        if (!Numerics.Exist(NumericKey.Mana))
        {
            return false;
        }
        var expectedMana = Numerics[NumericKey.Mana] - mana;
        var result = expectedMana >= 0;
        Numerics[NumericKey.Mana] = expectedMana;
        return result;
    }
    public DamageResult Damage(Status dealer, float coefficient)
    {
        if (!Numerics.Exist(NumericKey.Health))
        {
            return DamageResult.Impossible;
        }
        // TODO : 공격 계산식
        var currentHealth = Numerics[NumericKey.Health];
        if (currentHealth <= 0)
        {
            return DamageResult.Dead;
        }
        var expectedHealth = currentHealth - coefficient;
        if (expectedHealth <= 0)
        {
            Numerics[NumericKey.Health] = 0;
            return DamageResult.Die;
        }
        Numerics[NumericKey.Health] = expectedHealth;
        return DamageResult.Alive;
    }

}

public enum DamageResult
{
    Impossible,
    Alive,
    Die,
    Dead,
}
public class Container<TK, TV>
where TK : System.Enum
{
    private Dictionary<TK, TV> datas;
    public Container()
    {
        datas = new Dictionary<TK, TV>();
    }
    public TV this[TK index]
    {
        get
        {
            return datas[index];
        }
        set
        {
            datas[index] = value;
        }
    }
    public bool Exist(TK key)
    {
        return datas.ContainsKey(key);
    }
    public TV GetOr(TK key, Func<TV> handler)
    {
        TV result;
        if (datas.TryGetValue(key, out result))
        {
            return result;
        }
        else
        {
            return handler();
        }
    }
    public void SetOr(TK key, TV value, Func<TV, bool> handler)
    {
        TV result;
        if (datas.TryGetValue(key, out result))
        {
            if (handler(result))
            {
                datas[key] = value;
            }
        }
        else
        {
            datas.Add(key, value);
        }
    }
}