using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FLURRY_ENABLE
using FlurrySDK;

//! 플러리 관리자
public class CFlurryManager : CSingleton<CFlurryManager> {
	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티

	#region 함수
	//! 초기화
	public virtual void Init(string a_oAPIKey, System.Action<CFlurryManager, bool> a_oCallback) {
		CFunc.ShowLog("CFlurryManager.Init: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oAPIKey);

		if(!this.IsInit && CAccess.IsMobilePlatform()) {
			CAccess.Assert(a_oAPIKey.ExIsValid());

			this.IsInit = true;
			var oBuilder = new Flurry.Builder();

#if FLURRY_ANALYTICS_ENABLE
#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
			oBuilder.WithLogEnabled(true);
			oBuilder.WithCrashReporting(true);
			oBuilder.WithIncludeBackgroundSessionsInMetrics(true);
#else
			oBuilder.WithLogEnabled(false);
			oBuilder.WithCrashReporting(false);
			oBuilder.WithIncludeBackgroundSessionsInMetrics(false);
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)

			oBuilder.WithMessaging(false);
			oBuilder.WithLogLevel(Flurry.LogLevel.VERBOSE);
			oBuilder.WithAppVersion(CProjInfoTable.Instance.ProjInfo.m_oBuildVersion);
			oBuilder.WithContinueSessionMillis((long)(KCDefine.U_TIMEOUT_FLURRY_NETWORK_CONNECTION * 1000.0f));
#endif			// #if FLURRY_ANALYTICS_ENABLE

#if MSG_PACK_ENABLE
			oBuilder.WithDataSaleOptOut(CUserInfoStorage.Instance.UserInfo.IsAgree);
#else
			oBuilder.WithDataSaleOptOut(false);
#endif			// #if MSG_PACK_ENABLE

			oBuilder.Build(a_oAPIKey);
		}

		a_oCallback?.Invoke(this, this.IsInit);
	}
	#endregion			// 함수

	#region 조건부 함수
#if FLURRY_ANALYTICS_ENABLE
	//! 유저 식별자를 변경한다
	public void SetUserID(string a_oID) {
		CAccess.Assert(a_oID.ExIsValid());
		CFunc.ShowLog("CFlurryManager.SetUserID: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oID);

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
			[a_oParam] = a_oDataList.ExToString(KCDefine.U_TOKEN_FLURRY_ANALYTICS_LOG_DATA)
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
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_DEVICE_ID, CAppInfoStorage.Instance.AppInfo.DeviceID);

#if AUTO_LOG_PARAM_ENABLE
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_PLATFORM, CAppInfoStorage.Instance.PlatformName);
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_USER_TYPE, CUserInfoStorage.Instance.UserInfo.UserType.ToString());
			
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_LOG_TIME, System.DateTime.UtcNow.ExToLongString());
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_INSTALL_TIME, CAppInfoStorage.Instance.AppInfo.UTCInstallTime.ExToLongString());
#endif			// #if AUTO_LOG_PARAM_ENABLE
#endif			// #if MSG_PACK_ENABLE

			Flurry.LogEvent(a_oName, oDataList);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
	}
#endif			// #if FLURRY_ANALYTICS_ENABLE
	#endregion			// 조건부 함수
}
#endif			// #if FLURRY_ENABLE
