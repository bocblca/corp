using Flurl;
using Flurl.Http;
namespace workapi.baiduapi
{
    public class Idcard
    {




        public static async Task<string> CardInfo(string token, string filebase64, string frontback)
        {



            string mhost = "https://aip.baidubce.com";


            var result = await mhost
                .AppendPathSegment("rest/2.0/ocr/v1/idcard")
                .SetQueryParam("access_token", token)
                .PostUrlEncodedAsync(new
                {
                    id_card_side = frontback,
                    image = filebase64

                }).ReceiveString();





            return result;
        }


    }
}
