using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 移动状态 - 复刻第3步
    /// 角色正在移动
    /// </summary>
    public class MyMoveState : MyBaseState
    {
        private MyCharacterController _controller;

        public MyMoveState(MyCharacterController controller)
        {
            _controller = controller;
        }

        public override void Enter()
        {
            Debug.Log("[State] 进入 Move 状态");
        }

        public override void LogicUpdate()
        {
            // 在Move状态，如果没有输入了，回到Idle
            if (_controller.RuntimeData.MoveInput.magnitude <= 0.1f)
            {
                _controller.StateMachine.ChangeState(new MyIdleState(_controller));
            }
        }

        public override void PhysicsUpdate()
        {
            // 这里将来处理角色移动
        }

        public override void Exit()
        {
            Debug.Log("[State] 退出 Move 状态");
        }
    }
}