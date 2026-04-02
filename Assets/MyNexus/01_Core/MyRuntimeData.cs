using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 运行时数据黑板 - 复刻BBBNexus的第2步
    ///
    /// 设计原则：
    /// 1. 只承载数据，不包含逻辑
    /// 2. 数据分"持续状态"和"帧级意图"
    /// 3. 帧级意图每帧Reset
    /// </summary>
    public class MyRuntimeData
    {
        // 持有主控器引用（方便其他系统访问）
        private MyCharacterController _controller;

        public MyRuntimeData(MyCharacterController controller)
        {
            _controller = controller;
            CameraTransform = controller.PlayerCamera;
        }

        // ============================================
        // 第一类：持续状态（不会每帧重置）
        // ============================================

        #region 物理与地面状态
        /// <summary>是否接地</summary>
        public bool IsGrounded;
        /// <summary>竖直速度</summary>
        public float VerticalVelocity;
        #endregion

        #region 输入状态（持续的）
        /// <summary>相机视角输入</summary>
        public Vector2 LookInput;
        /// <summary>移动摇杆输入</summary>
        public Vector2 MoveInput;
        #endregion

        #region 视角与旋转
        /// <summary>相机Transform</summary>
        public Transform CameraTransform;
        /// <summary>角色当前朝向</summary>
        public float CurrentYaw;
        #endregion

        #region 运动状态
        /// <summary>当前水平速度</summary>
        public float CurrentSpeed;
        /// <summary>期望移动方向</summary>
        public Vector3 DesiredWorldMoveDir;
        #endregion

        // ============================================
        // 第二类：帧级意图（每帧Reset）
        // ============================================

        #region 帧级意图标志
        /// <summary>本帧是否想跳跃</summary>
        public bool WantsToJump;
        /// <summary>本帧是否想跑</summary>
        public bool WantsToRun;
        #endregion

        // ============================================
        // 方法
        // ============================================

        /// <summary>
        /// 每帧结束时调用：清除所有帧级意图
        /// 为什么重要？防止上一帧的意图影响下一帧
        /// </summary>
        public void ResetIntent()
        {
            WantsToJump = false;
            WantsToRun = false;
        }
    }
}
