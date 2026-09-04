# Git Workflow

## Branch Naming

- `release/<package>-<version>` — release branches (e.g. `release/core-4.1.0`); merging a release branch into master triggers the NuGet publish workflow
- `fix/<description>` — bug fixes
- `feat/<description>` — new features

## Commit Messages

No enforced commit convention is detected. Keep messages short and descriptive. Reference issue numbers where applicable.

## Pull Requests

This repo is in the Auth0 org and has a local PR template (`.github/PULL_REQUEST_TEMPLATE.md`). Every PR must fill in:

- **Changes** — what changed and why, including any API surface changes, deprecated methods, or behavior differences
- **References** — links to support tickets, community posts, or issues
- **Testing** — how reviewers can verify the change; check all boxes that apply (unit tests, integration tests, tested on latest platform)
- **Checklist** — confirm you've read the Auth0 contribution and code-of-conduct guidelines, all tests pass, and all code guidelines in `CONTRIBUTING.md` are followed

Key rules from `CONTRIBUTING.md`:
- Maintain the existing minimum .NET framework/core support — do not raise the minimum target without prior discussion
- Keep PRs focused; change the minimum number of lines to achieve your goal
- Do not introduce breaking changes without prior discussion and approval
