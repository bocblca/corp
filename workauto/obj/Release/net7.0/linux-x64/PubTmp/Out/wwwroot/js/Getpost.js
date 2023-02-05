const { jsonToHTMLTable, jsonToExcel } = JSONToTable
var mJson;
function getdata() {

    $.ajax({
        url: "GetpostData",
        type: "POST",
        data:
        {
            bankx: localStorage.openid,
        },
        beforeSend: function () {
            $("#cusMsg").text("loading...");
        },
        success: function (result) {
            $("#cusMsg").text("data already loaded");
            mJson = result;

        },
        error: function () {
            $("#cusMsg").text("loading error!");
        }



    });

}

function getjsData() {
    const domCon = document.getElementById('dataList')
    if (domCon.innerText) {
        //$("#cusMsg").text("data is already set...");
        alert("data is already set...");
        return "success";
    }






    $.ajax({
        url: "GetpostData",
        type: "POST",
        data:
        {
            hopenid: localStorage.openid,
        },
        beforeSend: function () {
            $("#cusMsg").text("loading...");
        },
        success: function (result) {

            // $("#cusMsg").text("机构预约业务列表");

            const props = ['姓名', '手机号码', '预约编号', '预约时间', '预约金额']
            //alert(result);
            const data = JSON.parse(result)
            if (data.length == 0) {
                $("#Gdata").text("本机构没有预约业务!");
                $("#loadings").hide();
                return "nodata";
            };
            const dom = jsonToHTMLTable(data, props, { format: 'dom' })
            $("#loadings").hide();
            //domContainer.append(dom);
            domCon.innerHTML = dom.innerHTML;


        },
        error: function () {
            $("#cusMsg").text("loading error!");
            $("#loadings").hide();
        }



    });

    $.ajax({

        url: "Getbanks",
        type: "POST",
        data:
        {
            muserid: localStorage.openid,
        },

        success: function (result) {
            if (result.msg == "nobank") {
                alert("您没有操作权限!")
                domContainer.innerHTML = "";
                WeixinJSBridge.call('closeWindow');
            }
            else {
                $("#cusMsg").text(result.msg);

            }
        },
        error: function () {
            $("#cusMsg").text("loading error!");
            $("#loadings").hide();
        }
    });


}



function getopenid() {
    ui = document.getElementById("mopenid");
    // ux = document.getElementById("openid");


    if (localStorage.openid) {

        ui.value = localStorage.openid;


    }


}
