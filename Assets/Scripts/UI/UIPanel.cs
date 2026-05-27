using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssalDive.UI {
    /// <summary>
    /// UI 面板基类
    /// </summary>
    public abstract class UIPanel : MonoBehaviour {
        [Header("Panel Settings")]
        public bool animateOnShow = true;
        public float animationDuration = 0.3f;

        protected bool _isVisible;
        public bool IsVisible => _isVisible;

        /// <summary>
        /// 显示面板
        /// </summary>
        public virtual void Show() {
            if (_isVisible) return;
            _isVisible = true;
            gameObject.SetActive(true);
            OnShow();
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public virtual void Hide() {
            if (!_isVisible) return;
            _isVisible = false;
            OnHide();
        }

        /// <summary>
        /// 显示时回调
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// 隐藏时回调
        /// </summary>
        protected virtual void OnHide() { }

        /// <summary>
        /// 每帧更新
        /// </summary>
        public virtual void UpdatePanel() { }
    }
}