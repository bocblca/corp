// See https://aka.ms/new-console-template for more information
using Wxsdk;



Console.WriteLine(Wxcorpsdk.GetOsform());

if (Wxcorpsdk.Iswindows())
{
    Console.WriteLine("windows平台");

}
else {
    Console.WriteLine("非windwos平台");
}
if (Wxcorpsdk.Islinux())
{
    Console.WriteLine("Linux平台");

}
else
{
    Console.WriteLine("非Linux平台");
}

var sdk = Wxcorpsdk.NewSdk();
var result = Wxcorpsdk.Init(sdk, "wwa32481e26ed5a1b1", "AELp7hzny2kBz4J0WT6TY9AZ3nTGwnVAcoobdmHEmyQ");

if (result == 0)
{
    Console.WriteLine("wcsdkdll调用成功！");
}
else
{
    Console.WriteLine("wcsdkdll调用失败！");
}
