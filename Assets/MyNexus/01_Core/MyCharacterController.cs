using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 主控器 - 复刻BBBNexus
    /// 第2步：集成MyRuntimeData黑板
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
        // 第2步新增：运行时数据黑板
        // ============================================
        public MyRuntimeData RuntimeData { get; private set; }

        // 启动标志
        private bool _booted;

        private void Awake()
        {
            // 第1步：获取组件
            _charController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();

            // 第2步：实例化数据黑板
            RuntimeData = new MyRuntimeData(this);

            Debug.Log("[MyNexus] Awake - 组件获取 + 数据黑板初始化完成");
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

            // 确保RuntimeData也拿到相机引用
            RuntimeData.CameraTransform = PlayerCamera;

            _booted = true;
            Debug.Log("[MyNexus] 启动完成！数据黑板已就绪");
        }

        /// <summary>
        /// 核心：单入口Update！
        /// </summary>
        private void Update()
        {
            if (!_booted) return;

            // 预留位置：将来这里会按顺序调用各个系统
            // 1. ArbiterPipeline.ProcessUpdateArbiters()
            // 2. InputPipeline.Update()
            // 3. MainProcessorPipeline.UpdateIntentProcessors()
            // 4. StateMachine.CurrentState.LogicUpdate()
        }

        /// <summary>
        /// 物理与表现更新
        /// </summary>
        private void LateUpdate()
        {
            if (!_booted) return;

            // 预留位置：将来这里处理物理
            // 1. StateMachine.CurrentState.PhysicsUpdate()

            // 第2步新增：每帧结束时清理意图！
            RuntimeData.ResetIntent();
        }
    }
}
