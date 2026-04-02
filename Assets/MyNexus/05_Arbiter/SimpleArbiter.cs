using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 简单仲裁器 - 复刻BBBNexus
    /// 检查意图是否可以执行
    /// </summary>
    public class SimpleArbiter
    {
        // 简单的体力系统
        public float MaxStamina = 100f;
        public float CurrentStamina { get; private set; }
        public float StaminaRecoveryRate = 20f;
        public float SprintStaminaCost = 15f;
        public float JumpStaminaCost = 20f;
        public float DodgeStaminaCost = 25f;

        // 冷却
        public float DodgeCooldown { get; private set; }
        public float DodgeCooldownTime = 0.5f;

        public SimpleArbiter()
        {
            CurrentStamina = MaxStamina;
            DodgeCooldown = 0f;
        }

        /// <summary>
        /// 每帧更新（恢复体力 + 冷却）
        /// </summary>
        public void Update(float deltaTime)
        {
            // 恢复体力
            if (CurrentStamina < MaxStamina)
            {
                CurrentStamina += StaminaRecoveryRate * deltaTime;
                CurrentStamina = Mathf.Min(CurrentStamina, MaxStamina);
            }

            // 冷却
            if (DodgeCooldown > 0)
            {
                DodgeCooldown -= deltaTime;
            }
        }

        /// <summary>
        /// 仲裁跳跃
        /// </summary>
        public bool TryJump(bool wantsToJump, bool isGrounded)
        {
            if (!wantsToJump) return false;
            if (!isGrounded) return false;
            if (CurrentStamina < JumpStaminaCost) return false;

            CurrentStamina -= JumpStaminaCost;
            return true;
        }

        /// <summary>
        /// 仲裁冲刺
        /// </summary>
        public bool TrySprint(bool wantsToRun)
        {
            if (!wantsToRun) return false;
            if (CurrentStamina < SprintStaminaCost) return false;

            // 冲刺消耗持续体力（在移动中扣）
            return true;
        }

        /// <summary>
        /// 消耗冲刺体力（每帧调用）
        /// </summary>
        public void ConsumeSprintStamina(float deltaTime)
        {
            if (CurrentStamina > 0)
            {
                CurrentStamina -= SprintStaminaCost * deltaTime;
            }
        }

        /// <summary>
        /// 仲裁闪避
        /// </summary>
        public bool TryDodge(bool wantsToDodge, bool isGrounded)
        {
            if (!wantsToDodge) return false;
            if (!isGrounded) return false;
            if (DodgeCooldown > 0) return false;
            if (CurrentStamina < DodgeStaminaCost) return false;

            CurrentStamina -= DodgeStaminaCost;
            DodgeCooldown = DodgeCooldownTime;
            return true;
        }
    }
}