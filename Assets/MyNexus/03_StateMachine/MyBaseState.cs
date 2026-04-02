namespace MyNexus
{
    /// <summary>
    /// 状态基类 - 复刻第3步
    /// 定义每个状态必须实现的4个方法
    /// </summary>
    public abstract class MyBaseState
    {
        /// <summary>进入状态时调用（初始化）</summary>
        public abstract void Enter();

        /// <summary>每帧Update时调用（逻辑）</summary>
        public abstract void LogicUpdate();

        /// <summary>每帧LateUpdate时调用（物理）</summary>
        public abstract void PhysicsUpdate();

        /// <summary>退出状态时调用（清理）</summary>
        public abstract void Exit();
    }
}