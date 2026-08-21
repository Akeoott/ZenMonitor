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
    ConfigModel? initialConfig = null) : IConfigService
{
    private readonly Lock _lock = new();
    public event EventHandler<ConfigModel>? ConfigChanged;

    private ConfigModel _current = initialConfig ?? new ConfigModel();
    public ConfigModel Current
    {
        get
        {
            lock (_lock) return _current;
        }
    }

    private void SetCurrent(ConfigModel newModel)
    {
        ArgumentNullException.ThrowIfNull(newModel);
        lock (_lock) _current = newModel;
    }

    public void UpdateConfig(ConfigModel newConfigModel)
    {
        ArgumentNullException.ThrowIfNull(newConfigModel);
        SetCurrent(newConfigModel);
        logger.LogInformation("ConfigModel updated in memory (not saved).");
        ConfigChanged?.Invoke(this, newConfigModel);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(configFilePath))
        {
            logger.LogWarning("ConfigModel file not found. Using default config and saving it.");
            var defaultConfig = new ConfigModel();
            SetCurrent(defaultConfig);
            await SaveAsync(cancellationToken);
            ConfigChanged?.Invoke(this, defaultConfig);
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(configFilePath, cancellationToken);
            var loaded = JsonSerializer.Deserialize(json, ConfigContext.Default.ConfigModel);
            if (loaded != null)
            {
                SetCurrent(loaded);
                logger.LogInformation("ConfigModel loaded from {FilePath}.", configFilePath);
                ConfigChanged?.Invoke(this, loaded);
            }
            else
            {
                logger.LogWarning("Deserialized config is null. Using default.");
                var defaultConfig = new ConfigModel();
                SetCurrent(defaultConfig);
                await SaveAsync(cancellationToken);
                ConfigChanged?.Invoke(this, defaultConfig);
            }
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid JSON in config file. Resetting to default.");
            var defaultConfig = new ConfigModel();
            SetCurrent(defaultConfig);
            await SaveAsync(cancellationToken);
            ConfigChanged?.Invoke(this, defaultConfig);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "I/O error reading config file. Using default config.");
            SetCurrent(new ConfigModel());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error loading config. Using default config.");
            SetCurrent(new ConfigModel());
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(configFilePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        ConfigModel configModelToSave;
        lock (_lock) configModelToSave = _current;

        try
        {
            var json = JsonSerializer.Serialize(configModelToSave, ConfigContext.Default.ConfigModel);
            await File.WriteAllTextAsync(configFilePath, json, cancellationToken);
            logger.LogInformation("ConfigModel saved to {FilePath}.", configFilePath);
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

    internal static ConfigModel InitConfig(string configFilePath)
    {
        if (!File.Exists(configFilePath))
        {
            var defaultConfig = new ConfigModel();
            var directory = Path.GetDirectoryName(configFilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            var json = JsonSerializer.Serialize(defaultConfig, ConfigContext.Default.ConfigModel);
            File.WriteAllText(configFilePath, json);
            return defaultConfig;
        }

        try
        {
            var json = File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize(json, ConfigContext.Default.ConfigModel);
            return config ?? new ConfigModel();
        }
        catch
        {
            return new ConfigModel();
        }
    }
}
