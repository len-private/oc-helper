# Deployment Guide: Publishing OC-Helper to Dalamud

This guide explains how to build, package, and distribute OC-Helper through Dalamud's plugin system.

## Table of Contents

1. [Initial GitHub Setup](#initial-github-setup)
2. [Building and Packaging](#building-and-packaging)
3. [Creating Releases](#creating-releases)
4. [Users Installing OC-Helper](#users-installing-oc-helper)
5. [Updating Versions](#updating-versions)
6. [Troubleshooting](#troubleshooting)

---

## Initial GitHub Setup

### Step 1: Create a GitHub Repository

1. Go to [github.com/new](https://github.com/new)
2. Enter repository name: `oc-helper`
3. Description: "Clean, modular Dalamud automation for Occult Crescent farming"
4. Choose **Public** (required for Dalamud plugin discovery)
5. Initialize with README (already have one)
6. License: AGPL-3.0
7. Click **Create repository**

### Step 2: Upload Code to GitHub

```bash
cd /home/len-s/oc-helper

# Initialize git if not already done
git init
git add .
git commit -m "Initial commit: OC-Helper plugin foundation"

# Add remote and push
git remote add origin https://github.com/YOUR_USERNAME/oc-helper.git
git branch -M main
git push -u origin main
```

### Step 3: Update Manifest References

Edit `manifest.json` and replace `YOUR_USERNAME` with your actual GitHub username:

```json
{
    "IconUrl": "https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/assets/icon.png",
    "RepoUrl": "https://github.com/YOUR_USERNAME/oc-helper",
    "DownloadLinkInstall": "https://github.com/YOUR_USERNAME/oc-helper/releases/latest/download/latest.zip",
    "DownloadLinkUpdate": "https://github.com/YOUR_USERNAME/oc-helper/releases/latest/download/latest.zip"
}
```

Then commit and push:

```bash
git add manifest.json
git commit -m "Update manifest with repository URL"
git push
```

---

## Building and Packaging

### Prerequisites on Your Dev Machine

1. ✅ .NET 8 SDK installed (`dotnet --version`)
2. ✅ Dalamud development libraries
   - **Windows/Mac**: Install via XIVLauncher automatically
   - **Linux**: Set up Dalamud stubs (see below)

### Building the Plugin

```bash
cd /home/len-s/oc-helper/SamplePlugin
dotnet build -c Release
```

**Expected output:**
```
SamplePlugin/bin/Release/SamplePlugin/
├── SamplePlugin.json
├── SamplePlugin.dll
└── [other compiled files]
```

### Packaging for Distribution

Create a `latest.zip` file containing the Release output:

```bash
# From /home/len-s/oc-helper/
mkdir -p release
cp -r SamplePlugin/bin/Release/SamplePlugin release/
cd release
zip -r latest.zip SamplePlugin/
cd ..
```

The `latest.zip` should contain:
```
latest.zip
└── SamplePlugin/
    ├── SamplePlugin.json
    ├── SamplePlugin.dll
    └── [supporting files]
```

---

## Creating Releases

### Manual Release Process

#### 1. Create a Git Tag

```bash
# Tag releases as v0.0.0.1
git tag v0.0.0.1
git push origin v0.0.0.1
```

**Version Format**: Follow semantic versioning (`major.minor.patch.build`)

#### 2. Create a GitHub Release

1. Go to your repository
2. Click **Releases** → **Create a new release**
3. Select the tag (e.g., `v0.0.0.1`)
4. Title: `Release 0.0.0.1`
5. Description:
   ```
   ## OC-Helper v0.0.0.1

   ### Features
   - Initial release
   - Treasure hunting automation
   - Combat automation with safety checks
   - Performance tracking

   ### Known Issues
   - Pathfinding not yet implemented
   - [List any known issues]

   ### Installation
   Add this URL to Dalamud plugin repository settings:
   https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json
   ```

6. Upload `latest.zip` as release asset
7. Click **Publish release**

### Automated Releases with GitHub Actions

The `.github/workflows/release.yml` file will:

1. Trigger when you create a tag
2. Set up .NET SDK
3. Build the plugin
4. Package as `latest.zip`
5. Create a GitHub Release with the zip file attached

**To use this:**

```bash
# Just create a tag and push
git tag v0.0.0.2
git push origin v0.0.0.2

# GitHub Actions will automatically:
# - Build the project
# - Create latest.zip
# - Upload to GitHub Releases
```

---

## Users Installing OC-Helper

### Method 1: Add via Plugin Manager (Recommended)

1. Launch Final Fantasy XIV with XIVLauncher
2. Open plugin installer: `/xlplugins`
3. Click **Settings** (gear icon)
4. Under "Custom Plugin Repositories", click **+**
5. Paste: `https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json`
6. Click **Save**
7. Go back to plugin list
8. Search for "OC-Helper"
9. Click **Install**

### Method 2: Manual Install

1. Download `latest.zip` from [Releases](https://github.com/YOUR_USERNAME/oc-helper/releases)
2. Extract to: `%APPDATA%\XIVLauncher\plugins\SamplePlugin\`
3. Reload plugins: `/xlplugins` → **Reload** button

---

## Updating Versions

### When to Update Version

1. **Bug fix** → Increment patch: `0.0.0.1` → `0.0.0.2`
2. **New feature** → Increment minor: `0.0.1.0` → `0.1.0.0`
3. **Major rewrite** → Increment major: `1.0.0.0` → `2.0.0.0`

### Steps to Release an Update

#### 1. Update Version Numbers

**In `SamplePlugin/SamplePlugin.csproj`:**
```xml
<PropertyGroup>
    <Version>0.0.0.2</Version>
    ...
</PropertyGroup>
```

**In `manifest.json`:**
```json
{
    "AssemblyVersion": "0.0.0.2",
    "TestingAssemblyVersion": "0.0.0.2",
    ...
}
```

**In `README.md` and `QUICKSTART.md` if version is mentioned**

#### 2. Update Changelog

Edit or create `CHANGELOG.md`:

```markdown
## [0.0.0.2] - 2026-03-25

### Added
- Treasure detection improvements
- Better logging

### Fixed
- Config serialization edge case
- Memory leak in service disposal

### Changed
- Updated documentation

## [0.0.0.1] - 2026-03-25

### Added
- Initial release
- Service architecture
- Configuration system
```

#### 3. Commit and Tag

```bash
git add -A
git commit -m "Release v0.0.0.2: Improvements and fixes"
git tag v0.0.0.2
git push origin main
git push origin v0.0.0.2
```

#### 4. Create Release on GitHub

GitHub Actions will automatically create the release, or do it manually:

1. Go to **Releases**
2. Create new release from tag `v0.0.0.2`
3. Add changelog from `CHANGELOG.md`
4. Attach `latest.zip`
5. Publish

#### 5. Users Receive Update

Once you push the release, users who have OC-Helper installed will see an update available (usually within a few hours as Dalamud caches the manifest).

---

## Manifest File Reference

### Required Fields

| Field | Example | Purpose |
|-------|---------|---------|
| `Author` | "OC-Helper Contributors" | Plugin author name |
| `Name` | "OC-Helper" | Display name in plugin manager |
| `InternalName` | "OCHelper" | Unique identifier (no spaces/special chars) |
| `Punchline` | "Clean automation for OC farming" | Brief description (one line) |
| `Description` | "Full description..." | Long description with features/notes |
| `DalamudApiLevel` | 14 | Dalamud API version (check current value) |
| `AssemblyVersion` | "0.0.0.1" | Plugin version (must match .csproj) |
| `DownloadLinkInstall` | GitHub release zip URL | Where to download |
| `DownloadLinkUpdate` | GitHub release zip URL | Update from this URL |
| `ApplicableVersion` | "any" | Game version compatibility |
| `RepoUrl` | GitHub repo URL | Source code repository |

### Optional Fields

| Field | Purpose |
|-------|---------|
| `IconUrl` | Icon image (PNG recommended) |
| `Tags` | Search keywords |
| `TestingDalamudApiLevel` | API level for testing builds |
| `TestingAssemblyVersion` | Version for testing builds |
| `DownloadLinkTesting` | Testing release URL |
| `IsTestingExclusive` | Hide from main repository |

---

## Dalamud API Level

The `DalamudApiLevel` indicates which Dalamud version your plugin supports. Check the current level:

1. **Windows**: XIVLauncher displays in settings
2. **From code**: Check Dalamud.NET.Sdk version in `.csproj`
3. **Update when**:
   - XIVLauncher updates Dalamud
   - You need new Dalamud APIs
   - Bump when ready to target newer version

**Current for this project**: `14`

---

## Icon Setup (Optional)

To add a custom icon to your plugin:

### 1. Create Icon

- Size: 256x256 pixels recommended
- Format: PNG with transparency
- Place in: `assets/icon.png`

### 2. Update Manifest

```json
{
    "IconUrl": "https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/assets/icon.png"
}
```

### 3. Commit and Push

```bash
git add assets/icon.png manifest.json
git commit -m "Add plugin icon"
git push
```

---

## Troubleshooting Deployment

### Issue: "Plugin not found" in Dalamud

**Causes:**
- Manifest URL is wrong
- Repository is private (needs to be public)
- Manifest JSON is malformed

**Fix:**
- Verify manifest URL is accessible: `https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json`
- Check repository is public: **Settings** → **Change to public**
- Validate JSON: `jq . manifest.json`

### Issue: Download fails

**Causes:**
- Release zip doesn't exist
- Wrong URL in manifest
- GitHub rate limiting

**Fix:**
- Check release page: github.com/YOUR_USERNAME/oc-helper/releases
- Verify all URLs in manifest
- Use `git_release` URLs, not `latest`

### Issue: Users can't update

**Cause:**
- AssemblyVersion in .csproj doesn't match manifest.json

**Fix:**
- Ensure they match exactly (e.g., both `0.0.0.2`)
- Format: `major.minor.patch.build`

---

## Release Checklist

Before publishing a new version:

- [ ] Update version in `SamplePlugin.csproj`
- [ ] Update version in `manifest.json`
- [ ] Update `CHANGELOG.md`
- [ ] Update `README.md` if needed
- [ ] Build locally: `dotnet build -c Release`
- [ ] Test plugin in XIVLauncher (/oc command works, settings save)
- [ ] Create git tag: `git tag v0.0.0.X`
- [ ] Push to main: `git push origin main`
- [ ] Push tag: `git push origin v0.0.0.X`
- [ ] Create GitHub Release with download link
- [ ] Verify manifest URL is correct
- [ ] Test installation from Dalamud plugin manager

---

## Quick Reference

### First Time Setup
```bash
# 1. Create GitHub repo
# 2. Clone it locally

git remote add origin <repo-url>
git push -u origin main

# 3. Update manifest.json with YOUR_USERNAME
# 4. Build plugin
cd SamplePlugin && dotnet build -c Release

# 5. Create release
git tag v0.0.0.1
git push origin v0.0.0.1
# Create GitHub Release with latest.zip
```

### Regular Updates
```bash
# 1. Make code changes
# 2. Update version in .csproj and manifest.json
# 3. Commit
git add -A
git commit -m "Release vX.X.X.X: Description"

# 4. Tag and push
git tag vX.X.X.X
git push origin main vX.X.X.X

# 5. Create GitHub Release
# (GitHub Actions handles this automatically)
```

### User Installation
```
/xlplugins → Settings → Custom Plugin Repositories
Add: https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json
Search "OC-Helper" → Install
```

---

## Additional Resources

- [Dalamud Plugin Development](https://dalamud.dev/plugin-development/)
- [GitHub Releases Documentation](https://docs.github.com/en/repositories/releasing-projects-on-github)
- [Semantic Versioning](https://semver.org/)
- [JSON Validator](https://jsonlint.com/) - Validate manifest.json

---

## Next Steps

1. **Test the build locally** - Ensure `dotnet build -c Release` succeeds on your dev machine with Dalamud installed
2. **Set up GitHub repo** - Follow "Initial GitHub Setup" above
3. **Create first release** - Tag v0.0.0.1 and create GitHub Release
4. **Test user installation** - Have someone add your manifest and install
5. **Monitor feedback** - Watch for issues and update accordingly

---

**Questions?** Check `README.md` for architecture overview or `QUICKSTART.md` for development help.
