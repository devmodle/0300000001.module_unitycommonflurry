using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FLURRY_MODULE_ENABLE
using FlurrySDK;

//! 플러리 관리자
public partial class CFlurryManager : CSingleton<CFlurryManager> {
	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티

	#region 함수
	//! 초기화
	public virtual void Init(string a_oAPIKey, System.Action<CFlurryManager, bool> a_oCallback) {
		CFunc.ShowLog("CFlurryManager.Init: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oAPIKey);

		// 초기화 가능 할 경우
		if(!this.IsInit && CAccess.IsMobilePlatform()) {
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
			oBuilder.WithDataSaleOptOut(CCommonUserInfoStorage.Instance.UserInfo.IsAgree);
#else
			oBuilder.WithDataSaleOptOut(false);
#endif			// #if MSG_PACK_ENABLE

			oBuilder.Build(a_oAPIKey);
		}

		a_oCallback?.Invoke(this, this.IsInit);
	}
	#endregion			// 함수
}
#endif			// #if FLURRY_MODULE_ENABLE
