using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if FLURRY_MODULE_ENABLE
using FlurrySDK;

#if PURCHASE_MODULE_ENABLE
using UnityEngine.Purchasing;
#endif			// #if PURCHASE_MODULE_ENABLE

//! 플러리 관리자
public partial class CFlurryManager : CSingleton<CFlurryManager> {
	#region 함수
	//! 분석 유저 식별자를 변경한다
	public void SetAnalyticsUserID(string a_oID) {
		CAccess.Assert(a_oID.ExIsValid());
		CFunc.ShowLog($"CFlurryManager.SetAnalyticsUserID: {a_oID}", KCDefine.B_LOG_COLOR_PLUGIN);

#if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
		// 초기화 되었을 경우
		if(this.IsInit) {
			Flurry.SetUserId(a_oID);
		}
#endif			// #if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, Dictionary<string, string> a_oDataDict) {
		CAccess.Assert(a_oName.ExIsValid());
		CFunc.ShowLog($"CFlurryManager.SendLog: {a_oName}, {a_oDataDict}", KCDefine.B_LOG_COLOR_PLUGIN);
				
#if (FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)) && (ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))
		// 초기화 되었을 경우
		if(this.IsInit) {
			var oDataDict = a_oDataDict ?? new Dictionary<string, string>();

			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_DEVICE_ID, CCommonAppInfoStorage.Inst.AppInfo.DeviceID);
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_PLATFORM, CCommonAppInfoStorage.Inst.Platform);

#if AUTO_LOG_PARAMS_ENABLE
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_USER_TYPE, CCommonUserInfoStorage.Inst.UserInfo.UserType.ToString());
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_LOG_TIME, System.DateTime.UtcNow.ExToLongStr());
			oDataDict.ExAddVal(KCDefine.L_LOG_KEY_INSTALL_TIME, CCommonAppInfoStorage.Inst.AppInfo.UTCInstallTime.ExToLongStr());
#endif			// #if AUTO_LOG_PARAMS_ENABLE

			Flurry.LogEvent(a_oName, oDataDict);
		}
#endif			// #if (FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)) && (ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))
	}
	#endregion			// 함수

	#region 조건부 함수
#if PURCHASE_MODULE_ENABLE
	//! 결제 로그를 전송한다
	public void SendPurchaseLog(Product a_oProduct, int a_nNumProducts) {
		CAccess.Assert(a_oProduct != null && a_nNumProducts > KCDefine.B_VAL_0_INT);
		CFunc.ShowLog($"CFlurryManager.SendPurchaseLog: {a_oProduct}, {a_nNumProducts}", KCDefine.B_LOG_COLOR_PLUGIN);

#if (FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)) && (ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))
		// 초기화 되었을 경우
		if(this.IsInit) {
			double dblPrice = decimal.ToDouble(a_oProduct.metadata.localizedPrice);
			Flurry.LogPayment(a_oProduct.metadata.localizedTitle, a_oProduct.definition.id, a_nNumProducts, dblPrice, a_oProduct.metadata.isoCurrencyCode, a_oProduct.transactionID, null);
		}
#endif			// #if (FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)) && (ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD))
	}
#endif			// #if PURCHASE_MODULE_ENABLE
	#endregion			// 조건부 함수
}
#endif			// #if FLURRY_MODULE_ENABLE
