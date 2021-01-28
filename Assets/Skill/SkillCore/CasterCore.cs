using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Status))]
public class CasterCore : MonoBehaviour
{
    public Status status { get; private set; }
    private Dictionary<SkillIdentifier, float> skillCalledAt;
    void Awake()
    {
        status = GetComponent<Status>();
        skillCalledAt = new Dictionary<SkillIdentifier, float>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    CastingResult Cast(SkillData data)
    {
        float prev;
        if (skillCalledAt.TryGetValue(data.identifier, out prev))
        {
            if (Time.time - prev > data.cooltime)
            {
                return new CastingResult
                {
                    result = CastingResultCode.Cooltime,
                    skill = null,
                };
            }
        }
        else
        {
            skillCalledAt.Add(data.identifier, Time.time);
        }
        var child = new GameObject(name + "/" + data.identifier.ToString());
        var skcore = child.AddComponent<SkillCore>();
        skcore.skillData = data;
        return new CastingResult{
            result = CastingResultCode.Success,
            skill = skcore,
        };
    }
}

public struct CastingResult
{
    public CastingResultCode result;
    public SkillCore skill;

    // 
    public bool isSuccess { get { return result is CastingResultCode.Success; } }
}
public enum CastingResultCode
{
    Success,
    Cooltime,
}