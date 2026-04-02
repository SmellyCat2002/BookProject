using UnityEngine;

namespace MyNexus
{
    /// <summary>
    /// 测试struct字段赋值
    /// 运行后看Console输出
    /// </summary>
    public class StructCopyTest : MonoBehaviour
    {
        public struct InnerStruct
        {
            public int Value;
        }

        public class Container
        {
            public InnerStruct field;
        }

        private void Start()
        {
            Debug.Log("========== Struct字段赋值测试 ===========");

            var container = new Container();
            container.field = new InnerStruct { Value = 100 };

            Debug.Log($"初始值: {container.field.Value}");

            // ==================== 测试: 直接写 ====================
            Debug.Log("\n--- 直接写 container.field.Value = 200 ---");
            container.field.Value = 200;
            Debug.Log($"写完后值: {container.field.Value}");

            Debug.Log("\n========== 结论 ===========");
            if (container.field.Value == 200)
            {
                Debug.Log("✓ 可以直接写，不需要复制！");
                Debug.Log("BBBNexus的写法可能是过度谨慎，或者有其他原因");
            }
            else
            {
                Debug.Log("✗ 直接写无效，需要复制再赋值回去");
                Debug.Log("这是C#行为：struct字段访问返回副本");
            }
        }
    }
}
