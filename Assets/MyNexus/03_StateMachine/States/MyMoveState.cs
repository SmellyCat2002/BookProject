using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 移动状态 - 复刻第4步
    /// 角色正在移动
    /// </summary>
    public class MyMoveState : MyBaseState
    {
        private MyCharacterController _controller;
        private CharacterController _charController;

        // 移动参数
        private float _walkSpeed = 5f;
        private float _runSpeed = 8f;

        public MyMoveState(MyCharacterController controller)
        {
            _controller = controller;
            _charController = controller.GetComponent<CharacterController>();
        }

        public override void Enter()
        {
            Debug.Log("[State] 进入 Move 状态");
        }

        public override void LogicUpdate()
        {
            var runtime = _controller.RuntimeData;

            // 1. 输入转换：基于相机方向，将2D输入转为3D世界方向
            if (runtime.CameraTransform != null)
            {
                // 相机的前方（忽略Y轴）
                Vector3 camForward = runtime.CameraTransform.forward;
                camForward.y = 0f;
                camForward.Normalize();

                Vector3 camRight = runtime.CameraTransform.right;
                camRight.y = 0f;
                camRight.Normalize();

                // 混合输入 + 相机方向 = 世界方向
                runtime.DesiredWorldMoveDir = (camForward * runtime.MoveInput.y + camRight * runtime.MoveInput.x).normalized;
            }
            else
            {
                runtime.DesiredWorldMoveDir = new Vector3(runtime.MoveInput.x, 0, runtime.MoveInput.y).normalized;
            }

            // 2. 没有输入了，回到Idle
            if (runtime.MoveInput.magnitude <= 0.1f)
            {
                _controller.StateMachine.ChangeState(new MyIdleState(_controller));
            }
        }

        public override void PhysicsUpdate()
        {
            var runtime = _controller.RuntimeData;

            // 3. 应用移动
            float speed = runtime.WantsToRun ? _runSpeed : _walkSpeed;
            Vector3 moveDir = runtime.DesiredWorldMoveDir * speed;

            // 4. 处理重力
            if (runtime.IsGrounded && runtime.VerticalVelocity < 0)
            {
                runtime.VerticalVelocity = -2f; // 保持接地
            }

            moveDir.y = runtime.VerticalVelocity;

            // 5. 执行移动
            _charController.Move(moveDir * Time.deltaTime);

            // 6. 更新朝向（面向移动方向）
            if (runtime.DesiredWorldMoveDir.sqrMagnitude > 0.01f)
            {
                float targetYaw = Mathf.Atan2(runtime.DesiredWorldMoveDir.x, runtime.DesiredWorldMoveDir.z) * Mathf.Rad2Deg;
                runtime.CurrentYaw = Mathf.MoveTowardsAngle(runtime.CurrentYaw, targetYaw, 720f * Time.deltaTime);
                _controller.transform.rotation = Quaternion.Euler(0, runtime.CurrentYaw, 0);
            }

            // 7. 记录当前速度
            runtime.CurrentSpeed = moveDir.magnitude;
        }

        public override void Exit()
        {
            Debug.Log("[State] 退出 Move 状态");
        }
    }
}