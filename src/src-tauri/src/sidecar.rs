// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

use std::path::PathBuf;
use anyhow::{Context, Result, bail};
use tokio::io::{AsyncBufReadExt, BufReader, Lines};
use tokio::process::{Child, Command};
use tokio::task::JoinHandle;

/// Returns the target triple for the current build.
fn target_triple() -> &'static str {
    #[cfg(target_os = "windows")]
    {
        "-x86_64-pc-windows-msvc"
    }
    #[cfg(target_os = "linux")]
    {
        "-x86_64-unknown-linux-gnu"
    }
    #[cfg(target_os = "macos")]
    {
        #[cfg(target_arch = "aarch64")]
        {
            "-aarch64-apple-darwin"
        }
        #[cfg(target_arch = "x86_64")]
        {
            "-x86_64-apple-darwin"
        }
    }
}

/// Constructs the base filename of the sidecar binary.
fn sidecar_binary_name() -> String {
    format!("dotnet-backend{}", target_triple())
}

/// Searches for the sidecar binary in a set of candidate directories.
pub fn find_sidecar() -> Result<PathBuf> {
    let base_name = sidecar_binary_name();
    let candidates = build_candidate_paths(&base_name);

    for path in candidates {
        if path.exists() {
            return Ok(path);
        }
    }

    bail!("sidecar binary not found in any candidate location");
}

/// Builds a list of possible absolute paths where the sidecar might reside.
fn build_candidate_paths(base_name: &str) -> Vec<PathBuf> {
    let mut candidates = Vec::new();

    // 1. Relative to current working directory: ./bin/<base_name>
    let mut local = PathBuf::from("bin").join(base_name);
    add_extension(&mut local);
    candidates.push(local);

    // 2. Next to the current executable: <exe_dir>/bin/<base_name>
    if let Ok(exe) = std::env::current_exe() {
        if let Some(exe_dir) = exe.parent() {
            let mut adjacent = exe_dir.join("bin").join(base_name);
            add_extension(&mut adjacent);
            candidates.push(adjacent);
        }
    }

    // 3. macOS app bundle: <exe_dir>/../../Resources/bin/<base_name>
    #[cfg(target_os = "macos")]
    if let Ok(exe) = std::env::current_exe() {
        if let Some(exe_dir) = exe.parent() {
            let mut bundle = exe_dir
                .join("../../Resources/bin")
                .join(base_name);
            add_extension(&mut bundle);
            candidates.push(bundle);
        }
    }

    candidates
}

/// Appends the appropriate executable extension on Windows.
fn add_extension(path: &mut PathBuf) {
    #[cfg(target_os = "windows")]
    path.set_extension("exe");
}

/// Spawns the sidecar process, reads its port from stdout, and returns
/// the port, the child process handle, and a task that forwards remaining stdout.
pub async fn spawn_sidecar() -> Result<(u16, Child, JoinHandle<()>)> {
    let path = find_sidecar()?;
    eprintln!("[rust] sidecar binary: {}", path.display());

    let mut child = Command::new(&path)
        .stdout(std::process::Stdio::piped())
        .stderr(std::process::Stdio::inherit())
        .spawn()
        .context("failed to spawn sidecar process")?;

    eprintln!("[rust] sidecar spawned (PID: {})", child.id().unwrap_or(0));

    let stdout = child
        .stdout
        .take()
        .context("failed to capture sidecar stdout")?;
    let reader = BufReader::new(stdout);
    let (port, lines) = read_port_from_stdout(reader).await?;

    // Forward any further output to the parent's stdout.
    let forward_handle = tokio::spawn(async move {
        let mut lines = lines;
        while let Ok(Some(line)) = lines.next_line().await {
            println!("{}", line);
        }
    });

    Ok((port, child, forward_handle))
}

/// Reads lines from the sidecar's stdout until it emits `API_PORT=<port>`.
/// Returns the port and the line reader for later forwarding.
async fn read_port_from_stdout<R: AsyncBufReadExt + Unpin>(
    reader: R,
) -> Result<(u16, Lines<R>)> {
    let mut lines = reader.lines();
    eprintln!("[rust] waiting for port announcement...");

    while let Some(line) = lines.next_line().await? {
        eprintln!("{}", line);

        if let Some(port_str) = line.strip_prefix("API_PORT=") {
            let port = port_str
                .parse::<u16>()
                .with_context(|| format!("invalid port number: '{}'", port_str))?;
            eprintln!("[rust] port received: {}", port);
            return Ok((port, lines));
        }
    }

    bail!("sidecar stdout closed before 'API_PORT=' was emitted");
}