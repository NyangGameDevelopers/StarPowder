using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SkillCore : MonoBehaviour
{

    #region FIELD-OPTIONS
    [SerializeField]
    [Range(0f, 0.300f)]
    private float UpdateFrequency = 0.02f;
    #endregion

    #region PRIVATE-RUNTIME
    private float _timeChecker;

    private Rigidbody rb;
    private Collider co;

    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        co = GetComponent<Collider>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Time.deltaTime
    }
}


namespace skill
{
    public class SkillState
    {
        public CasterCore caster { get; private set; }
        public SkillCore skill { get; private set; }
    }
    public class StepResult
    {
        // 
        public enum ResultCode
        {
            Ok,
            Warning,
            Error,
            Fetal,
        }
        public ResultCode code { get; private set; }
        public string message { get; private set; }
        public Step step { get; private set; }
        private StepResult()
        {
            code = ResultCode.Ok;
            message = "";
            step = null;
        }
        public static StepResult Ok(Step step)
        {
            var res = new StepResult();
            res.code = ResultCode.Ok;
            return res;
        }
        public static StepResult Warning(Step step, string msg)
        {

            var res = new StepResult();
            res.code = ResultCode.Warning;
            res.message = msg;
            return res;
        }
        public static StepResult Error(string msg)
        {

            var res = new StepResult();
            res.code = ResultCode.Error;
            res.message = msg;
            return res;
        }
    }
    public class Skill
    {
        Step entry;
        Step current;
    }
    public abstract class Step
    {
        public Step parent;
        public abstract StepResult Do(SkillState state);
        public abstract Step[] PossibleSteps();
        public int children() { return 1; }

    }
    
}