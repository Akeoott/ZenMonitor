// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

use tauri::State;
use crate::state::SidecarState;

#[tauri::command]
pub async fn get_backend_url(state: State<'_, SidecarState>) -> Result<String, String> {
    let inner = state.inner.get().ok_or("Sidecar not initialized")?;
    Ok(format!("http://127.0.0.1:{}", inner.port))
}