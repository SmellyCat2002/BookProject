using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 输入管道 - 负责将原始硬件输入加工为带Buffer的处理后输入
    ///
    /// 工作流程：
    /// 1. 从InputSource获取原始输入(RawInputData)
    /// 2. 加工处理：移动轴防抖 + 按键Buffer处理
    /// 3. 输出处理后数据(ProcessedInputData)供状态机使用
    /// </summary>
    public class MyInputPipeline
    {
        #region 依赖
        /// <summary>输入源（键盘/手柄/AI，不写死具体实现方便切换）</summary>
        private readonly IInputSource inputSource;
        #endregion

        #region 配置
        /// <summary>按键Buffer时间（按一下后多少秒内算按下）</summary>
        private readonly float actionBufferTime;
        /// <summary>移动轴防抖时间（松开后多少秒内保持上一帧值）</summary>
        private readonly float inputFlickerBuffer;
        #endregion

        #region 状态
        /// <summary>输入数据容器（当前帧+上一帧）</summary>
        private InputDataContainer inputData;
        /// <summary>原始输入数据（临时）</summary>
        private RawInputData rawData;
        /// <summary>移动轴缓冲（用于防抖）</summary>
        private Vector2 bufferedMove;
        /// <summary>上一次非零移动输入的时间</summary>
        private float lastNonZeroMoveTime;
        /// <summary>帧索引</summary>
        private ulong frameIndex;
        #endregion

        #region 属性
        /// <summary>当前帧处理后的输入数据（供外部读取）</summary>
        public ProcessedInputData CurrentProcessed => inputData.CurrentFrame.Processed;
        #endregion

        #region 初始化
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inputSource">输入源实现</param>
        /// <param name="actionBufferTime">按键Buffer时间，默认0.2秒</param>
        /// <param name="flickerBuffer">移动轴防抖时间，默认0.05秒</param>
        public MyInputPipeline(IInputSource inputSource, float actionBufferTime = 0.2f, float flickerBuffer = 0.05f)
        {
            this.inputSource = inputSource;
            this.actionBufferTime = actionBufferTime;
            this.inputFlickerBuffer = flickerBuffer;

            // 初始化数据结构
            inputData = new InputDataContainer();
            inputData.CurrentFrame = new FrameInputData { FrameIndex = 0 };
            inputData.LastFrame = new FrameInputData { FrameIndex = 0 };
            rawData = default;
            bufferedMove = Vector2.zero;
            lastNonZeroMoveTime = Time.time;
        }
        #endregion

        #region 核心方法
        /// <summary>
        /// 每帧调用 - 更新输入数据
        /// 必须在Update中调用
        /// </summary>
        public void Update()
        {
            // 1. 保存上一帧数据
            inputData.LastFrame = inputData.CurrentFrame;

            // 2. 从输入源获取原始输入
            inputSource.FetchRawInput(ref rawData);

            // 3. 加工处理
            ProcessRawInput();

            // 4. 帧索引递增
            frameIndex++;
        }

        /// <summary>
        /// 核心：加工原始输入
        /// 1. 移动轴：防抖处理（FlickerBuffer）
        /// 2. 按键：InputBuffer处理
        /// </summary>
        private void ProcessRawInput()
        {
            var currentFrame = new FrameInputData
            {
                FrameIndex = frameIndex,
                Raw = rawData,
                Processed = default
            };

            #region 移动轴处理
            // 思路：摇杆有输入时使用当前值，松开的瞬间保留上一帧值一小段时间
            // 解决：摇杆回中时的抖动问题
            if (rawData.MoveAxis.sqrMagnitude > 0.01f)
            {
                // 有输入：更新缓冲值并记录时间
                bufferedMove = rawData.MoveAxis;
                lastNonZeroMoveTime = Time.time;
                currentFrame.Processed.Move = rawData.MoveAxis;
            }
            else if (Time.time - lastNonZeroMoveTime < inputFlickerBuffer)
            {
                // 无输入但在防抖时间内：使用缓冲值
                currentFrame.Processed.Move = bufferedMove;
            }
            else
            {
                // 超出防抖时间：归零
                currentFrame.Processed.Move = Vector2.zero;
            }
            #endregion

            #region 视角轴处理
            // 视角不需要Buffer，直接透传
            currentFrame.Processed.Look = rawData.LookAxis;
            #endregion

            #region 按住状态透传
            // 这些是持续状态，不需要Buffer
            currentFrame.Processed.JumpHeld = rawData.JumpHeld;
            currentFrame.Processed.SprintHeld = rawData.SprintHeld;
            currentFrame.Processed.DodgeHeld = rawData.DodgeHeld;
            currentFrame.Processed.CrouchHeld = rawData.CrouchHeld;
            currentFrame.Processed.AttackHeld = rawData.AttackHeld;
            #endregion

            #region 按键Buffer处理
            // 核心机制：按一下 → BufferTimer = 0.2s → 倒计时结束前都算"按下"
            var lastProc = inputData.LastFrame.Processed;
            currentFrame.Processed.JumpBufferTimer = UpdateBuffer(lastProc.JumpBufferTimer, rawData.JumpJustPressed);
            currentFrame.Processed.SprintBufferTimer = UpdateBuffer(lastProc.SprintBufferTimer, rawData.SprintJustPressed);
            currentFrame.Processed.DodgeBufferTimer = UpdateBuffer(lastProc.DodgeBufferTimer, rawData.DodgeJustPressed);
            currentFrame.Processed.CrouchBufferTimer = UpdateBuffer(lastProc.CrouchBufferTimer, rawData.CrouchJustPressed);
            currentFrame.Processed.AttackBufferTimer = UpdateBuffer(lastProc.AttackBufferTimer, rawData.AttackJustPressed);
            #endregion

            // 保存当前帧
            inputData.CurrentFrame = currentFrame;
        }

        /// <summary>
        /// 更新Buffer计时器
        /// </summary>
        /// <param name="lastTimer">上一帧的计时器值</param>
        /// <param name="justPressed">当前帧是否刚按下</param>
        /// <returns>新的计时器值</returns>
        private float UpdateBuffer(float lastTimer, bool justPressed)
        {
            // 1. 倒计时：减去本帧流逝的时间，最小为0
            float newTimer = Mathf.Max(0f, lastTimer - Time.deltaTime);

            // 2. 如果刚按下：重置为Buffer时间
            if (justPressed) newTimer = actionBufferTime;

            return newTimer;
        }
        #endregion

        #region Consume方法
        /// <summary>
        /// 消耗跳跃输入（执行跳跃后调用，防止重复触发）
        /// </summary>
        public void ConsumeJump() {inputData.CurrentFrame.Processed.JumpBufferTimer = 0f;}

        /// <summary>
        /// 消耗冲刺输入
        /// </summary>
        public void ConsumeSprint() { var f = inputData.CurrentFrame; f.Processed.SprintBufferTimer = 0f; inputData.CurrentFrame = f; }

        /// <summary>
        /// 消耗闪避输入
        /// </summary>
        public void ConsumeDodge() { var f = inputData.CurrentFrame; f.Processed.DodgeBufferTimer = 0f; inputData.CurrentFrame = f; }

        /// <summary>
        /// 消耗蹲下输入
        /// </summary>
        public void ConsumeCrouch() { var f = inputData.CurrentFrame; f.Processed.CrouchBufferTimer = 0f; inputData.CurrentFrame = f; }

        /// <summary>
        /// 消耗攻击输入
        /// </summary>
        public void ConsumeAttack() { var f = inputData.CurrentFrame; f.Processed.AttackBufferTimer = 0f; inputData.CurrentFrame = f; }
        #endregion
    }
}