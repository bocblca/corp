
   


    //function getopenid()
        //{
        //    localStorage.clear();
        //}
        var lat;//精度
        var lon;//纬度
        function pst1()
        {
        // localStorage.clear();
        ui = document.getElementById("m6420");
            ux = document.getElementById("bankno");
            ux.value = "6420";
            ui.submit();
        }
        function pst2() {
        // localStorage.clear();
        ui = document.getElementById("m6420");
            ux = document.getElementById("bankno");
            ux.value = "6501";
            ui.submit();
        }
        function pst3() {
        // localStorage.clear();
        ui = document.getElementById("m6420");
            ux = document.getElementById("bankno");
            ux.value = "6500";
            ui.submit();
        }


        function getopenid() {
        ui = document.getElementById("mopenid");
            ux = document.getElementById("openid");
            
            if (localStorage.openid) {
        // localStorage.openid =
        //  localStorage.clear();
        //alert(localStorage.openid);
        // localStorage.clear();
        ui.value = localStorage.openid;


            }
            else {
                if (ux.value != "0") {

        localStorage.openid = ux.value;
                    // alert("可以存储openid");
                }
                // localStorage.openid ="bocblc"
                // ui.value = "openid不存在";

            }

        }
        function setmap1() {

        wx.getLocation({
            type: 'gcj02', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
            success: function (res) {
                //使用微信内置地图查看位置接口
                lat = res.latitude;
                lon = res.longitude
                wx.openLocation({
                    latitude: 41.576231,//res.latitude, // 纬度，浮点数，范围为90 ~ -90 41.576231,120.455740
                    longitude: 120.455740, //res.longitude, // 经度，浮点数，范围为180 ~ -180。
                    name: '葫芦岛农商银行朝阳分行', // 位置名
                    address: '朝阳市双塔区新华路二段68号', // 地址详情说明
                    scale: 18, // 地图缩放级别,整形值,范围从1~28。默认为最大
                    infoUrl: '' // 在查看位置界面底部显示的超链接,可点击跳转（测试好像不可用）
                });
            },
            cancel: function (res) {

            }
        });

        }

        function setmap2() {

        wx.getLocation({
            type: 'gcj02', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
            success: function (res) {
                //使用微信内置地图查看位置接口
                wx.openLocation({
                    latitude: 41.567441,//res.latitude, // 纬度，浮点数，范围为90 ~ -90 41.567441,120.450069
                    longitude: 120.450069, //res.longitude, // 经度，浮点数，范围为180 ~ -180。
                    name: 'HLDRCB朝阳文化路支行', // 位置名
                    address: '辽宁省朝阳市双塔区文化路二段125号', // 地址详情说明
                    scale: 18, // 地图缩放级别,整形值,范围从1~28。默认为最大
                    infoUrl: '' // 在查看位置界面底部显示的超链接,可点击跳转（测试好像不可用）
                });
            },
            cancel: function (res) {

            }
        });

        }

        function setmap3() {

        wx.getLocation({
            type: 'gcj02', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
            success: function (res) {
                //使用微信内置地图查看位置接口
                wx.openLocation({
                    latitude: 41.243762,//res.latitude, // 纬度，浮点数，范围为90 ~ -90 41.243762,119.409004
                    longitude: 119.409004, //res.longitude, // 经度，浮点数，范围为180 ~ -180。
                    name: 'HLDRCB朝阳凌源支行', // 位置名
                    address: '辽宁省朝阳市凌源市市府路东段43号', // 地址详情说明
                    scale: 18, // 地图缩放级别,整形值,范围从1~28。默认为最大
                    infoUrl: '' // 在查看位置界面底部显示的超链接,可点击跳转（测试好像不可用）
                });
            },
            cancel: function (res) {

            }
        });

        }
   


  