# Changelog

All notable changes to OC-Helper will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Auto-pathfinding to treasure locations
- Buff casting automation (Bard, Knight, Monk)
- Inventory management and auto-sell
- Advanced combat rotations
- Per-area configuration profiles

---

## [0.0.0.1] - 2026-03-25

### Added
- Initial release of OC-Helper
- Configuration system with 3 feature modules
  - Treasure hunting settings
  - Combat automation settings
  - Performance tracker settings
- Service-oriented architecture with 3 services
  - TreasureHuntingService for treasure detection
  - CombatAutomationService for combat logic
  - PerformanceTrackerService for metrics
- Plugin integration with Dalamud
  - Framework update hook
  - Configuration persistence
  - Command handling (/oc)
- Comprehensive documentation (500+ lines)
  - ARCHITECTURE.md - Design patterns
  - CONFIG_REFERENCE.md - Settings explanation
  - QUICKSTART.md - Development guide
  - DEPLOYMENT.md - Release instructions
  - README.md - Project overview

### Known Issues
- Pathfinding not yet implemented (TODO in TreasureHuntingService)
- Combat command execution not yet hooked up (TODO in CombatAutomationService)
- Buff system not yet implemented (TODO in CombatAutomationService)
- UI windows (ConfigWindow, MainWindow) need implementation
- No in-game status display yet

### Architecture
- Modular service-based design (easy to extend)
- Strong config typing (no broken JSON mapping like BOCCHI)
- Clear XML documentation throughout
- Framework-synchronized updates
- Proper resource disposal

---

## Version Format

OC-Helper uses semantic versioning: `MAJOR.MINOR.PATCH.BUILD`

Examples:
- `0.0.0.1` → First initial release
- `0.0.0.2` → Bug fix
- `0.1.0.0` → New feature (treasure hunting v2)
- `0.1.0.1` → Bug fix on new feature
- `1.0.0.0` → Major release

---

## Release Process

When creating a new release:

1. Update version in `SamplePlugin/SamplePlugin.csproj`
2. Update version in `manifest.json`
3. Add entry to CHANGELOG.md
4. Commit: `git commit -m "Release vX.X.X.X: Description"`
5. Tag: `git tag vX.X.X.X`
6. Push: `git push origin main vX.X.X.X`
7. Create GitHub Release with changelog

See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed instructions.

---

## Maintenance

### Updates to Add

- Bug fixes (increment PATCH)
- UI implementation (increment MINOR)
- New services (increment MINOR)
- Performance improvements (increment PATCH)
- Documentation updates (no version bump)

### Deprecations

Features will be marked as deprecated with examples of replacements before removal.

Format:
```markdown
### Deprecated
- Old config option (use NewOption instead)
```

---

## Contributors

- Initial development: OC-Helper Team
- Based on Dalamud Sample Plugin template

---

## Acknowledgments

- Dalamud for the plugin framework
- BOCCHI for inspiration on feature set
- XIVLauncher for distribution infrastructure

---

See [README.md](README.md) for more information about OC-Helper.
