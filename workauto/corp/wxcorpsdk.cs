using System.Runtime.InteropServices;
namespace workauto.corpsdk
{
    public class Wxcorpsdk
    {

        private const string DllName = "libWeWorkFinanceSdk_C.so";
        

        [DllImport(DllName)] public static extern long NewSdk();
        /**
         * 初始化函数
         * Return值=0表示该API调用成功
         * 
         * @param [in]  sdk			NewSdk返回的sdk指针
         * @param [in]  corpId      调用企业的企业id，例如：wwd08c8exxxx5ab44d，可以在企业微信管理端--我的企业--企业信息查看
         * @param [in]  secret		会话内容存档的Secret，可以在企业微信管理端--管理工具--会话内容存档查看
         *						
         *
         * @return 返回是否初始化成功
         *      ==0 - 成功
         *      !=0 - 失败
         */
        [DllImport(DllName)] public static extern int Init(long sdk, string corpId, string secret);

    }
}
