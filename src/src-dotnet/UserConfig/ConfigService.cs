// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace ZenMonitor.UserConfig;

public class ConfigService(
    string configFilePath,
    ILogger<ConfigService> logger,
    Config? initialConfig = null) : IConfigService
{
    private readonly Lock _lock = new();
    private Config _current = initialConfig ?? new Config();

    public Config Current
    {
        get
        {
            lock (_lock) return _current;
        }
    }

    public void UpdateConfig(Config newConfig)
    {
        ArgumentNullException.ThrowIfNull(newConfig);
        lock (_lock)
        {
            _current = newConfig;
            logger.LogInformation("Config updated in memory (not saved).");
        }
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(configFilePath))
        {
            logger.LogWarning("Config file not found. Using default config and saving it.");
            _current = new Config();
            await SaveAsync(cancellationToken);
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(configFilePath, cancellationToken);
            var loaded = JsonSerializer.Deserialize(json, ConfigContext.Default.Config);
            if (loaded != null)
            {
                lock (_lock) _current = loaded;
                logger.LogInformation("Config loaded from {FilePath}.", configFilePath);
            }
            else
            {
                logger.LogWarning("Deserialized config is null. Using default.");
                _current = new Config();
                await SaveAsync(cancellationToken);
            }
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid JSON in config file. Resetting to default.");
            _current = new Config();
            await SaveAsync(cancellationToken);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "I/O error reading config file. Using default config.");
            _current = new Config();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error loading config. Using default config.");
            _current = new Config();
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(configFilePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        Config configToSave;
        lock (_lock) configToSave = _current;

        try
        {
            var json = JsonSerializer.Serialize(configToSave, ConfigContext.Default.Config);
            await File.WriteAllTextAsync(configFilePath, json, cancellationToken);
            logger.LogInformation("Config saved to {FilePath}.", configFilePath);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "Failed to save config to {FilePath}.", configFilePath);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error saving config.");
            throw;
        }
    }

    internal static Config InitConfig(string configFilePath)
    {
        if (!File.Exists(configFilePath))
        {
            var defaultConfig = new Config();
            var directory = Path.GetDirectoryName(configFilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            var json = JsonSerializer.Serialize(defaultConfig, ConfigContext.Default.Config);
            File.WriteAllText(configFilePath, json);
            return defaultConfig;
        }

        try
        {
            var json = File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize(json, ConfigContext.Default.Config);
            return config ?? new Config();
        }
        catch
        {
            // On error, return default (don't overwrite the file – let the service handle that later)
            return new Config();
        }
    }
}
