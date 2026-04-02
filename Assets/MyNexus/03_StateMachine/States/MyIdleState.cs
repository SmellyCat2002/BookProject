using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 待机状态 - 复刻第3步
    /// 什么都不敢，等待输入
    /// </summary>
    public class MyIdleState : MyBaseState
    {
        // 持有主控器引用，方便访问
        private MyCharacterController _controller;

        public MyIdleState(MyCharacterController controller)
        {
            _controller = controller;
        }

        public override void Enter()
        {
            Debug.Log("[State] 进入 Idle 状态");
        }

        public override void LogicUpdate()
        {
            // 在Idle状态，检查是否有移动输入
            if (_controller.RuntimeData.MoveInput.magnitude > 0.1f)
            {
                // 有输入，切换到移动状态
                _controller.StateMachine.ChangeState(new MyMoveState(_controller));
            }
        }

        public override void PhysicsUpdate()
        {
            // Idle状态下处理重力等
        }

        public override void Exit()
        {
            Debug.Log("[State] 退出 Idle 状态");
        }
    }
}