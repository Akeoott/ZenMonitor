// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

use std::sync::Mutex;
use tokio::process::Child;
use tokio::task::JoinHandle;

/// Holds the state of the sidecar backend.
pub struct SidecarState {
    /// The child process, if still running.
    pub process: Mutex<Option<Child>>,
    /// The port on which the backend is (or should be) listening.
    pub port: u16,
    /// The task that forwards stdout to the parent process.
    pub _forward_task: Option<JoinHandle<()>>,
}