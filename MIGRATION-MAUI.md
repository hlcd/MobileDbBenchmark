# Migration Plan: Xamarin.Forms to .NET 9 MAUI (Multi-Project Structure)

## Phase 1: Foundation Setup ✅ COMPLETED
1. ✅ **Create global.json** with .NET 9 SDK configuration (version 9.0.303, workload version 9.0.100)
2. **Install .NET 9 SDK** and MAUI workload if not already present

## Phase 2: Migrate Common Library ✅ COMPLETED
3. ✅ **Update MobileDbBenchamark.Common.csproj**:
   - ✅ Changed `<TargetFramework>` from `netstandard2.0` to `net9.0`
   - ✅ Updated `<LangVersion>` from `7.3` to `13`
   - ✅ Kept Realm package at `10.18.0` (do NOT upgrade)
   - ✅ Kept the project SDK-style structure (already modern)
   - ✅ Added `NU1900` to NoWarn list for NuGet vulnerability warnings
   - ✅ Build succeeded - Common library compiles with .NET 9

## Phase 3: Migrate UI Library ✅ COMPLETED
4. ✅ **Update MobileDbBenchmark.UI.csproj**:
   - ✅ Changed `<TargetFramework>` from `netstandard2.0` to `net9.0`
   - ✅ Updated `<LangVersion>` from `7.3` to `13`
   - ✅ Replaced `Xamarin.Forms` with `Microsoft.Maui.Controls` (9.0.0) and `Microsoft.Maui.Controls.Compatibility` (9.0.0)
   - ✅ Kept Realm at `10.18.0`
   - ✅ Added `CS0618` to NoWarn list for obsolete MainPage property
5. ✅ **Update UI code files**:
   - ✅ Updated all XAML files xmlns from `http://xamarin.com/schemas/2014/forms` to `http://schemas.microsoft.com/dotnet/2021/maui`
   - ✅ Updated MainPage from `MasterDetailPage` to `FlyoutPage` (MAUI equivalent)
   - ✅ Updated all C# files: `using Xamarin.Forms;` → `using Microsoft.Maui.Controls;`
   - ✅ Removed `[XamlCompilation(XamlCompilationOptions.Compile)]` attributes (not needed in MAUI)
   - ✅ DependencyService calls remain (will be migrated in platform projects)
   - ✅ Build succeeded - UI library compiles with .NET 9 and MAUI

## Phase 4: Migrate Android Project ✅ COMPLETED
6. ✅ **Update AndroidManifest.xml**:
   - ✅ Changed `<uses-sdk android:minSdkVersion="21"` to `android:minSdkVersion="26"`
   - ✅ Changed `android:targetSdkVersion="29"` to `android:targetSdkVersion="35"`
7. ✅ **Convert MobileDbBenchmark.Android.csproj** to SDK-style:
   - ✅ Changed from old-style to `<Project Sdk="Microsoft.NET.Sdk">` with `<UseMaui>true</UseMaui>`
   - ✅ Set `<TargetFrameworks>net9.0-android</TargetFrameworks>`
   - ✅ Set `<LangVersion>13</LangVersion>`
   - ✅ Added `<SupportedOSPlatformVersion>26.0</SupportedOSPlatformVersion>` to match AndroidManifest
   - ✅ Set `<OutputType>Exe</OutputType>` and `<ApplicationId>`
   - ✅ Replaced `AndroidSupportedAbis` with `RuntimeIdentifiers` (android-arm;android-arm64;android-x86;android-x64)
   - ✅ Kept custom AOT profile configuration
8. ✅ **Update Android code**:
   - ✅ Updated MainActivity: `FormsAppCompatActivity` → `MauiAppCompatActivity`
   - ✅ Created MauiProgram.cs with MAUI initialization and dependency injection
   - ✅ Removed `[assembly: Xamarin.Forms.Dependency]` attributes from all services
   - ✅ Registered services in MauiProgram: IStorageManager, IMemoryService, IDialogService
   - ✅ Updated to `Acr.UserDialogs.Maui` (9.2.2)
   - ✅ Removed AndroidX and other Xamarin-specific packages
   - ✅ Cleaned up AssemblyInfo.cs (removed duplicate attributes)
   - ✅ Fixed namespace conflicts (Android.App.Application vs Microsoft.Maui.Controls.Application)
   - ✅ Build succeeded with only Realm library alignment warnings

## Phase 5: Migrate iOS Project ✅ COMPLETED
9. ✅ **Update Info.plist**:
   - ✅ Changed `<key>MinimumOSVersion</key>` from `11.0` to `15.0`
10. ✅ **Convert MobileDbBenchmark.iOS.csproj** to SDK-style:
    - ✅ Changed from old-style to `<Project Sdk="Microsoft.NET.Sdk">` with `<UseMaui>true</UseMaui>`
    - ✅ Set `<TargetFrameworks>net9.0-ios</TargetFrameworks>`
    - ✅ Set `<LangVersion>13</LangVersion>`
    - ✅ Did NOT set SupportedOSPlatformVersion in csproj (using Info.plist instead as requested)
    - ✅ Kept `<OutputType>Exe</OutputType>`
    - ✅ Replaced `Xamarin.Forms` with `Microsoft.Maui.Controls` (9.0.0) and `Microsoft.Maui.Controls.Compatibility` (9.0.0)
    - ✅ Updated to `Acr.UserDialogs.Maui` (9.2.2)
    - ✅ Kept Realm at `10.18.0`
11. ✅ **Update iOS code**:
    - ✅ Updated AppDelegate: `FormsApplicationDelegate` → `MauiUIApplicationDelegate`
    - ✅ Replaced `global::Xamarin.Forms.Forms.Init()` with `CreateMauiApp()` override
    - ✅ Created MauiProgram.cs with MAUI initialization and dependency injection
    - ✅ Removed `[assembly: Xamarin.Forms.Dependency]` attributes from all services
    - ✅ Registered services in MauiProgram: IStorageManager (IOSStorageManager), IMemoryService, IDialogService
    - ✅ Configured Acr.UserDialogs for iOS
    - ✅ Updated Main.cs to use `typeof(AppDelegate)` instead of string (fixed CS0618 warning)
    - ✅ Cleaned up AssemblyInfo.cs (removed duplicate attributes)
    - ✅ Build succeeded with only orientation warning (non-critical)

## Phase 6: Create MauiProgram.cs ✅ COMPLETED
12. ✅ **Add MauiProgram.cs** to Android and iOS projects:
    - ✅ Created separate MauiProgram.cs for Android project with Android-specific services
    - ✅ Created separate MauiProgram.cs for iOS project with iOS-specific services
    - ✅ Configured `MauiApp.CreateBuilder()` in both
    - ✅ Registered platform-specific services: IStorageManager, IMemoryService, IDialogService
    - ✅ Configured Acr.UserDialogs for each platform
    - ✅ Kept existing App.xaml.cs initialization logic

## Phase 7: Update Solution & Dependencies ✅ COMPLETED
13. ✅ **Update package references across all projects**:
    - ✅ Replaced `Xamarin.Forms` (4.8.0.1534) with `Microsoft.Maui.Controls` (9.0.0) and `Microsoft.Maui.Controls.Compatibility` (9.0.0)
    - ✅ Updated `Acr.UserDialogs` to `Acr.UserDialogs.Maui` (9.2.2) in Android and iOS projects
    - ✅ Kept Realm at `10.18.0` across all projects (Common, UI, Android, iOS)
    - ✅ Removed all Xamarin-specific packages and references
    - ✅ Removed obsolete configuration files:
      - Deleted empty `packages.config` from UI project
      - Deleted `app.config` files from Android and iOS projects (no longer needed in SDK-style projects)
    - ✅ Verified package consistency across all projects:
      - **Common**: Realm 10.18.0
      - **UI**: Microsoft.Maui.Controls 9.0.0, Microsoft.Maui.Controls.Compatibility 9.0.0, Realm 10.18.0
      - **Android**: Microsoft.Maui.Controls 9.0.0, Microsoft.Maui.Controls.Compatibility 9.0.0, Acr.UserDialogs.Maui 9.2.2, Realm 10.18.0
      - **iOS**: Microsoft.Maui.Controls 9.0.0, Microsoft.Maui.Controls.Compatibility 9.0.0, Acr.UserDialogs.Maui 9.2.2, Realm 10.18.0
    - ✅ No Xamarin references remain in any .csproj files

## Phase 8: Build & Test ✅ COMPLETED
14. ✅ **Incremental build & fix**:
    - ✅ Build Common library first - SUCCESS (1.96s, 0 warnings, 0 errors)
    - ✅ Build UI library - SUCCESS (0.74s, 0 warnings, 0 errors)
    - ✅ Build Android project (Debug) - SUCCESS (37.11s, 2 Realm alignment warnings)
      - Warning XA0141: Realm 10.18.0 shared library alignment warning (non-critical)
      - Warning XA4301: APK duplicate librealm-wrappers.so (non-critical)
    - ✅ Build iOS project (Debug) - SUCCESS (2.06s, 1 orientation warning)
      - Warning: Supported iPhone orientations not set (non-critical)
    - ⏭️ Test Realm database operations still work (manual testing by user)
    - ⏭️ Verify AOT profile works - Android Release (manual testing by user)

## Phase 9: Update Documentation ✅ COMPLETED
15. ✅ **Update CLAUDE.md** with new .NET 9 MAUI multi-project structure and build commands:
    - ✅ Updated project overview to reflect .NET 9 MAUI status
    - ✅ Updated solution structure with target frameworks (net9.0, net9.0-android, net9.0-ios)
    - ✅ Replaced all Xamarin.Forms references with MAUI 9.0.0
    - ✅ Updated build commands to use `dotnet` CLI instead of `msbuild`
    - ✅ Added platform-specific build configuration details (SDK versions, known warnings)
    - ✅ Updated Platform Services section with MAUI dependency injection pattern
    - ✅ Added Recent Changes section documenting the migration
    - ✅ Added MAUI Architecture Notes section
    - ✅ Added Platform-Specific Configuration notes

## Key Settings:
- **C# Language Version**: 13
- **Android minSdk**: 26 (in AndroidManifest.xml)
- **Android targetSdk**: 35 (in AndroidManifest.xml)
- **iOS MinimumOSVersion**: 15.0 (in Info.plist)
- **Realm**: Stay at 10.18.0
- **Multi-project structure**: Keep separate Android/iOS projects

## Important Notes:
- Start with Common library to validate .NET 9 compatibility independently
- Use incremental approach: Common → UI → Android → iOS
- Keep Realm at 10.18.0 (do not upgrade)
- Prefer platform-specific configuration files (AndroidManifest.xml, Info.plist) over csproj settings
- SDK-style csproj for Android/iOS projects is a major structural change from old-style