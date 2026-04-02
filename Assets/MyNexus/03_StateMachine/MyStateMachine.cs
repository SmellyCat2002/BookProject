namespace MyNexus
{
    /// <summary>
    /// 简单状态机 - 复刻第3步
    /// 只管两件事：当前是谁、怎么切换
    /// </summary>
    public class MyStateMachine
    {
        /// <summary>当前状态</summary>
        public MyBaseState CurrentState { get; private set; }

        /// <summary>初始化状态机</summary>
        public void Initialize(MyBaseState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        /// <summary>切换状态</summary>
        public void ChangeState(MyBaseState newState)
        {
            // 1. 退出当前状态
            CurrentState.Exit();

            // 2. 切换到新状态
            CurrentState = newState;

            // 3. 进入新状态
            CurrentState.Enter();
        }
    }
}