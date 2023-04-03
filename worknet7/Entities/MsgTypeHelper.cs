﻿#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2023 Jeffrey Su & Suzhou Senparc Network Technology Co.,Ltd.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.

Detail: https://github.com/JeffreySu/WeiXinMPSDK/blob/master/license.md

----------------------------------------------------------------*/
#endregion Apache License Version 2.0

/*----------------------------------------------------------------
    Copyright (C) 2023 Senparc
    
    文件名：MsgTypeHelper.cs
    文件功能描述：根据xml信息返回MsgType、ThirdPartyInfo、RequestInfoType
    
    
    创建标识：Senparc - 20150313
    
    修改标识：Senparc - 20150313
    修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Xml.Linq;
using Senparc.Weixin.Work.Entities.Request.Event;

namespace Senparc.Weixin.Work.Helpers
{
    public static class MsgTypeHelper
    {


        #region ThirdPartyInfo
        /// <summary>
        /// 根据xml信息，返回ThirdPartyInfo
        /// </summary>
        /// <returns></returns>
        public static ThirdPartyInfo GetThirdPartyInfo(XDocument doc)
        {
            return GetThirdPartyInfo(doc.Root.Element("InfoType").Value);
        }
        /// <summary>
        /// 根据xml信息，返回RequestInfoType
        /// </summary>
        /// <returns></returns>
        public static ThirdPartyInfo GetThirdPartyInfo(string str)
        {
            return (ThirdPartyInfo)Enum.Parse(typeof(ThirdPartyInfo), str, true);
        }

        #endregion
        
        #region ExternalContactChangeType

        /// <summary>
        /// 根据xml信息，返回ExternalContactChangeType
        /// </summary>
        /// <returns></returns>
        public static ExternalContactChangeType GetExternalContactChangeType(XDocument doc)
        {
            return GetExternalContactChangeType(doc.Root.Element("ChangeType").Value);
        }
        /// <summary>
        /// 根据xml信息，返回ExternalContactChangeType
        /// </summary>
        /// <returns></returns>
        public static ExternalContactChangeType GetExternalContactChangeType(string str)
        {
            return (ExternalContactChangeType)Enum.Parse(typeof(ExternalContactChangeType), str, true);
        }

        #endregion

    }
}