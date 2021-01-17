using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.BehaviorTree
{
    /// <summary> Not조건 검사 노드 </summary>
    public class NotConditionNode : IDecoratorNode
    {
        public Func<bool> Condition { get; private set; }
        public NotConditionNode(Func<bool> condition)
        {
            Condition = () => !condition();
        }

        public bool Run() => Condition();

        // Func <=> ConditionNode 타입 캐스팅
        public static implicit operator NotConditionNode(Func<bool> condition) => new NotConditionNode(condition);
        public static implicit operator Func<bool>(NotConditionNode condition) => new Func<bool>(condition.Condition);

        // Decorated Node Creator Methods
        public IfActionNode Action(Action action)
            => new IfActionNode(Condition, action);

        public IfSequenceNode Sequence(params INode[] nodes)
            => new IfSequenceNode(Condition, nodes);

        public IfSelectorNode Selector(params INode[] nodes)
            => new IfSelectorNode(Condition, nodes);

        public IfParallelNode Parallel(params INode[] nodes)
            => new IfParallelNode(Condition, nodes);
    }
}