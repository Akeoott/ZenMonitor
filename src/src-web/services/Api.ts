import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { invoke } from '@tauri-apps/api/core'

import { withTimeout, isRunningInTauri } from "../utils/Utils.ts";

async function getBackendUrl(): Promise<string> {
  const DEFAULT_PORT_URL = 'http://127.0.0.1:5000';
  const MAX_ATTEMPTS = 6;
  const PER_ATTEMPT_TIMEOUT_MS = 500;
  const RETRY_DELAY_MS = 100;

  if (!isRunningInTauri) {
    console.log(`[Api] Tauri not detected. Defaulting to: ${DEFAULT_PORT_URL}`);
    return DEFAULT_PORT_URL;
  }

  console.log('[Api] Tauri detected');

  let lastError: unknown;
  for (let attempt = 0; attempt < MAX_ATTEMPTS; attempt++) {
    try {
      const url = await withTimeout<string>(
          invoke('get_backend_url'),
          PER_ATTEMPT_TIMEOUT_MS,
          `[Api] Sidecar did not announce port within ${PER_ATTEMPT_TIMEOUT_MS / 1000}s (attempt ${attempt + 1}/${MAX_ATTEMPTS})`
      );
      console.log(`[Api] Url received: ${url}`);
      return url;
    } catch (error) {
      lastError = error;
      console.warn(`[Api] Attempt ${attempt + 1}/${MAX_ATTEMPTS} failed: ${error}`);
      if (attempt < MAX_ATTEMPTS - 1) {
        await new Promise(resolve => setTimeout(resolve, RETRY_DELAY_MS));
      }
    }
  }

  console.warn(`[Api] Failed to retrieve url after ${MAX_ATTEMPTS} attempts (${lastError}). Falling back to: ${DEFAULT_PORT_URL}`);
  return DEFAULT_PORT_URL;
}

/**
 * Silently pings the backend host endpoint until it actively responds.
 * Prevents dirty console log errors from uninitialized HTTP stacks.
 */
async function awaitServerAvailability(baseUrl: string, maxAttempts = 10, delayMs = 500): Promise<boolean> {
  console.log("[Api] Pinging Backend...");

  const workerCode = `
    self.onmessage = async (e) => {
      const { url, timeout } = e.data;
      const controller = new AbortController();
      const id = setTimeout(() => controller.abort(), timeout);

      try {
        await fetch(url, { mode: 'no-cors', signal: controller.signal });
        clearTimeout(id);
        self.postMessage(true);
      } catch (err) {
        clearTimeout(id);
        self.postMessage(false);
      }
    };
  `;

  const blob = new Blob([workerCode], { type: "application/javascript" });
  const workerUrl = URL.createObjectURL(blob);
  const worker = new Worker(workerUrl);

  for (let i = 0; i < maxAttempts; i++) {
    const pingUrl = `${baseUrl}/?t=${Date.now()}`;

    const isAlive = await new Promise<boolean>((resolve) => {
      worker.onmessage = (e) => resolve(e.data);
      worker.postMessage({ url: pingUrl, timeout: delayMs - 50 });
    });

    if (isAlive) {
      console.log(`[Api] Backend verified alive`);
      worker.terminate();
      URL.revokeObjectURL(workerUrl);
      return true;
    }

    console.log("[Api] Retrying Ping...");
    await new Promise((resolve) => setTimeout(resolve, delayMs));
  }

  console.error(`[Api] Backend did not respond within timeout limits`);
  worker.terminate();
  URL.revokeObjectURL(workerUrl);
  return false;
}

class SignalrService {
  private connection: HubConnection | null = null;
  private cachedUrl: string | null = null;

  public async connect(): Promise<void> {
    if (this.connection && this.connection.state !== HubConnectionState.Disconnected) {
      return;
    }

    if (!this.cachedUrl) {
      this.cachedUrl = await getBackendUrl();
    }
    const currentUrl = this.cachedUrl;

    if (!await awaitServerAvailability(currentUrl)) {
      return;
    }

    this.connection = new HubConnectionBuilder()
      .withUrl(`${currentUrl}/api`)
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();

    this.connection.onreconnecting((error) => {
      console.warn(`[Api] Connection lost (${error}). Attempting reconnect...`);
    });

    this.connection.onreconnected((connectionId) => {
      console.log(`[Api] Connection restored. ID: ${connectionId}`);
    });

    this.connection.onclose((error) => {
      console.error(`[Api] Connection closed permanently: ${error}`);
    });

    try {
      await this.connection.start();
      console.log(`[Api] Connected successfully to hub: ${currentUrl}/api`);
    } catch (err) {

      console.error('[Api] Sudden error starting socket handler:', err);
      setTimeout(() => this.connect(), 5000);
    }
  }

  public on<T>(methodName: string, callback: (data: T) => void): void {
    if (!this.connection) {
      console.warn(`[Api] Attempted to listen to '${methodName}' before connection initialized.`);
      return;
    }
    this.connection.on(methodName, callback);
  }

  public off(methodName: string): void {
    this.connection?.off(methodName);
  }

  public async invoke<T = any>(methodName: string, ...args: any[]): Promise<T> {
    if (this.connection?.state !== HubConnectionState.Connected) {
      throw new Error(`[Api] Cannot invoke method. Hub is currently: ${this.connection?.state}`);
    }
    return await this.connection.invoke<T>(methodName, ...args);
  }

  public async send(methodName: string, ...args: any[]): Promise<void> {
    if (this.connection?.state !== HubConnectionState.Connected) {
      throw new Error(`[Api] Cannot send data. Hub is currently: ${this.connection?.state}`);
    }
    await this.connection.send(methodName, ...args);
  }
}

export const api = new SignalrService();
