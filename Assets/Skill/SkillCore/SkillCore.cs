using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(Status))]
public class SkillCore : MonoBehaviour
{

    #region 설정값
    [SerializeField]
    [Range(0f, 0.300f)]
    private float UpdateFrequency = 0.02f;
    #endregion


    #region 로컬 정보값
    private skill.State state;
    private SkillData skillData;
    #endregion

    #region 다른 스크립트
    public Rigidbody ownRigid { get; private set; }
    public Collider ownColli { get; private set; }
    public Status status { get; private set; }
    #endregion

    void Awake()
    {
        ownRigid = GetComponent<Rigidbody>();
        ownColli = GetComponent<Collider>();
        status = GetComponent<Status>();

        // TODO : 잘못된 CasterCore에서 생성 요청시에 오류 발생
        _state = new skill.State(
            this.transform.parent.gameObject.GetComponent<CasterCore>(),
            this
        );
    }
    // Start is called before the first frame update
    void Start()
    {
        ownRigid.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        // 상태 업데이트
        state.deltatime = Time.deltaTime;
        state.runtime += Time.deltaTime;
        // 상태에 따른 호출 제어

    }
}


namespace skill
{




    public class State
    {

        public CasterCore caster { get; private set; }
        public SkillCore skill { get; private set; }
        public List<GameObject> targets;
        public float runtime;
        public float deltatime;
        public RunState runState { get; private set; }
        public event Action<State, GameObject> OnContact;

        public State(CasterCore caster, SkillCore skill)
        {
            this.caster = caster;
            this.skill = skill;
            this.runState = RunState.Run;
        }

        public void ChangeState(RunState runState)
        {
            this.runState = runState;
        }
    }

    public enum RunState
    {
        Run, Yeild, Break, Continue, End, Exit, Wait, Fail
    }

    #region 인터페이스
    public interface IStateless : IBase, INode
    {
    }
    public interface IBase
    {
        INode Build(State state);
    }
    public interface INode
    {
        void Reset(State state);
        void Calling(State state);
    }
    public interface IBaseLocation
    {
        INodeLocation Build(State state);
    }
    public interface INodeLocation
    {
        void Reset(State state);
        Nullable<Vector3> GetLocation(State state);
    }
    public interface IBNLocation : IBaseLocation, INodeLocation { }
    public interface IBaseDirection
    {
        INodeDirection Build(State state);
    }
    public interface INodeDirection
    {
        void Reset(State state);
        Nullable<Quaternion> GetDirection(State state);
    }
    public interface IBNDirection : IBaseDirection, INodeDirection { }
    public interface IBaseCrosshair
    {
        INodeCrosshair Build(State state);
    }
    public interface INodeCrosshair
    {
        void Reset(State state);
        Nullable<Vector3> GetCameraLocation(State state);
        Nullable<Quaternion> GetCameraDirection(State state);
    }
    public interface IBNCrosshair : IBaseCrosshair, INodeCrosshair { }

    public interface IBaseTarget
    {
        INodeTarget Build(State state);
        IBaseLocation TargetPos();
        IBaseDirection TargetRot();
    }
    public interface INodeTarget
    {
        void Reset(State state);
        GameObject GetTarget(State state);
    }
    public interface IBNTarget : IBaseTarget, INodeTarget { }
    public interface IBaseTargets
    {
        INodeTargets Build(State state);
    }
    public interface INodeTargets
    {
        void Reset(State state);
        IEnumerable<GameObject> GetTargets(State state);
    }
    public interface IBNTargets : IBaseTargets, INodeTargets { }

    public interface IFilter
    {
        IEnumerable<GameObject> Filtering(State state, IEnumerable<GameObject> from);
    }

    public interface IBaseInterrupt
    {
        INodeInterrupt Build(State state, RunState targetState);
    }
    public interface INodeInterrupt
    {
        void Install(State state);
        void Uninstall(State state);
    }
    #endregion

    // 쿨타임
    // 플레이어 밀어내기
    // 스테이터스 버프/디버프,
    // 스테이터스 계수,
    // 스킬 호출 체인,
    // 스킬 호출 체인,
    // Timeout, Interval
    // 소환수 관리 : 캐스터 자식으로 스폰
    // 필터링



    public static class Node
    {
        public static IBase Spell(params IBase[] steps) { return Block(steps); }
        public static BBlock Block(params IBase[] steps)
        {
            return new BBlock(steps.ToList());
        }
        public static BLoop Loop(int count, params IBase[] steps)
        {
            return new BLoop(Block(steps), count);
        }
        public static BUntil Until(IBaseInterrupt when, params IBase[] steps)
        {
            return new BUntil(when, Block(steps));
        }
        public static IBase Require()
        {
            return new BNWork((state) =>
            {
                if (state.targets.Count() < 1)
                {
                    state.ChangeState(RunState.Exit);
                }
            });
        }
        public static IBase Require(Predicate<State> predicate)
        {
            return new BNWork((state) =>
            {
                if (!predicate(state))
                {
                    state.ChangeState(RunState.Exit);
                }
            });
        }
        #region 검색식
        public static BNCaster Caster()
        {
            return new BNCaster();
        }
        public static BNSkill Skill()
        {
            return new BNSkill();
        }
        public static BCrosshair Crosshair()
        {
            return new BCrosshair(Caster(), float.PositiveInfinity, Physics.DefaultRaycastLayers);
        }
        public static BCrosshair Crosshair(float limit)
        {
            return new BCrosshair(Caster(), limit, Physics.DefaultRaycastLayers);

        }
        public static BCrosshair Crosshair(IBaseCrosshair who)
        {
            return new BCrosshair(
                who,
                float.PositiveInfinity,
                Physics.DefaultRaycastLayers
            );
        }
        public static BCrosshair Crosshair(IBaseCrosshair who, float limit)
        {
            return new BCrosshair(
                who,
                limit,
                Physics.DefaultRaycastLayers
            );
        }
        public static BCrosshair Crosshair(IBaseLocation origin, IBaseDirection direction)
        {
            return new BCrosshair(new BVirtualCrosshair(origin, direction), float.PositiveInfinity, Physics.DefaultRaycastLayers);
        }
        public static BCrosshair Crosshair(IBaseLocation origin, IBaseDirection direction, float limit)
        {
            return new BCrosshair(new BVirtualCrosshair(origin, direction), limit, Physics.DefaultRaycastLayers);
        }
        public static BCrosshair Crosshair(IBaseLocation origin, IBaseDirection direction, float limit, int mask)
        {
            return new BCrosshair(new BVirtualCrosshair(origin, direction), limit, mask);
        }

        public static BRaycast Raycast()
        {
            return new BRaycast(Caster(), float.PositiveInfinity, Physics.DefaultRaycastLayers);
        }
        public static BRaycast Raycast(float limit)
        {
            return new BRaycast(Caster(), limit, Physics.DefaultRaycastLayers);
        }
        public static BRaycast Raycast(IBaseCrosshair who)
        {
            return new BRaycast(
                who,
                float.PositiveInfinity,
                Physics.DefaultRaycastLayers
            );
        }
        public static BRaycast Raycast(IBaseCrosshair who, float limit)
        {
            return new BRaycast(
                who,
                limit,
                Physics.DefaultRaycastLayers
            );
        }
        public static BRaycast Raycast(IBaseLocation origin, IBaseDirection direction)
        {
            return new BRaycast(new BVirtualCrosshair(origin, direction), float.PositiveInfinity, Physics.DefaultRaycastLayers);
        }
        public static BRaycast Raycast(IBaseLocation origin, IBaseDirection direction, float limit)
        {
            return new BRaycast(new BVirtualCrosshair(origin, direction), limit, Physics.DefaultRaycastLayers);
        }
        public static BRaycast Raycast(IBaseLocation origin, IBaseDirection direction, float limit, int mask)
        {
            return new BRaycast(new BVirtualCrosshair(origin, direction), limit, mask);
        }

        public static BSphereoverlap Sphereoverlap(float radius)
        {
            return new BSphereoverlap(Skill(), radius, Physics.AllLayers);
        }
        public static BSphereoverlap Sphereoverlap(float radius, IBaseLocation at)
        {
            return new BSphereoverlap(at, radius, Physics.AllLayers);
        }
        public static BSphereoverlap Sphereoverlap(float radius, IBaseLocation at, int mask)
        {
            return new BSphereoverlap(at, radius, mask);
        }
        #endregion
        #region 선택식
        public static BNAny Any()
        {
            return new BNAny();
        }
        public static BFirst First()
        {
            return new BFirst(Any());
        }
        public static BFirst First(IBaseTargets targets)
        {
            return new BFirst(targets);
        }
        #endregion
        #region 추가식
        public static BExtend Extend(params IBaseTargets[] targeter)
        {
            return new BExtend(targeter.ToList());
        }
        #endregion
        #region 동작식
        public static IBase Cost(float mana)
        {
            return new BNWork((state) =>
            {
                if (!state.caster.status.PayMana(mana))
                {
                    state.ChangeState(RunState.Fail);
                }
            });
        }
        public static BCost Cost(IBaseTarget targeter, float mana)
        {
            return new BCost(targeter, mana);
        }
        public static BDamage Damage(float coefficient)
        {
            return new BDamage(Any(), Caster(), coefficient);
        }
        public static BDamage Damage(IBaseTargets targets, float coefficient)
        {
            return new BDamage(targets, Caster(), coefficient);
        }
        public static BDamage Damage(IBaseTargets targets, IBaseTarget dealer, float coefficient)
        {
            return new BDamage(targets, dealer, coefficient);
        }



        public static BMoveBy MoveBy(float speed)
        {
            return MoveBy(Skill(), speed, float.PositiveInfinity);
        }
        public static BMoveBy MoveBy(float speed, float angularSpeed)
        {
            return MoveBy(Skill(), speed, angularSpeed);
        }
        public static BMoveBy MoveBy(IBaseDirection direction, float speed)
        {
            return MoveBy(direction, speed, float.PositiveInfinity);
        }
        public static BMoveBy MoveBy(IBaseDirection direction, float speed, float angularSpeed)
        {
            return new BMoveBy(speed, angularSpeed, direction);
        }

        public static BMoveTo MoveTo(IBaseLocation location)
        {
            return MoveTo(location, float.PositiveInfinity, float.PositiveInfinity);
        }
        public static BMoveTo MoveTo(IBaseLocation location, float speed)
        {
            return MoveTo(location, speed, float.PositiveInfinity);
        }
        public static BMoveTo MoveTo(IBaseLocation location, float speed, float angularSpeed)
        {
            return new BMoveTo(speed, angularSpeed, location);
        }
        #endregion
        #region 인터럽트
        public static BHContact Contact()
        {
            return new BHContact(Any());
        }
        public static BHContact Contact(IFilter cond)
        {
            return new BHContact(cond);
        }
        #endregion
    }
    
    public class BVirtualCrosshair : IBaseCrosshair
    {
        private IBaseLocation loc;
        private IBaseDirection dir;

        public BVirtualCrosshair(IBaseLocation loc, IBaseDirection dir)
        {
            this.loc = loc;
            this.dir = dir;
        }

        public INodeCrosshair Build(State state)
        {
            return new NVirtualCrosshair(this.loc.Build(state), this.dir.Build(state));
        }
    }
    public class NVirtualCrosshair : INodeCrosshair
    {
        private INodeLocation loc;
        private INodeDirection dir;

        public NVirtualCrosshair(INodeLocation loc, INodeDirection dir)
        {
            this.loc = loc;
            this.dir = dir;
        }

        public Nullable<Quaternion> GetCameraDirection(State state)
        {
            return this.dir.GetDirection(state);
        }

        public Nullable<Vector3> GetCameraLocation(State state)
        {
            return this.loc.GetLocation(state);
        }

        public void Reset(State state)
        {

        }
    }

    public class BNCaster : IStateless, IBNLocation, IBNDirection, IBNCrosshair, IBNTarget, IBNTargets
    {
        INode IBase.Build(State state)
        {
            return this;
        }

        public void Calling(State state)
        {
            state.targets.Add(state.caster.gameObject);
        }

        public void Reset(State state)
        {
        }

        INodeLocation IBaseLocation.Build(State state)
        {
            return this;
        }

        void INodeLocation.Reset(State state) { }

        Nullable<Vector3> INodeLocation.GetLocation(State state)
        {
            return state.caster.transform.position;
        }

        INodeDirection IBaseDirection.Build(State state)
        {
            return this;
        }

        void INodeDirection.Reset(State state) { }

        Nullable<Quaternion> INodeDirection.GetDirection(State state)
        {
            return state.caster.transform.rotation;
        }

        INodeCrosshair IBaseCrosshair.Build(State state)
        {
            return this;
        }

        void INodeCrosshair.Reset(State state) { }

        Nullable<Vector3> INodeCrosshair.GetCameraLocation(State state)
        {
            // TODO Caster CameraLocation
            return state.caster.transform.position;
        }

        Nullable<Quaternion> INodeCrosshair.GetCameraDirection(State state)
        {
            // TODO Caster CameraRotation
            return state.caster.transform.rotation;
        }

        INodeTarget IBaseTarget.Build(State state)
        {
            return this;
        }

        void INodeTarget.Reset(State state) { }

        GameObject INodeTarget.GetTarget(State state)
        {
            return state.caster.gameObject;
        }

        INodeTargets IBaseTargets.Build(State state)
        {
            return this;
        }

        void INodeTargets.Reset(State state) { }

        IEnumerable<GameObject> INodeTargets.GetTargets(State state)
        {
            return new GameObject[] { state.caster.gameObject };
        }

        public IBaseLocation TargetPos()
        {
            return new BTargetLocDir(this);
        }

        public IBaseDirection TargetRot()
        {
            return new BTargetLocDir(this);
        }
    }
    public class BNSkill : IStateless, IBNLocation, IBNDirection, IBNCrosshair, IBNTarget, IBNTargets
    {
        void INode.Calling(State state)
        {
            state.targets.Add(state.skill.gameObject);
        }

        INode IBase.Build(State state)
        {
            return this;
        }

        void INode.Reset(State state)
        {
        }

        INodeLocation IBaseLocation.Build(State state)
        {
            return this;
        }

        void INodeLocation.Reset(State state)
        {
        }

        Nullable<Vector3> INodeLocation.GetLocation(State state)
        {
            return state.skill.transform.position;
        }

        INodeDirection IBaseDirection.Build(State state)
        {
            return this;
        }

        void INodeDirection.Reset(State state)
        {
        }

        Nullable<Quaternion> INodeDirection.GetDirection(State state)
        {
            return state.skill.transform.rotation;
        }

        INodeTarget IBaseTarget.Build(State state)
        {
            return this;
        }

        void INodeTarget.Reset(State state)
        {
        }

        GameObject INodeTarget.GetTarget(State state)
        {
            return state.skill.gameObject;
        }

        INodeTargets IBaseTargets.Build(State state)
        {
            return this;
        }

        void INodeTargets.Reset(State state)
        {
        }

        IEnumerable<GameObject> INodeTargets.GetTargets(State state)
        {
            return new GameObject[] { state.skill.gameObject };
        }

        INodeCrosshair IBaseCrosshair.Build(State state)
        {
            return this;
        }

        void INodeCrosshair.Reset(State state)
        {
        }

        Nullable<Vector3> INodeCrosshair.GetCameraLocation(State state)
        {
            return state.skill.transform.position;
        }

        Nullable<Quaternion> INodeCrosshair.GetCameraDirection(State state)
        {
            return state.skill.transform.rotation;
        }

        public IBaseLocation TargetPos()
        {
            return new BTargetLocDir(this);
        }

        public IBaseDirection TargetRot()
        {
            return new BTargetLocDir(this);
        }
    }

    public class BNWork : IStateless
    {
        private Action<State> work;
        public BNWork(Action<State> work)
        {
            this.work = work;
        }

        public INode Build(State state)
        {
            return this;
        }

        public void Calling(State state)
        {
            work(state);
        }

        public void Reset(State state)
        {
        }
    }
    
    public class BExtend : IBase, IBaseTargets
    {
        private List<IBaseTargets> targeters;

        public BExtend(List<IBaseTargets> targeters)
        {
            this.targeters = targeters;
        }

        public INode Build(State state)
        {
            return new NExtend(targeters.Select((t) => t.Build(state)).ToList());
        }

        INodeTargets IBaseTargets.Build(State state)
        {
            return new NExtend(targeters.Select((t) => t.Build(state)).ToList());
        }
    }
    public class NExtend : INode, INodeTargets
    {
        private List<INodeTargets> targeters;

        public NExtend(List<INodeTargets> targeters)
        {
            this.targeters = targeters;
        }

        public void Calling(State state)
        {

        }

        public IEnumerable<GameObject> GetTargets(State state)
        {

            var result = new List<GameObject>();
            foreach (var t in targeters)
            {
                var tmp = t.GetTargets(state);
                if (tmp is null)
                {
                    return null;
                }
                result.AddRange(tmp);
            }
            return result;
        }

        public void Reset(State state)
        {
            foreach (var t in targeters)
            {
                t.Reset(state);
            }
        }
    }
    public class BRaycast : IBase, IBaseTargets
    {
        private IBaseCrosshair baseCamera;
        private float limit;
        private int mask;

        public BRaycast(IBaseCrosshair baseCamera, float limit, int mask)
        {
            this.baseCamera = baseCamera;
            this.limit = limit;
            this.mask = mask;
        }

        INode IBase.Build(State state)
        {
            return new NRaycast(baseCamera.Build(state), limit, mask);
        }

        INodeTargets IBaseTargets.Build(State state)
        {
            return new NRaycast(baseCamera.Build(state), limit, mask);
        }
    }
    public class NRaycast : INode, INodeTargets
    {
        private INodeCrosshair baseCamera;
        private float limit;
        private int mask;

        public NRaycast(INodeCrosshair baseCamera, float limit, int mask)
        {
            this.baseCamera = baseCamera;
            this.limit = limit;
            this.mask = mask;
        }

        public void Calling(State state)
        {
            var res = this.GetTargets(state);
            if (res is null)
            {
                state.ChangeState(RunState.Fail);
            }
            else
            {
                state.targets.AddRange(res);
            }

        }

        void INode.Reset(State state)
        {
            baseCamera.Reset(state);
        }

        public IEnumerable<GameObject> GetTargets(State state)
        {
            var nl = baseCamera.GetCameraLocation(state);
            var nd = baseCamera.GetCameraDirection(state);
            if (!nl.HasValue || !nd.HasValue)
            {
                return null;
            }
            return Physics.RaycastAll(nl.Value, nd.Value.eulerAngles, limit, mask).Select((hit) => hit.transform.gameObject);
        }

        void INodeTargets.Reset(State state)
        {
            baseCamera.Reset(state);
        }
    }
    public class BCrosshair : IBase, IBaseLocation, IBaseTarget, IBaseTargets
    {
        private IBaseCrosshair baseCrosshair;
        private float limit;
        private int mask;

        public BCrosshair(IBaseCrosshair baseCrosshair, float limit, int mask)
        {
            this.baseCrosshair = baseCrosshair;
            this.limit = limit;
            this.mask = mask;
        }

        public IBaseLocation TargetPos()
        {
            return new BTargetLocDir(this);
        }

        public IBaseDirection TargetRot()
        {
            return new BTargetLocDir(this);
        }

        INode IBase.Build(State state)
        {
            return new NCrosshair(baseCrosshair.Build(state), limit, mask);
        }

        INodeLocation IBaseLocation.Build(State state)
        {
            return new NCrosshair(baseCrosshair.Build(state), limit, mask);
        }

        INodeTarget IBaseTarget.Build(State state)
        {
            return new NCrosshair(baseCrosshair.Build(state), limit, mask);
        }

        INodeTargets IBaseTargets.Build(State state)
        {
            return new NCrosshair(baseCrosshair.Build(state), limit, mask);
        }
    }
    public class NCrosshair : INode, INodeLocation, INodeTarget, INodeTargets
    {

        private INodeCrosshair nodeCrosshair;
        private float limit;
        private int mask;

        public NCrosshair(INodeCrosshair nodeCrosshair, float limit, int mask)
        {
            this.nodeCrosshair = nodeCrosshair;
            this.limit = limit;
            this.mask = mask;
        }

        public void Calling(State state)
        {
            var res = this.GetTarget(state);
            if (res is null)
            {
                state.ChangeState(RunState.Fail);
            }
            else
            {
                state.targets.Add(res);
            }
        }

        public Nullable<Vector3> GetLocation(State state)
        {
            var nl = nodeCrosshair.GetCameraLocation(state);
            var nd = nodeCrosshair.GetCameraDirection(state);

            // clocdir.Data.Item1
            RaycastHit output;
            if (nl.HasValue && nd.HasValue && Physics.Raycast(nl.Value, nd.Value.eulerAngles, out output, limit, mask))
            {
                return output.point;
            }
            else
            {
                return null;
            }
        }

        public GameObject GetTarget(State state)
        {

            var nl = nodeCrosshair.GetCameraLocation(state);
            var nd = nodeCrosshair.GetCameraDirection(state);
            // clocdir.Data.Item1
            RaycastHit output;
            if (nl.HasValue && nd.HasValue && Physics.Raycast(nl.Value, nd.Value.eulerAngles, out output, limit, mask))
            {
                return output.transform.gameObject;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<GameObject> GetTargets(State state)
        {
            var target = GetTarget(state);
            return target is null ? null : new GameObject[] { target };
        }

        public void Reset(State state)
        {
            nodeCrosshair.Reset(state);
        }

    }

    public class BTargetLocDir : IBaseLocation, IBaseDirection
    {
        private IBaseTarget target;
        public BTargetLocDir(IBaseTarget target)
        {
            this.target = target;
        }

        INodeLocation IBaseLocation.Build(State state)
        {
            return new NTargetLocDir(target.Build(state));
        }

        INodeDirection IBaseDirection.Build(State state)
        {
            return new NTargetLocDir(target.Build(state));
        }
    }
    public class NTargetLocDir : INodeLocation, INodeDirection
    {

        private INodeTarget target;

        public NTargetLocDir(INodeTarget target)
        {
            this.target = target;
        }

        public Nullable<Quaternion> GetDirection(State state)
        {
            return target.GetTarget(state)?.transform?.rotation;
        }

        public Nullable<Vector3> GetLocation(State state)
        {
            return target.GetTarget(state)?.transform?.position;
        }

        public void Reset(State state)
        {
            target.Reset(state);
        }
    }

    public class BSphereoverlap : IBase, IBaseTargets
    {
        private IBaseLocation basePosition;
        private float radius;
        private int mask;

        public BSphereoverlap(IBaseLocation basePosition, float radius, int mask)
        {
            this.basePosition = basePosition;
            this.radius = radius;
            this.mask = mask;
        }

        INode IBase.Build(State state)
        {
            return new NSphereoverlap(basePosition.Build(state), radius, mask);
        }

        INodeTargets IBaseTargets.Build(State state)
        {
            return new NSphereoverlap(basePosition.Build(state), radius, mask);
        }
    }
    public class NSphereoverlap : INode, INodeTargets
    {
        private INodeLocation nodePosition;
        private float radius;
        private int mask;

        public NSphereoverlap(INodeLocation baseCamera, float radius, int mask)
        {
            this.nodePosition = baseCamera;
            this.radius = radius;
            this.mask = mask;
        }

        public void Calling(State state)
        {
            var res = this.GetTargets(state);
            if (res is null)
            {
                state.ChangeState(RunState.Fail);
            }
            else
            {
                state.targets.AddRange(res);
            }

        }

        void INode.Reset(State state)
        {
            nodePosition.Reset(state);
        }

        public IEnumerable<GameObject> GetTargets(State state)
        {
            var cloc = nodePosition.GetLocation(state);
            if (cloc.HasValue)
            {
                return Physics.OverlapSphere(cloc.Value, radius, mask).Select((hit) => hit.transform.gameObject);
            }
            return null;
        }

        void INodeTargets.Reset(State state)
        {
            nodePosition.Reset(state);
        }
    }

    public class BMoveBy : IBase
    {
        private float speed;
        private float angularSpeed;
        private IBaseDirection direction;

        public BMoveBy(float speed, float angularSpeed, IBaseDirection direction)
        {
            this.speed = speed;
            this.angularSpeed = angularSpeed;
            this.direction = direction;
        }

        public INode Build(State state)
        {
            return new NMoveBy(speed, angularSpeed, direction.Build(state));
        }
    }
    public class NMoveBy : INode
    {
        private float speed;
        private float angularSpeed;
        private INodeDirection direction;

        public NMoveBy(float speed, float angularSpeed, INodeDirection direction)
        {
            this.speed = speed;
            this.angularSpeed = angularSpeed;
            this.direction = direction;
        }

        public void Calling(State state)
        {
            var dir = direction.GetDirection(state);
            if (dir.HasValue)
            {
                state.skill.transform.rotation = Quaternion.RotateTowards(state.skill.transform.rotation, dir.Value, angularSpeed * state.deltatime);
                state.skill.transform.position = state.skill.transform.rotation * state.skill.transform.forward * (speed * state.deltatime);
            }
            else
            {
                state.ChangeState(RunState.Fail);
            }
        }

        public void Reset(State state)
        {
            direction.Reset(state);
        }
    }
    public class BMoveTo : IBase
    {
        private float speed;
        private float angularSpeed;
        private IBaseLocation destination;

        public BMoveTo(float speed, float angularSpeed, IBaseLocation destination)
        {
            this.speed = speed;
            this.angularSpeed = angularSpeed;
            this.destination = destination;
        }

        public INode Build(State state)
        {
            return new NMoveTo(speed, angularSpeed, destination.Build(state));
        }
    }
    public class NMoveTo : INode
    {

        private float speed;
        private float angularSpeed;
        private INodeLocation destination;

        public NMoveTo(float speed, float angularSpeed, INodeLocation destination)
        {
            this.speed = speed;
            this.angularSpeed = angularSpeed;
            this.destination = destination;
        }

        public void Calling(State state)
        {
            var loc = destination.GetLocation(state);
            if (loc.HasValue)
            {
                var dir = Quaternion.FromToRotation(state.skill.transform.position, loc.Value);
                state.skill.transform.rotation = Quaternion.RotateTowards(state.skill.transform.rotation, dir, angularSpeed * state.deltatime);
                state.skill.transform.position = state.skill.transform.rotation * state.skill.transform.forward * (speed * state.deltatime);
            }
            else
            {
                state.ChangeState(RunState.Fail);
            }
        }

        public void Reset(State state)
        {
            destination.Reset(state);
        }
    }

    public class BDamage : IBase
    {
        private IBaseTargets targeter;
        private IBaseTarget dealer;
        private float coefficient;

        public BDamage(IBaseTargets targeter, IBaseTarget dealer, float coefficient)
        {
            this.coefficient = coefficient;
            this.dealer = dealer;
            this.targeter = targeter;
        }

        public INode Build(State state)
        {
            return new NDamage(targeter.Build(state), dealer.Build(state), coefficient);
        }
    }
    public class NDamage : INode
    {
        private INodeTargets targeter;
        private INodeTarget dealer;
        private float coefficient;

        public NDamage(INodeTargets targeter, INodeTarget dealer, float coefficient)
        {
            this.coefficient = coefficient;
            this.dealer = dealer;
            this.targeter = targeter;
        }

        public void Calling(State state)
        {
            var ctrgs = targeter.GetTargets(state);
            var cdealer = dealer.GetTarget(state);
            if (ctrgs is null || cdealer is null)
            {
                state.ChangeState(RunState.Fail);
            }
            else
            {
                var dealer = cdealer.GetComponent<Status>();
                var targets = ctrgs.Select((t) => t.GetComponent<Status>()).Where((t) => !(t is null));
                foreach (var item in targets)
                {
                    item.Damage(dealer, coefficient);
                }
            }
        }

        public void Reset(State state)
        {
            targeter.Reset(state);
            dealer.Reset(state);
        }
    }

    public class BLoop : IBase
    {
        private BBlock block;
        private int count;

        public BLoop(BBlock block, int count)
        {
            this.block = block;
            this.count = count;
        }

        public INode Build(State state)
        {
            return new NLoop(block.Build(state) as NBlock, count);
        }
    }
    public class NLoop : INode
    {
        private NBlock block;
        private int count;
        private int index;
        public NLoop(NBlock block, int count)
        {
            this.block = block;
            this.count = count;
            this.index = 0;
        }

        public void Calling(State state)
        {
            if (index >= count)
            {
                Reset(state);
                state.ChangeState(RunState.End);
                return;
            }
            count++;
            block.Calling(state);
            switch (state.runState)
            {
                case RunState.Break:
                    index = count;
                    Reset(state);
                    state.ChangeState(RunState.End);
                    return;
                case RunState.Continue:
                    return;
                case RunState.End:
                    return;
                case RunState.Exit:
                    index = count;
                    Reset(state);
                    state.ChangeState(RunState.Exit);
                    return;
                case RunState.Yeild:
                case RunState.Wait:
                case RunState.Fail:
                    return;
            }
            state.ChangeState(RunState.Yeild);
        }

        public void Reset(State state)
        {
            index = 0;
            block.Reset(state);
        }
    }

    public class BNLambdaFilter : IStateless, IFilter
    {
        private Func<GameObject, bool> pred;

        public BNLambdaFilter(Func<GameObject, bool> pred)
        {
            this.pred = pred;
        }

        public INode Build(State state)
        {
            return this;
        }

        public void Calling(State state)
        {
        }

        public IEnumerable<GameObject> Filtering(State state, IEnumerable<GameObject> from)
        {
            return from.Where(pred);
        }

        public void Reset(State state)
        {
        }
    }
    public class BNAny : IBNTargets, IFilter
    {
        public IEnumerable<GameObject> Filtering(State state, IEnumerable<GameObject> from)
        {
            return from;
        }

        public IEnumerable<GameObject> GetTargets(State state)
        {
            return state.targets;
        }

        public void Reset(State state)
        {
        }

        INodeTargets IBaseTargets.Build(State state)
        {
            return this;
        }

    }
    public class BFirst : IBaseTarget, IFilter
    {
        private IBaseTargets targets;

        public BFirst(IBaseTargets targets)
        {
            this.targets = targets;
        }

        public IEnumerable<GameObject> Filtering(State state, IEnumerable<GameObject> from)
        {
            return from.Take(1);
        }

        public IBaseLocation TargetPos()
        {
            return new BTargetLocDir(this);
        }

        public IBaseDirection TargetRot()
        {
            return new BTargetLocDir(this);
        }

        INodeTarget IBaseTarget.Build(State state)
        {
            return new NFirst(targets.Build(state));
        }
    }
    public class NFirst : INodeTarget
    {
        private INodeTargets targets;

        public NFirst(INodeTargets targets)
        {
            this.targets = targets;
        }

        public GameObject GetTarget(State state)
        {
            var res = targets.GetTargets(state);
            if (res is null)
            {
                return null;
            }
            return res.FirstOrDefault(null);
        }

        public void Reset(State state)
        {
            targets.Reset(state);
        }
    }

    public class BCost : IBase
    {
        private IBaseTarget target;
        private float cost;

        public BCost(IBaseTarget target, float cost)
        {
            this.target = target;
            this.cost = cost;
        }

        public INode Build(State state)
        {
            return new NCost(target.Build(state), cost);
        }
    }
    public class NCost : INode
    {
        private INodeTarget target;
        private float cost;

        public NCost(INodeTarget target, float cost)
        {
            this.target = target;
            this.cost = cost;
        }

        public void Calling(State state)
        {
            var ctrg = target.GetTarget(state);
            if (!(ctrg?.GetComponent<Status>()?.PayMana(cost) ?? false))
            {
                state.ChangeState(RunState.Fail);
                return;
            }
        }

        public void Reset(State state)
        {
            target.Reset(state);
        }
    }



    public class BBlock : IBase
    {
        private List<IBase> codes;

        public BBlock(List<IBase> codes)
        {
            this.codes = codes;
        }

        public INode Build(State state)
        {
            return new NBlock(codes.Select((line) => line.Build(state)).ToList());
        }

    }
    public class NBlock : INode
    {

        private int index = 0;
        private List<INode> codes;

        public NBlock(List<INode> codes)
        {
            this.codes = codes;
        }

        public void Reset(State state)
        {
            index = 0;
            foreach (var item in codes)
            {
                item.Calling(state);
            }
        }

        public void Calling(State state)
        {
            for (; index < codes.Count; index++)
            {

                codes[index].Calling(state);
                switch (state.runState)
                {
                    case RunState.Break:
                        break;
                    case RunState.Yeild:
                    case RunState.Wait:
                    case RunState.Fail:
                        return;
                }
            }
            state.ChangeState(RunState.End);
        }
    }

    public class BUntil : IBase
    {
        private IBaseInterrupt condition;
        private BBlock bBlock;

        public BUntil(IBaseInterrupt condition, BBlock bBlock)
        {
            this.condition = condition;
            this.bBlock = bBlock;
        }

        public INode Build(State state)
        {
            return new NUntil(condition.Build(state, RunState.Break), bBlock.Build(state) as NBlock);
        }
    }
    public class NUntil : INode
    {
        private INodeInterrupt condition;
        private NBlock nBlock;
        private bool isInstalled = false;

        public NUntil(INodeInterrupt condition, NBlock nBlock)
        {
            this.condition = condition;
            this.nBlock = nBlock;
        }

        public void Calling(State state)
        {
            if (isInstalled)
            {
                if (state.runState is RunState.Break)
                {
                    condition.Uninstall(state);
                    isInstalled = false;
                    state.ChangeState(RunState.End);
                    return;
                }
            }
            else
            {
                condition.Install(state);
                isInstalled = true;
            }
            nBlock.Calling(state);
            switch (state.runState)
            {
                case RunState.End:
                    state.ChangeState(RunState.Run);
                    return;
            }
        }

        public void Reset(State state)
        {
            nBlock.Reset(state);
            if (isInstalled)
            {
                condition.Uninstall(state);
                isInstalled = false;
            }
        }
    }

    public class BHContact : IBaseInterrupt
    {
        private IFilter filter;

        public BHContact(IFilter filter)
        {
            this.filter = filter;
        }

        public INodeInterrupt Build(State state, RunState targetState)
        {
            return new NHContact(filter, targetState);
        }
    }
    public class NHContact : INodeInterrupt
    {
        private IFilter filter;
        private RunState targetState;
        public NHContact(IFilter filter, RunState targetState)
        {
            this.filter = filter;
            this.targetState = targetState;
        }

        public void Install(State state)
        {
            state.OnContact += OnContact;
        }

        public void Uninstall(State state)
        {
            state.OnContact -= OnContact;
        }
        public void OnContact(State state, GameObject other)
        {
            if (filter.Filtering(state, new GameObject[] { other }).Any())
            {
                state.ChangeState(targetState);
            }
        }
    }
    // public class HTimer { }
    // public class HInterval { }
}