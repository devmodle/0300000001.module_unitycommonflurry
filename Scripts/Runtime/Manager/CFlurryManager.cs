using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FLURRY_MODULE_ENABLE
#if UNITY_IOS || UNITY_ANDROID
using FlurrySDK;
#endif			// #if UNITY_IOS || UNITY_ANDROID

//! 플러리 관리자
public partial class CFlurryManager : CSingleton<CFlurryManager> {
	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티

	#region 함수
	//! 초기화
	public virtual void Init(string a_oAPIKey, System.Action<CFlurryManager, bool> a_oCallback) {
		CFunc.ShowLog("CFlurryManager.Init: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oAPIKey);

#if UNITY_IOS || UNITY_ANDROID
		// 초기화 되었을 경우
		if(this.IsInit) {
			a_oCallback?.Invoke(this, true);
		} else {
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
			oBuilder.WithAppVersion(CProjInfoTable.Instance.ProjInfo.m_stBuildVersion.m_oVersion);
			oBuilder.WithContinueSessionMillis(KCDefine.U_TIMEOUT_FLURRY_NETWORK_CONNECTION);
#endif			// #if FLURRY_ANALYTICS_ENABLE

#if MSG_PACK_ENABLE
			oBuilder.WithDataSaleOptOut(CCommonUserInfoStorage.Instance.UserInfo.IsAgree);
#else
			oBuilder.WithDataSaleOptOut(false);
#endif			// #if MSG_PACK_ENABLE

			oBuilder.Build(a_oAPIKey);

			this.IsInit = true;
			a_oCallback?.Invoke(this, this.IsInit);
		}
#else
		a_oCallback?.Invoke(this, false);
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}
	#endregion			// 함수
}
#endif			// #if FLURRY_MODULE_ENABLE
