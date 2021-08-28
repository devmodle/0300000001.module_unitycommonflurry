using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if FLURRY_MODULE_ENABLE
using FlurrySDK;

//! 플러리 관리자
public partial class CFlurryManager : CSingleton<CFlurryManager> {
	//! 매개 변수
	public struct STParams {
		public string m_oAPIKey;
	}
	
	#region 변수
	private STParams m_stParams;
	private System.Action<CFlurryManager, bool> m_oInitCallback = null;
	#endregion			// 변수

	#region 프로퍼티
	public bool IsInit { get; private set; } = false;
	#endregion			// 프로퍼티

	#region 함수
	//! 초기화
	public virtual void Init(STParams a_stParams, System.Action<CFlurryManager, bool> a_oCallback) {
		CAccess.Assert(a_stParams.m_oAPIKey.ExIsValid());
		CFunc.ShowLog($"CFlurryManager.Init: {a_stParams.m_oAPIKey}", KCDefine.B_LOG_COLOR_PLUGIN);

#if UNITY_IOS || UNITY_ANDROID
		// 초기화 되었을 경우
		if(this.IsInit) {
			a_oCallback?.Invoke(this, true);
		} else {
			m_stParams = a_stParams;
			m_oInitCallback = a_oCallback;

			var oBuilder = new Flurry.Builder();
			oBuilder.WithMessaging(false);
			oBuilder.WithLogLevel(Flurry.LogLevel.VERBOSE);
			oBuilder.WithContinueSessionMillis(KCDefine.U_TIMEOUT_FLURRY_M_NETWORK_CONNECTION);
			oBuilder.WithAppVersion(CProjInfoTable.Inst.ProjInfo.m_stBuildVer.m_oVer);
			oBuilder.WithDataSaleOptOut(!CCommonAppInfoStorage.Inst.AppInfo.IsAgreeTracking);

#if FLURRY_ANALYTICS_ENABLE && (ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))
			oBuilder.WithLogEnabled(true);
			oBuilder.WithCrashReporting(true);
			oBuilder.WithIncludeBackgroundSessionsInMetrics(true);
#else
			oBuilder.WithLogEnabled(false);
			oBuilder.WithCrashReporting(false);
			oBuilder.WithIncludeBackgroundSessionsInMetrics(false);
#endif			// #if FLURRY_ANALYTICS_ENABLE && (ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))

			oBuilder.Build(a_stParams.m_oAPIKey);
			this.ExLateCallFunc((a_oSender, a_oParams) => this.OnInit());
		}
#else
		a_oCallback?.Invoke(this, false);
#endif			// #if UNITY_IOS || UNITY_ANDROID
	}
	#endregion			// 함수

	#region 조건부 함수
#if UNITY_IOS || UNITY_ANDROID
	// 초기화 되었을 경우
	private void OnInit() {
		CScheduleManager.Inst.AddCallback(KCDefine.U_KEY_FLURRY_M_INIT_CALLBACK, () => {
			CFunc.ShowLog("CFlurryManager.OnInit");
			this.IsInit = true;
			
			CFunc.Invoke(ref m_oInitCallback, this, this.IsInit);
		});
	}
#endif			// #if UNITY_IOS || UNITY_ANDROID
	#endregion			// 조건부 함수
}
#endif			// #if FLURRY_MODULE_ENABLE
