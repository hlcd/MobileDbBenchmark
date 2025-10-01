# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MobileDbBenchmark is a .NET 9 MAUI mobile database benchmarking application that measures Realm database performance on Android and iOS platforms. The project tests various database operations including inserts, counts, selects, updates, deletes, and many-to-many relationships.

**Migration Status**: Successfully migrated from Xamarin.Forms to .NET 9 MAUI. See `MIGRATION-MAUI.md` for detailed migration history.

## Solution Structure

The solution consists of 4 projects (multi-project MAUI structure):

- **MobileDbBenchamark.Common** (net9.0) - Shared benchmark logic, database models, and test implementations
- **MobileDbBenchmark.UI** (net9.0) - MAUI UI layer with XAML pages
- **MobileDbBenchmark.Android** (net9.0-android) - Android platform-specific implementation
- **MobileDbBenchmark.iOS** (net9.0-ios) - iOS platform-specific implementation

## Key Technologies

- **.NET 9.0** - Target framework (net9.0, net9.0-android, net9.0-ios)
- **MAUI 9.0.0** (Microsoft.Maui.Controls) - Cross-platform UI framework
- **Realm 10.18.0** - Mobile database (DO NOT upgrade)
- **C# 13** - Language version
- **Acr.UserDialogs.Maui 9.2.2** - Dialog library
- **Fody** - Used for Realm weaving

## Build Commands

**Note**: This is a .NET 9 MAUI project using SDK-style projects. Use `dotnet` CLI commands for building.

### Build entire solution
```bash
dotnet build MobileDbBenchmark.sln -c Debug
dotnet build MobileDbBenchmark.sln -c Release
```

### Build specific projects
```bash
# Common library
dotnet build MobileDbBenchmark/MobileDbBenchamark.Common/MobileDbBenchamark.Common.csproj

# UI library
dotnet build MobileDbBenchmark/MobileDbBenchmark.UI/MobileDbBenchmark.UI.csproj

# Android
dotnet build MobileDbBenchmark/MobileDbBenchmark.Android/MobileDbBenchmark.Android.csproj -c Debug
dotnet build MobileDbBenchmark/MobileDbBenchmark.Android/MobileDbBenchmark.Android.csproj -c Release

# iOS
dotnet build MobileDbBenchmark/MobileDbBenchmark.iOS/MobileDbBenchmark.iOS.csproj -c Debug
dotnet build MobileDbBenchmark/MobileDbBenchmark.iOS/MobileDbBenchmark.iOS.csproj -c Release
```

### Android-specific build configurations
- **Debug**: No linking, shared runtime enabled
- **Release**: SDK-only linking, AOT profiling enabled with custom profile (`custom.aprof`)
- **Runtime Identifiers**: android-arm, android-arm64, android-x86, android-x64
- **Min SDK**: 26 (Android 8.0)
- **Target SDK**: 35 (Android 15)
- **Known warnings**: XA0141 (Realm library alignment), XA4301 (duplicate librealm-wrappers.so) - both non-critical

### iOS-specific build configurations
- **Min iOS Version**: 15.0 (configured in Info.plist)
- **Target Framework**: net9.0-ios
- **Known warnings**: Supported iPhone orientations not set - non-critical

## Architecture

### Benchmark System

The benchmark system is built around an abstract `BenchmarkBase` class that defines database operations:

- Core operations: `OpenDB()`, `DeleteDB()`, `RunInTransaction()`
- Test operations: Insert, Count, Select, Update (single/many transactions), Delete (single/many transactions), Collections
- Timing: Each test is wrapped with `PerformTest()` which measures elapsed time

The `RealmBenchmark` class implements all benchmark operations for Realm database.

### Test Specifications

Tests are defined using `TestSpec` objects that specify:
- `DbType` - Currently only Realm is supported
- `BenchmarkTest` - The type of operation to test
- `NumberOfItems` - Number of records to operate on
- `RepeatTimes` - How many times to run the test
- `RemoveDbBetweenIterations` - Whether to reset database between runs

### Data Models

Located in `MobileDbBenchamark.Common/Models/Realm/`:

- `Publication` - Main entity with Id, Title, CoverUrl, Version, RemoteId (indexed), DownloadPercentage, SomeDescription
- `PublicationCollection` - Collection entity with many-to-many relationship to Publications via backlinks

### Platform Services

Each platform implements services using MAUI dependency injection (configured in `MauiProgram.cs`):
- `IMemoryService` - Memory usage tracking
- `IDialogService` - UI dialogs
- `IStorageManager` - Platform-specific storage path management
  - Android: `AndroidStorageManager`
  - iOS: `IOSStorageManager`

Services are registered in platform-specific `MauiProgram.cs` files using `builder.Services.AddSingleton<TInterface, TImplementation>()`.

## Important Notes

### Realm Configuration
- Database file: `test.realm`
- Current schema version: 2
- Migration callback defined in `RealmBenchmark.cs:79`
- Configuration located at `RealmBenchmark.Config`

### Android AOT Profiling
The Android Release build uses custom AOT profiling (`AndroidEnableProfiledAot=true`, `AndroidUseDefaultAotProfile=false`) with a custom profile file at `MobileDbBenchmark.Android/custom.aprof`.

### Recent Changes
- **December 2024**: Migrated from Xamarin.Forms to .NET 9 MAUI (multi-project structure)
  - Converted all projects to SDK-style .csproj files
  - Replaced Xamarin.Forms with Microsoft.Maui.Controls 9.0.0
  - Updated from .NET Standard 2.0 to .NET 9.0
  - Migrated from Xamarin.Forms DependencyService to MAUI dependency injection
  - Updated Acr.UserDialogs to Acr.UserDialogs.Maui 9.2.2
  - Kept Realm at 10.18.0 (do NOT upgrade)
  - See `MIGRATION-MAUI.md` and `MIGRATION_PART1.MD`, `MIGRATION_PART2.MD` for details
- **Earlier**: Upgraded from Realm 10.1.2 to 10.18.0 (branch `realm10_18` merged to `master`)
- Custom AOT profile updated for Realm 10

## Common Issues

### Typo in namespace
Note there's a typo in the Common project name: "MobileDbBenchamark" (missing 'h' in Benchmark). This is consistent throughout the codebase.

### FodyWeavers
Realm requires Fody weaving. Each project that uses Realm models has a `FodyWeavers.xml` file that configures the Realm weaver.

### MAUI Architecture Notes
- **Multi-project structure**: This project uses separate platform projects (Android/iOS) instead of single-project MAUI
- **Entry points**:
  - Android: `MainActivity.cs` extends `MauiAppCompatActivity`
  - iOS: `AppDelegate.cs` extends `MauiUIApplicationDelegate`
- **App initialization**: Each platform has a `MauiProgram.cs` that configures the MAUI app and registers platform-specific services
- **UI Layer**: Shared UI in `MobileDbBenchmark.UI` project uses MAUI Controls (XAML with xmlns: `http://schemas.microsoft.com/dotnet/2021/maui`)
- **FlyoutPage**: Uses MAUI `FlyoutPage` (formerly Xamarin.Forms `MasterDetailPage`)

### Platform-Specific Configuration
- **Android**: Platform settings in `AndroidManifest.xml` (minSdkVersion, targetSdkVersion)
- **iOS**: Platform settings in `Info.plist` (MinimumOSVersion)
- Prefer these configuration files over csproj settings for platform-specific values