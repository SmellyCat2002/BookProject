using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 处理后输入数据 - 带 InputBuffer
    /// 按下后 buffer timer 开始倒计时，倒计时结束前都算"按下"
    /// </summary>
    public struct ProcessedInputData
    {
        #region 轴向输入
        public Vector2 Move;
        public Vector2 Look;
        #endregion

        #region 按住状态（直接透传）
        public bool JumpHeld;
        public bool SprintHeld;
        public bool DodgeHeld;
        public bool CrouchHeld;
        public bool AttackHeld;
        #endregion

        #region Input Buffer 计时器
        public float JumpBufferTimer;
        public float SprintBufferTimer;
        public float DodgeBufferTimer;
        public float CrouchBufferTimer;
        public float AttackBufferTimer;
        #endregion

        #region 按下状态（通过 BufferTimer 判断）
        public bool JumpPressed => JumpBufferTimer > 0f;
        public bool SprintPressed => SprintBufferTimer > 0f;
        public bool DodgePressed => DodgeBufferTimer > 0f;
        public bool CrouchPressed => CrouchBufferTimer > 0f;
        public bool AttackPressed => AttackBufferTimer > 0f;
        #endregion
    }
}