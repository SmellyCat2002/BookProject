using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 主控器 - 复刻BBBNexus
    /// 第3步：集成状态机
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

            Debug.Log("[MyNexus] Awake - 组件 + 黑板 + 状态机 创建完成");
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

            // 第4步：读取输入（临时放在这里，后面会拆到输入系统）
            ReadInput();

            // 第3步：状态机逻辑更新
            StateMachine.CurrentState.LogicUpdate();
        }

        /// <summary>
        /// 读取输入 - 临时版本，后面会移到InputPipeline
        /// </summary>
        private void ReadInput()
        {
            // 读取摇杆/键盘
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            RuntimeData.MoveInput = new Vector2(h, v).normalized;

            // Shift 奔跑
            RuntimeData.WantsToRun = Input.GetKey(KeyCode.LeftShift);
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
