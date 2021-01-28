using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using skill;
[CreateAssetMenu(fileName = "SkillStorage", menuName = "Skill/Storage", order = 1)]
public class SkillData : ScriptableObject
{

    public SkillIdentifier identifier;
    public string description;
    public float cooltime;
    public skill.IBase spellBase { get { return SpellBuild(identifier); } }
    private IBase SpellBuild(SkillIdentifier name)
    {


        switch (name)
        {
            case SkillIdentifier.TargetingFireball:

                return Node.Spell(
                    Node.Crosshair(),   // 시전자의 시야에 잡히는 타겟을 찾아 저장한다.
                    Node.Require(),     // 조준점상에 적이 존재해야만 나머지가 동작한다.
                    Node.Cost(100),     // 시전자의 마나를 100만큼 감소시킨다. 실패시 동작을 중지한다.
                    Node.Until(         // 무한루프를 통해 조건이 만족될때까지 계속 진행한다.
                        Node.Contact(), // 스펠에 설정된 충돌체를 이용해 충돌을 감지할 때 까지 진행한다.
                                        // 무한루프 동작
                                        // 무한루프는 기본적으로 한 업데이트당 한번의 루프를 돌게 된다.
                                        // 즉 Update 함수가 호출될 때  루프에 해당하는 코드가 한번씩 실행된다.
                        Node.MoveTo(Node.First().TargetPos(), 10, 1.5f) // 현재 스펠의 위치를 Crosshair 노드를 통해 찾아낸 적의 위치로 속도 10, 각속도 1.5로 이동시킨다.
                    ),
                    Node.Damage(Node.Sphereoverlap(1.3f), 1.2f) // 루프가 종료되면 데미지가 구형범위 내의 모든 목표물에게 1.3m이내로 1.2계수로 들어간다.
                );
            default:
                return null;
        }
    }
}
public enum SkillIdentifier
{
    NontargetFireball,
    TargetingFireball,
    GuidedFireball,
}