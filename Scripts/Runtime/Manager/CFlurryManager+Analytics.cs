using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FLURRY_ENABLE && FLURRY_ANALYTICS_ENABLE
using FlurrySDK;

//! 플러리 관리자
public partial class CFlurryManager : CSingleton<CFlurryManager> {
	#region 함수
	//! 분석 유저 식별자를 변경한다
	public void SetAnalyticsUserID(string a_oID) {
		CAccess.Assert(a_oID.ExIsValid());
		CFunc.ShowLog("CFlurryManager.SetAnalyticsUserID: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oID);

		if(this.IsInit) {
			Flurry.SetUserId(a_oID);
		}
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName) {
		this.SendLog(a_oName, null);
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, string a_oParam, List<string> a_oDataList) {
		CAccess.Assert(a_oParam.ExIsValid());

		this.SendLog(a_oName, new Dictionary<string, string>() {
			[a_oParam] = a_oDataList.ExToString(KCDefine.B_TOKEN_CSV_STRING)
		});
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, Dictionary<string, string> a_oDataList) {
		CAccess.Assert(a_oName.ExIsValid());
		CFunc.ShowLog("CFlurryManager.SendLog: {0}, {1}", KCDefine.B_LOG_COLOR_PLUGIN, a_oName, a_oDataList);

#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
		if(this.IsInit) {
			var oDataList = a_oDataList ?? new Dictionary<string, string>();

#if MSG_PACK_ENABLE
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_DEVICE_ID, CCommonAppInfoStorage.Instance.AppInfo.DeviceID);

#if AUTO_LOG_PARAM_ENABLE
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_PLATFORM, CCommonAppInfoStorage.Instance.PlatformName);
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_USER_TYPE, CCommonUserInfoStorage.Instance.UserInfo.UserType.ToString());
			
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_LOG_TIME, System.DateTime.UtcNow.ExToLongString());
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_INSTALL_TIME, CCommonAppInfoStorage.Instance.AppInfo.UTCInstallTime.ExToLongString());
#endif			// #if AUTO_LOG_PARAM_ENABLE
#endif			// #if MSG_PACK_ENABLE

			Flurry.LogEvent(a_oName, oDataList);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
	}
	#endregion			// 함수
}
#endif			// #if FLURRY_ENABLE && FLURRY_ANALYTICS_ENABLE
