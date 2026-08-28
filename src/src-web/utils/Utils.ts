import { isTauri  } from '@tauri-apps/api/core'

export const isRunningInTauri = typeof isTauri === 'function'
  ? isTauri()
  : typeof window !== 'undefined' && !!(window as any).__TAURI_INTERNALS__;

export function withTimeout<T>(promise: Promise<T>, ms: number, errorMsg: string): Promise<T> {
  return new Promise((resolve, reject) => {
    const timer = setTimeout(() => reject(new Error(errorMsg)), ms);
    promise
      .then((res) => {
        clearTimeout(timer);
        resolve(res);
      })
      .catch((err) => {
        clearTimeout(timer);
        reject(err);
      });
  });
}

export function setPageLoader(shouldShow: boolean, message: string = "") {
  requestAnimationFrame(() => {
    const loader = document.getElementById("page-loader");
    const loaderText = document.getElementById("page-loader-text");

    if (loader && loaderText) {
      if (shouldShow) {
        loader.classList.remove("hidden");
        loaderText.textContent = message;
      }
      else {
        loader.classList.add("hidden");
      }
    }
    else {
      console.error("Failed to load page loader");
      return;
    }
  });
}
