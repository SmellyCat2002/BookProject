using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 原始输入数据 - 从硬件直接读取
    /// </summary>
    public struct RawInputData
    {
        #region 轴向输入
        public Vector2 MoveAxis;
        public Vector2 LookAxis;
        #endregion

        #region 按住状态
        public bool JumpHeld;
        public bool SprintHeld;
        public bool DodgeHeld;
        public bool CrouchHeld;
        public bool AttackHeld;
        #endregion

        #region 刚刚按下
        public bool JumpJustPressed;
        public bool SprintJustPressed;
        public bool DodgeJustPressed;
        public bool CrouchJustPressed;
        public bool AttackJustPressed;
        #endregion
    }
}