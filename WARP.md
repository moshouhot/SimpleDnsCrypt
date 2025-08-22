# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

SimpleDnsCrypt is a Windows-based management tool for dnscrypt-proxy, providing a user-friendly GUI to configure and manage encrypted DNS services. It's built with C#/.NET Framework 4.8 using WPF and follows the MVVM pattern with Caliburn.Micro.

## Build Commands

### Build the solution
```powershell
# Restore NuGet packages
nuget restore SimpleDnsCrypt.sln

# Build Release x64 (default platform)
msbuild SimpleDnsCrypt.sln /p:Configuration=Release /p:Platform=x64

# Build Release x86
msbuild SimpleDnsCrypt.sln /p:Configuration=Release /p:Platform=x86

# Build Debug for development
msbuild SimpleDnsCrypt.sln /p:Configuration=Debug /p:Platform=x64
```

### Create deployment packages
```powershell
# Build ZIP package (portable version)
.\build-package.ps1 -Version 0.8.0 -CreateZIP

# Build both ZIP and MSI
.\build-package.ps1 -Version 0.8.0 -All
```

### Run tests
```powershell
# Note: No test projects found in solution
# Manual testing required for UI components
```

## Architecture Overview

### Core Components

**Entry Points & Bootstrap**
- `AppBootstrapper.cs` - MEF container configuration and application initialization
- `LoaderViewModel.cs` - Startup sequence: checks admin rights, validates prerequisites, handles auto-updates, loads configuration

**Main Application Flow**
1. Admin rights verification (required)
2. Visual C++ Redistributable check/installation
3. Auto-update check (if enabled)
4. dnscrypt-proxy folder validation
5. Configuration loading from `dnscrypt-proxy.toml`
6. Network interface detection and management
7. Main window or system tray initialization

### Service Architecture

**dnscrypt-proxy Integration**
- `DnsCryptProxyManager` - Windows service management (install/uninstall/start/stop)
- `DnscryptProxyConfigurationManager` - TOML configuration file management
- Service runs as Windows Service, managed through .NET ServiceController

**Configuration Management**
- Primary config: `dnscrypt-proxy/dnscrypt-proxy.toml`
- Backup/restore mechanism for updates
- Real-time configuration validation and hot-reload support

### MVVM Structure

**ViewModels Hierarchy**
- `MainViewModel` - Central orchestrator, manages tabs and global state
- Tab-specific ViewModels:
  - `ResolverViewModel` - DNS resolver selection
  - `QueryLogViewModel` - Query logging
  - `DomainBlacklistViewModel` - Domain blocking
  - `CloakAndForwardViewModel` - Cloaking and forwarding rules
  - `SettingsViewModel` - Application settings

**Key Services**
- `LocalNetworkInterfaceManager` - Network adapter DNS configuration
- `ApplicationUpdater` - Auto-update with minisign verification
- `PatchHelper` - Configuration migration between versions

### Data Flow

**DNS Configuration Chain**
1. User selects resolvers/settings in UI
2. ViewModel updates `DnscryptProxyConfiguration` model
3. Configuration saved to TOML file
4. Service restarted to apply changes
5. Network interfaces updated with new DNS servers

**Network Interface Management**
- Detects all network adapters
- Filters hidden/virtual interfaces (configurable)
- Sets DNS to localhost (127.0.0.1:53) when enabled
- Restores original DNS when disabled

## Key File Locations

### Configuration Files
- `dnscrypt-proxy/dnscrypt-proxy.toml` - Main configuration
- `dnscrypt-proxy/blacklist.txt` - Domain blacklist
- `dnscrypt-proxy/cloaking-rules.txt` - Cloaking rules
- `dnscrypt-proxy/forwarding-rules.txt` - Forwarding rules

### Log Files
- `logs/` - Application logs (NLog)
- `dnscrypt-proxy/query.log` - DNS query log
- `dnscrypt-proxy/blocked.log` - Blocked domains log

## Development Patterns

### Dependency Injection
Uses MEF (Managed Extensibility Framework) with attributes:
- `[Export]` for service registration
- `[ImportingConstructor]` for dependency injection
- CompositionContainer manages lifecycle

### Event Aggregation
Caliburn.Micro's EventAggregator for loose coupling between components

### Async Operations
Service operations use async/await with configurable delays:
- ServiceStartTime: 2500ms
- ServiceStopTime: 2500ms  
- ServiceRestartTime: 5000ms

### Localization
WPFLocalizeExtension with resource files in `Resources/Translation*.resx`

## Critical Paths

### Service Installation
1. Check admin privileges (required)
2. Install dnscrypt-proxy as Windows service
3. Configure service to run as LocalSystem
4. Start service
5. Update network interfaces

### Resolver Selection
1. Load resolvers from dnscrypt-proxy
2. Apply filters (DNSSec, NoLog, NoFilter, IPv6)
3. Handle automatic mode vs manual selection
4. Support for anonymized DNS routes (relays)

### Configuration Updates
1. Validate new configuration
2. Backup current configuration
3. Write new TOML file
4. Restart service if running
5. Verify service status

## External Dependencies

### NuGet Packages (Key)
- Caliburn.Micro (3.2.0) - MVVM framework
- MahApps.Metro (1.6.5) - UI controls
- Nett (0.15.0) - TOML parsing
- Hardcodet.NotifyIcon.Wpf (1.0.8) - System tray
- Costura.Fody (6.0.0) - Assembly embedding

### External Components
- dnscrypt-proxy.exe - Core DNS proxy (managed binary)
- Visual C++ Redistributable 2015-2019 - Runtime requirement

## Platform Requirements
- Windows 7 SP1 minimum
- .NET Framework 4.8
- Administrator privileges
- Visual C++ Redistributable 2015-2019
