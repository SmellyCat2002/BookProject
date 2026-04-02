using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 主控器 - 复刻BBBNexus
    /// MVP：框架完整，状态能跑
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class MyCharacterController : MonoBehaviour
    {
        [Header("基础引用")]
        public Transform PlayerCamera;

        // 组件引用
        private CharacterController _charController;
        private Animator _animator;

        // ============================================
        // 第2步：运行时数据黑板
        // ============================================
        public MyRuntimeData RuntimeData { get; private set; }

        // ============================================
        // 第3步：状态机
        // ============================================
        public MyStateMachine StateMachine { get; private set; }

        // ============================================
        // 第5步：输入管道
        // ============================================
        public MyInputPipeline InputPipeline { get; private set; }

        // ============================================
        // 第6步：仲裁层
        // ============================================
        public SimpleArbiter Arbiter { get; private set; }

        // 启动标志
        private bool _booted;

        private void Awake()
        {
            // 第1步：获取组件
            _charController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();

            // 第2步：实例化数据黑板
            RuntimeData = new MyRuntimeData(this);

            // 第3步：实例化状态机（暂时不初始化，等Boot时初始化）
            StateMachine = new MyStateMachine();

            // 第5步：实例化输入管道
            InputPipeline = new MyInputPipeline(new PlayerInputSource());

            // 第6步：实例化仲裁器
            Arbiter = new SimpleArbiter();

            Debug.Log("[MyNexus] Awake - 组件 + 黑板 + 状态机 + 输入管道 + 仲裁器 创建完成");
        }

        private void Start()
        {
            BootIfNeeded();
        }

        private void BootIfNeeded()
        {
            if (_booted) return;

            // 初始化相机
            if (PlayerCamera == null && Camera.main != null)
                PlayerCamera = Camera.main.transform;
            RuntimeData.CameraTransform = PlayerCamera;

            // 第3步：初始化状态机，设为Idle状态
            StateMachine.Initialize(new MyIdleState(this));

            _booted = true;
            Debug.Log("[MyNexus] 启动完成！状态机已就绪");
        }

        /// <summary>
        /// 核心：单入口Update！
        /// </summary>
        private void Update()
        {
            if (!_booted) return;

            // 第5步：输入管道更新 + 读取处理后数据
            InputPipeline.Update();
            var processed = InputPipeline.CurrentProcessed;
            RuntimeData.MoveInput = processed.Move;
            RuntimeData.LookInput = processed.Look;
            RuntimeData.WantsToJump = processed.JumpPressed;
            RuntimeData.WantsToRun = processed.SprintPressed;
            RuntimeData.WantsToDodge = processed.DodgePressed;
            RuntimeData.WantsToCrouch = processed.CrouchPressed;
            RuntimeData.WantsToAttack = processed.AttackPressed;

            // 第6步：仲裁器更新
            Arbiter.Update(Time.deltaTime);

            // 第3步：状态机逻辑更新
            StateMachine.CurrentState.LogicUpdate();
        }

        /// <summary>
        /// 物理与表现更新
        /// </summary>
        private void LateUpdate()
        {
            if (!_booted) return;

            // 第4步：地面检测
            CheckGround();

            // 第3步：状态机物理更新
            StateMachine.CurrentState.PhysicsUpdate();

            // 每帧结束时清理意图！
            RuntimeData.ResetIntent();
        }

        /// <summary>
        /// 地面检测 - 临时版本
        /// </summary>
        private void CheckGround()
        {
            // 使用 CharacterController 的 isGrounded
            RuntimeData.IsGrounded = _charController.isGrounded;
        }
    }
}
