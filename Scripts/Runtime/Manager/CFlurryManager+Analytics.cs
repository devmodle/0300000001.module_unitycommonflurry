using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		CFunc.ShowLog("CFlurryManager.SetAnalyticsUserID: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oID);

#if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
		// 초기화 되었을 경우
		if(this.IsInit) {
			Flurry.SetUserId(a_oID);
		}
#endif			// #if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}

	//! 로그를 전송한다
	public void SendLog(string a_oName, Dictionary<string, string> a_oDataList) {
		CAccess.Assert(a_oName.ExIsValid());
		CFunc.ShowLog("CFlurryManager.SendLog: {0}, {1}", KCDefine.B_LOG_COLOR_PLUGIN, a_oName, a_oDataList);
				
#if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			var oDataList = a_oDataList ?? new Dictionary<string, string>();

			oDataList.ExAddValue(KCDefine.U_LOG_KEY_DEVICE_ID, CCommonAppInfoStorage.Inst.AppInfo.DeviceID);
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_PLATFORM, CCommonAppInfoStorage.Inst.Platform);

#if AUTO_LOG_PARAMS_ENABLE
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_USER_TYPE, CCommonUserInfoStorage.Inst.UserInfo.UserType.ToString());
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_LOG_TIME, System.DateTime.UtcNow.ExToLongStr());
			oDataList.ExAddValue(KCDefine.U_LOG_KEY_INSTALL_TIME, CCommonAppInfoStorage.Inst.AppInfo.UTCInstallTime.ExToLongStr());
#endif			// #if AUTO_LOG_PARAMS_ENABLE

			Flurry.LogEvent(a_oName, oDataList);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
#endif			// #if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}
	#endregion			// 함수

	#region 조건부 함수
#if PURCHASE_MODULE_ENABLE
	//! 결제 로그를 전송한다
	public void SendPurchaseLog(Product a_oProduct, int a_nNumProducts) {
		CAccess.Assert(a_oProduct != null && a_nNumProducts > KCDefine.B_VALUE_0_INT);
		CFunc.ShowLog("CFlurryManager.SendPurchaseLog: {0}", KCDefine.B_LOG_COLOR_PLUGIN, a_oProduct);

#if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
#if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
		// 초기화 되었을 경우
		if(this.IsInit) {
			string oID = a_oProduct.definition.id;
			string oName = a_oProduct.metadata.localizedTitle;
			string oCurrencyCode = a_oProduct.metadata.isoCurrencyCode;
			string oTransactionID = a_oProduct.transactionID;

			double dblPrice = decimal.ToDouble(a_oProduct.metadata.localizedPrice);
			Flurry.LogPayment(oName, oID, a_nNumProducts, dblPrice, oCurrencyCode, oTransaction, null);
		}
#endif			// #if ANALYTICS_TEST_ENABLE || (ADHOC_BUILD || STORE_BUILD)
#endif			// #if FLURRY_ANALYTICS_ENABLE && (UNITY_IOS || UNITY_ANDROID)
	}
#endif			// #if PURCHASE_MODULE_ENABLE
	#endregion			// 조건부 함수
}
#endif			// #if FLURRY_MODULE_ENABLE
