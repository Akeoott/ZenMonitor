// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

mod commands;
mod sidecar;
mod state;

use std::sync::Mutex;
use tauri::{AppHandle, Manager};
use sidecar::spawn_sidecar;
use tokio::sync::OnceCell;
use state::{SidecarState, SidecarInner};

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .plugin(tauri_plugin_process::init())
        .plugin(tauri_plugin_shell::init())
        .setup(|app| {
            app.manage(SidecarState { inner: OnceCell::new() });

            let app_handle = app.handle().clone();
            let is_dev = cfg!(debug_assertions);
            initialize_sidecar(app_handle, is_dev);
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![commands::get_backend_url])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}

fn initialize_sidecar(app_handle: AppHandle, is_dev: bool) {
    tauri::async_runtime::spawn(async move {
        let state = app_handle.state::<SidecarState>();

        let inner_result = match spawn_sidecar(&app_handle).await {
            Ok((port, child, forward_handle)) => {
                eprintln!("[rust] backend started on port {}", port);
                Some(SidecarInner {
                    _process: Mutex::new(Some(child)),
                    port,
                    _forward_task: Some(forward_handle),
                })
            }
            Err(e) => {
                eprintln!("[rust] failed to start backend: {}", e);
                if is_dev {
                    eprintln!("[rust] dev mode: falling back to fixed port 5000");
                    Some(SidecarInner {
                        _process: Mutex::new(None),
                        port: 5000,
                        _forward_task: None,
                    })
                } else {
                    None
                }
            }
        };

        if let Some(inner_data) = inner_result {
            let _ = state.inner.set(inner_data);
        }
    });
}