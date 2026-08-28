// Copyright (c) Ame (Akeoot/Akeoott) <akeoot@pm.me>. Licensed under the LGPL-3.0 Licence.
// See the LICENSE file in the repository root for full license text.

export type Theme = 'dark' | 'light';

export type LogLevel =
  | 'verbose' | 'debug'
  | 'info' | 'information'
  | 'warn' | 'warning'
  | 'error' | 'fatal';

export interface ConfigModel {
  Theme: Theme;
  LogLevel: LogLevel;
  Delay: number;
}
