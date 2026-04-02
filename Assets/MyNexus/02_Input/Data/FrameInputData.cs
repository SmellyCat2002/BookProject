namespace MyNexus
{
    /// <summary>
    /// 每帧输入数据
    /// </summary>
    public struct FrameInputData
    {
        #region 帧信息
        public ulong FrameIndex;
        #endregion

        #region 输入数据
        public RawInputData Raw;
        public ProcessedInputData Processed;
        #endregion
    }
}