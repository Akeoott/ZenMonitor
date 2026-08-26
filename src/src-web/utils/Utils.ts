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