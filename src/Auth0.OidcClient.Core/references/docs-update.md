# Docs Update Rules

## Tracked Docs

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` | Installation, quickstart, `Auth0Client` configuration, `Auth0ClientOptions`, supported platforms, links to per-platform quickstarts | ✅ present |
| `EXAMPLES.md` | Runnable usage samples | ❌ missing |

API reference docs are generated automatically from XML doc comments via DocFX on release — do not edit `docs/` manually.

## When You Change Code, Update These Docs

| When this changes | Update these docs |
|-------------------|-------------------|
| Public API surface (`IAuth0Client`, `Auth0ClientBase` methods) | `README.md` (usage section) |
| `Auth0ClientOptions` properties (add, remove, rename, or change defaults) | `README.md` (configuration section) |
| Authentication / OIDC flow behavior | `README.md` (quickstart) |
| Supported platforms or minimum .NET targets | `README.md` (requirements / installation) |
| NuGet package names | `README.md` (installation section) |
| Any new public method added | Add a usage sample to `README.md`; create `EXAMPLES.md` if adding a runnable demo |
| Any public method removed or renamed | Remove/update all references in `README.md` |

> Update `README.md` in the same PR — do not defer to a follow-up PR.
