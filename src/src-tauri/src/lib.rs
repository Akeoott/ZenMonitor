// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

mod commands;
mod sidecar;
mod state;

use std::sync::Mutex;
use tauri::{AppHandle, Manager};
use sidecar::spawn_sidecar;
use state::SidecarState;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .setup(|app| {
            let app_handle = app.handle().clone();
            let is_dev = cfg!(debug_assertions);
            initialize_sidecar(app_handle, is_dev);
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![commands::get_backend_url])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}

/// Starts the sidecar (or falls back to a fixed port in dev mode)
/// and manages the state in the Tauri app.
fn initialize_sidecar(app_handle: AppHandle, is_dev: bool) {
    tauri::async_runtime::spawn(async move {
        let result = match spawn_sidecar().await {
            Ok((port, child, forward_handle)) => {
                eprintln!("[rust] backend started on port {}", port);
                Ok(SidecarState {
                    process: Mutex::new(Some(child)),
                    port,
                    _forward_task: Some(forward_handle),
                })
            }
            Err(e) => {
                eprintln!("[rust] failed to start backend: {}", e);
                if is_dev {
                    eprintln!("[rust] dev mode: falling back to fixed port 5000");
                    Ok(SidecarState {
                        process: Mutex::new(None),
                        port: 5000,
                        _forward_task: None,
                    })
                } else {
                    Err(())
                }
            }
        };

        if let Ok(state) = result {
            app_handle.manage(state);
        }
    });
}