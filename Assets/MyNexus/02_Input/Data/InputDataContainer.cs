namespace MyNexus
{
    /// <summary>
    /// 输入数据容器 - 保存当前帧和上一帧
    /// </summary>
    public class InputDataContainer
    {
        #region 帧数据
        public FrameInputData CurrentFrame;
        public FrameInputData LastFrame;
        #endregion
    }
}