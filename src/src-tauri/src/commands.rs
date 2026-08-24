// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

use tauri::State;
use crate::state::SidecarState;

/// Tauri command that returns the backend URL if the sidecar is alive.
/// If the sidecar has exited, returns an error.
#[tauri::command]
pub fn get_backend_url(state: State<SidecarState>) -> Result<String, String> {
    let mut guard = state
        .process
        .lock()
        .map_err(|_| "failed to lock process state".to_string())?;

    if let Some(proc) = guard.as_mut() {
        match proc.try_wait() {
            Ok(None) => Ok(format!("http://127.0.0.1:{}", state.port)),
            Ok(Some(status)) => {
                *guard = None;
                Err(format!("sidecar exited with status: {}", status))
            }
            Err(e) => Err(format!("failed to check sidecar status: {}", e)),
        }
    } else {
        // No process tracked, return the URL anyway (frontend may handle absence).
        Ok(format!("http://127.0.0.1:{}", state.port))
    }
}