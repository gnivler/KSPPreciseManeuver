/*******************************************************************************
 * Copyright (c) 2016, George Sedov
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace KSPPreciseManeuver.UI {
[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour {
  private CanvasGroup m_CanvasGroup;
  private IEnumerator m_FadeCoroutine;

  private float m_FastFadeDuration = 0.2f;
  private float m_SlowFadeDuration = 1.0f;

  private bool m_IsFadingIn;

  public bool IsFadingIn {
    get { return m_FadeCoroutine != null && m_IsFadingIn; }
  }

  public bool IsFadingOut {
    get { return m_FadeCoroutine != null && !m_IsFadingIn; }
  }

  public void setTransparent() {
    setAlpha (0.0f);
  }

  public void fadeIn () {
    m_IsFadingIn = true;
    gameObject.SetActive (true);
    FadeTo (1.0f, m_FastFadeDuration);
  }

  public void fadeClose() {
    m_IsFadingIn = false;
    FadeTo(0.0f, m_FastFadeDuration, Destroy);
  }

  public void fadeOut() {
    m_IsFadingIn = false;
    FadeTo(0.0f, m_FastFadeDuration, setInactive);
  }

  private void setInactive () {
    gameObject.SetActive (false);
  }

  public void fadeCloseSlow () {
    m_IsFadingIn = false;
    FadeTo (0.0f, m_SlowFadeDuration, Destroy);
  }

  private void FadeTo(float alpha, float duration, Action callback = null) {
    if (m_CanvasGroup == null)
      return;

    Fade(m_CanvasGroup.alpha, alpha, duration, callback);
  }

  private void setAlpha (float alpha) {
    if (m_CanvasGroup == null)
      return;

    alpha = Mathf.Clamp01 (alpha);
    m_CanvasGroup.alpha = alpha;
  }

  protected virtual void Awake() {
    m_CanvasGroup = GetComponent<CanvasGroup>();
  }

  private void Fade(float from, float to, float duration, Action callback) {
    if (m_FadeCoroutine != null)
      StopCoroutine(m_FadeCoroutine);

    if (Math.Abs(from - to) < 0.1) {
      setAlpha(to);
    } else {
      m_FadeCoroutine = FadeCoroutine(from, to, duration, callback);
      StartCoroutine(m_FadeCoroutine);
    }
  }

  private IEnumerator FadeCoroutine(float from, float to, float duration, Action callback) {
    // wait for end of frame so that only the last call to fade that frame is honoured.
    yield return new WaitForEndOfFrame();

    float progress = 0.0f;

    while (progress <= 1.0f) {
      progress += Time.deltaTime / duration;
      setAlpha(Mathf.Lerp(from, to, progress));
      yield return null;
    }

    callback?.Invoke();

    m_FadeCoroutine = null;
  }

  protected virtual void Destroy() {
    // disable game object first due to an issue within unity 5.2.4f1 that shows a single frame at full opaque alpha just before destruction
    gameObject.SetActive(false);
    Destroy(gameObject);
  }
}
}
