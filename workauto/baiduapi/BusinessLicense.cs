using Flurl;
using Flurl.Http;
namespace workapi.baiduapi
{
    public class BusinessLicense
    {

        public static async Task<string> LoanInfoAsync(string token, string Sfile64)
        {
            string mhost = "https://aip.baidubce.com";


            var result = await mhost
                .AppendPathSegment("rest/2.0/solution/v1/iocr/recognise")
                .SetQueryParam("access_token", token)
                .PostUrlEncodedAsync(new
                {

                    image = Sfile64,
                    //templateSign= "bf59f406fb2d4876e399152f6b0920c9"
                    classifierId = "1"
                }).ReceiveString();





            return result;

        }
        public static async Task<string> CardInfo(string token, string Sfile64)
        {

            string mhost = "https://aip.baidubce.com";


            var result = await mhost
                .AppendPathSegment("rest/2.0/ocr/v1/business_license")
                .SetQueryParam("access_token", token)
                .PostUrlEncodedAsync(new
                {

                    image = Sfile64

                }).ReceiveString();





            return result;
        }

        public static String getFileBase64(String fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Open);
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string baser64 = Convert.ToBase64String(arr);
            filestream.Close();
            return baser64;
        }
    }
}
