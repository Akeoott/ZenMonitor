// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

use std::sync::Mutex;
use tokio::task::JoinHandle;
use tokio::sync::OnceCell;
use tauri_plugin_shell::process::CommandChild;

pub struct SidecarInner {
    pub _process: Mutex<Option<CommandChild>>,
    pub port: u16,
    pub _forward_task: Option<JoinHandle<()>>,
}

pub struct SidecarState {
    pub inner: OnceCell<SidecarInner>,
}