﻿//视图层，新手引导模块，触发虚拟按键引导 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Kernel;
using Global;

namespace View {
	public class TriggerOperVitualKey : MonoBehaviour, IGuideTrigger {
		public static TriggerOperVitualKey Instance;      //本类实例
		public GameObject GoBackground;     //背景游戏对象（对话界面）
		private bool _IsExistNextDIalogsRecorder = false;       //是否存在下一条对话记录
		private Image _Img_GuideVK;       //引导ET贴图

		void Awake() {
			Instance = this;        //得到本类实例
		}

		void Start() {
			//得到背景贴图
			_Img_GuideVK = transform.parent.Find("Img_VK").GetComponent<Image>();
			//注册“背景贴图”
			RegisterGuideVK();
		}

		/// <summary>
		/// 检查触发条件
		/// </summary>
		/// <returns>
		/// true: 表示条件成立，触发后续业务逻辑
		/// </returns>
		public bool CheckCondition() {
			Log.Write(GetType() + "/CheckCondition()");
			if (_IsExistNextDIalogsRecorder) {
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// 运行业务逻辑
		/// </summary>
		/// <returns>
		/// true: 表示业务逻辑执行完毕
		/// </returns>
		public bool RunOperation() {
			Log.Write(GetType() + "/RunOperation()");
			//这个方法每次检查时只执行一次
			_IsExistNextDIalogsRecorder = false;

			//隐藏对话界面
			GoBackground.SetActive(false);
			//隐藏“引导VK贴图”
			_Img_GuideVK.gameObject.SetActive(false);
			//激活ET
			View_PlayerInfoResponse.Instance.DisplayET();
			//激活虚拟按键（普通攻击）
			View_PlayerInfoResponse.Instance.DisplayVK();
			//恢复对话系统，继续会话
			StartCoroutine(ResumeDialogs());
			return false;
		}

		/// <summary>
		/// 显示“引导虚拟按键贴图”
		/// </summary>
		public void DisplayGuideVK() {
			_Img_GuideVK.gameObject.SetActive(true);
		}



		//注册“引导虚拟按键贴图”
		private void RegisterGuideVK() {
			if (_Img_GuideVK != null) {
				EventTriggerListener.Get(_Img_GuideVK.gameObject).onClick += GuideVKOperation;
			}
		}

		/// <summary>
		/// 引导虚拟按键操作
		/// </summary>
		/// <param name="go">注册的游戏对象</param>
		private void GuideVKOperation(GameObject go) {
			if (go == _Img_GuideVK.gameObject) {
				_IsExistNextDIalogsRecorder = true;
			}
		}

		/// <summary>
		/// 恢复对话系统，继续会话
		/// </summary>
		IEnumerator ResumeDialogs() {
			//给玩家5s的时间来自由移动
			yield return new WaitForSeconds(5f);    
			//隐藏ET
			View_PlayerInfoResponse.Instance.HideET();
			//隐藏虚拟按键（普通攻击）
			View_PlayerInfoResponse.Instance.HideVK();
			//注册会话系统，允许继续会话
			TriggerDialogs.Instance.RegisterDialogs();
			//运行对话系统，直接显示下一条对话
			TriggerDialogs.Instance.RunOperation();
			//使用这个方法也可以
			//TriggerDialogs.Instance.DisplayNextDialogRecord(_Img_BGDIalogs.gameObject);
			//显示对话界面
			GoBackground.SetActive(true);
		}
	}
}
