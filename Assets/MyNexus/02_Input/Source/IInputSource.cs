using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 输入源接口 - 从硬件获取原始输入
    /// </summary>
    public interface IInputSource
    {
        #region 方法
        /// <summary>
        /// 获取原始输入数据（ref避免GC）
        /// </summary>
        void FetchRawInput(ref RawInputData rawData);
        #endregion
    }
}