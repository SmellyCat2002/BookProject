using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 玩家输入源 - 键盘/手柄输入
    /// </summary>
    public class PlayerInputSource : IInputSource
    {
        #region 按键配置
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode crouchKey = KeyCode.C;
        public KeyCode dodgeKey = KeyCode.Q;
        public KeyCode attackKey = KeyCode.Mouse0;
        #endregion

        #region IInputSource 实现
        public void FetchRawInput(ref RawInputData rawData)
        {
            // 移动轴
            rawData.MoveAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            rawData.LookAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // 按住状态
            rawData.JumpHeld = Input.GetKey(jumpKey);
            rawData.SprintHeld = Input.GetKey(sprintKey);
            rawData.DodgeHeld = Input.GetKey(dodgeKey);
            rawData.CrouchHeld = Input.GetKey(crouchKey);
            rawData.AttackHeld = Input.GetKey(attackKey);

            // 刚刚按下
            rawData.JumpJustPressed = Input.GetKeyDown(jumpKey);
            rawData.SprintJustPressed = Input.GetKeyDown(sprintKey);
            rawData.DodgeJustPressed = Input.GetKeyDown(dodgeKey);
            rawData.CrouchJustPressed = Input.GetKeyDown(crouchKey);
            rawData.AttackJustPressed = Input.GetKeyDown(attackKey);
        }
        #endregion
    }
}