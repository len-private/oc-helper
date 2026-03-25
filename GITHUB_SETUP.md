# GitHub Setup Guide for OC-Helper

Quick step-by-step guide to get OC-Helper on GitHub and distributed through Dalamud.

## 🚀 Quick Start (5 Minutes)

### Step 1: Create GitHub Repository

1. Go to https://github.com/new
2. Enter:
   - **Repository name**: `oc-helper`
   - **Description**: `Clean, modular Dalamud automation for Occult Crescent farming`
   - **Visibility**: **Public** (required for Dalamud)
   - **License**: AGPL-3.0
3. Click **Create repository**

Your repo URL will be: `https://github.com/YOUR_USERNAME/oc-helper`

### Step 2: Push Code to GitHub

```bash
cd /home/len-s/oc-helper

# Configure git if needed
git config --global user.name "Your Name"
git config --global user.email "you@example.com"

# Add remote
git remote add origin https://github.com/YOUR_USERNAME/oc-helper.git
git branch -M main
git push -u origin main
```

### Step 3: Update manifest.json

Replace `YOUR_USERNAME` in the file:

**File**: `/home/len-s/oc-helper/manifest.json`

```bash
# Quick substitution
sed -i 's/YOUR_USERNAME/YOUR_ACTUAL_USERNAME/g' manifest.json
git add manifest.json
git commit -m "Update manifest with repository URL"
git push
```

### Step 4: Create First Release

```bash
# Tag the release
git tag v0.0.0.1
git push origin v0.0.0.1
```

Then on GitHub (manual):
1. Go to **Releases** → **Create a new release**
2. Select tag `v0.0.0.1`
3. Title: `Release 0.0.0.1 - Initial Release`
4. Description:
   ```
   Initial release of OC-Helper

   - Treasure hunting and tracking
   - Combat automation with safety checks
   - Performance tracking
   - Comprehensive documentation

   To install, add this manifest to Dalamud:
   https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json
   ```
5. Attach `latest.zip` (build locally first, see below)
6. Click **Publish release**

### Step 5: Users Install the Plugin

Users add your manifest URL:
```
https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json
```

Then search "OC-Helper" in Dalamud plugin installer.

---

## 📦 Building and Packaging

### Build Locally

```bash
cd /home/len-s/oc-helper/SamplePlugin
dotnet build -c Release
```

### Create Release Package

```bash
cd /home/len-s/oc-helper

# Create release directory
mkdir -p release
cp -r SamplePlugin/bin/Release/SamplePlugin release/

# Package as zip
cd release
zip -r latest.zip SamplePlugin/
cd ..

# Now release/latest.zip is ready to upload to GitHub
```

---

## 📝 Files You Need

After setup, your GitHub repo should have:

```
oc-helper/
├── manifest.json              ← Users add this URL to Dalamud
├── README.md                  ← Project info
├── DEPLOYMENT.md              ← Full deployment guide
├── QUICKSTART.md              ← Development guide
├── ARCHITECTURE.md            ← Design documentation
├── CHANGELOG.md               ← Version history
├── .github/
│   └── workflows/
│       └── release.yml        ← Auto-build on tags
├── SamplePlugin/
│   ├── SamplePlugin.csproj   
│   ├── SamplePlugin.json      ← Plugin metadata
│   ├── Plugin.cs              ← Main class
│   ├── Configuration.cs       ← Settings
│   ├── Services/              ← Business logic
│   └── Windows/               ← UI windows
└── assets/
    └── icon.png               ← Plugin icon (optional)
```

---

## 🔄 Updating Your Plugin

When you make changes and want to release a new version:

```bash
# 1. Make code changes

# 2. Update version
# Edit SamplePlugin/SamplePlugin.csproj:
#   <Version>0.0.0.2</Version>
# Edit manifest.json:
#   "AssemblyVersion": "0.0.0.2"

# 3. Commit
git add -A
git commit -m "Release v0.0.0.2: Added treasure detection improvements"

# 4. Tag and push
git tag v0.0.0.2
git push origin main
git push origin v0.0.0.2

# 5. Create release on GitHub (same as Step 4 above)
```

Users will automatically see the update within a few hours.

---

## ✅ Verification Checklist

After setup, verify everything works:

- [ ] Repository is public (check GitHub repo settings)
- [ ] `manifest.json` contains YOUR_USERNAME (not placeholder)
- [ ] Manifest URL is accessible: 
  ```
  https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json
  ```
- [ ] First release created with tag `v0.0.0.1`
- [ ] Release has `latest.zip` attached
- [ ] Plugin installs from Dalamud (/xlplugins)
- [ ] `/oc` command opens the plugin UI
- [ ] Settings save and persist

---

## 🐛 Troubleshooting

**Problem**: "Plugin not found" in Dalamud
- Solution: Wait 30 minutes for Dalamud cache refresh
- Verify manifest URL is exactly:
  ```
  https://raw.githubusercontent.com/YOUR_USERNAME/oc-helper/refs/heads/main/manifest.json
  ```

**Problem**: Download fails when installing
- Solution: Check release page for `latest.zip`
- Verify all URLs in `manifest.json` are correct
- Manually check zip exists: 
  ```
  https://github.com/YOUR_USERNAME/oc-helper/releases/download/v0.0.0.1/latest.zip
  ```

**Problem**: "Plugin version incompatible"
- Solution: Ensure `AssemblyVersion` in manifest matches .csproj version

---

## 📚 Full Documentation

For detailed information, see:
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Complete deployment guide
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Code design
- **[QUICKSTART.md](QUICKSTART.md)** - Development guide

---

## Next Steps

1. ✅ Read this guide
2. ✅ Create GitHub repo
3. ✅ Push code
4. ✅ Update manifest.json
5. ✅ Build locally
6. ✅ Create first release
7. ✅ Share manifest URL with users

**You're ready to distribute!** 🎮
