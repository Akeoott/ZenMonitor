// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

use anyhow::{Context, Result, bail};
use tauri::AppHandle;
use tauri_plugin_shell::{ShellExt, process::CommandEvent};
use tauri_plugin_shell::process::CommandChild;
use tokio::task::JoinHandle;
use tokio::sync::oneshot;

pub async fn spawn_sidecar(app_handle: &AppHandle) -> Result<(u16, CommandChild, JoinHandle<()>)> {
    let sidecar = app_handle
        .shell()
        .sidecar("dotnet-backend")
        .context("failed to locate sidecar binary")?;

    // Spawn the sidecar process
    let (mut rx, child) = sidecar
        .spawn()
        .context("failed to spawn sidecar")?;

    eprintln!("[rust] sidecar spawned (PID: {})", child.pid());

    let (port_tx, port_rx) = oneshot::channel();
    let mut tx = Some(port_tx);

    let forward_handle = tokio::spawn(async move {
        while let Some(event) = rx.recv().await {
            match event {
                CommandEvent::Stdout(line_bytes) => {
                    let line = String::from_utf8_lossy(&line_bytes).to_string();
                    let trimmed = line.trim();
                    println!("[dotnet] {}", trimmed);

                    if let Some(port_str) = trimmed.strip_prefix("API_PORT=") {
                        if let Ok(p) = port_str.parse::<u16>() {
                            if let Some(sender) = tx.take() {
                                let _ = sender.send(p);
                            }
                        }
                    }
                }
                CommandEvent::Stderr(line_bytes) => {
                    let line = String::from_utf8_lossy(&line_bytes).to_string();
                    eprintln!("[dotnet stderr] {}", line.trim());
                }
                _ => {}
            }
        }
    });

    let port = match tokio::time::timeout(
        tokio::time::Duration::from_secs(10),
        port_rx
    ).await {
        Ok(Ok(p)) => p,
        Ok(Err(_)) => bail!("port sender dropped without sending"),
        Err(_) => bail!("timeout waiting for API_PORT"),
    };

    eprintln!("[rust] port received: {}", port);

    Ok((port, child, forward_handle))
}