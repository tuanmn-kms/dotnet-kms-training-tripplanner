# GitHub Actions Workflows

This directory contains CI/CD workflows for the Trip Planner project.

## Workflows Overview

### 1. `CI` (`ci-cd.yml`)
**Triggers:** Push/PR to `main` or `develop` branches

**Purpose:** Unified quality checks for API and UI

**Steps:**
- Build and test .NET API with coverage
- Upload coverage summary and Codecov report (optional)
- Build and validate React UI (lint + type check + build)
- Upload React build artifacts

**Status:** Runs on every push and pull request

---

### 2. `Docker Build` (`docker-build.yml`)
**Triggers:** Push/PR to `main` branch

**Purpose:** Build container images and publish on `main` pushes

**Steps:**
- Build API Docker image (PR + push)
- Build React UI Docker image (dev and prod)
- Push images to GHCR on push to `main`
- Use GitHub Actions cache for faster builds

**Status:** Build validation on PRs, publish on `main` pushes

---

### 3. `CodeQL` (`codeql.yml`)
**Triggers:** Push/PR to `main` and `develop`, plus weekly schedule

**Purpose:** Security and code scanning for C# and JavaScript/TypeScript

**Steps:**
- Analyze C# code with CodeQL
- Analyze JavaScript/TypeScript code with CodeQL
- Run weekly scheduled security scans

**Status:** Continuous security analysis

---

## Viewing Workflow Results

1. Go to your repository on GitHub
2. Click on the **Actions** tab
3. Select a workflow from the left sidebar
4. Click on a specific run to see details

## Code Coverage

Code coverage reports are generated and can be uploaded to [Codecov](https://codecov.io/).

To enable Codecov:
1. Sign up at https://codecov.io/
2. Add your repository
3. Add `CODECOV_TOKEN` to your repository secrets (Settings → Secrets and variables → Actions)

## Build Status Badges

Add these badges to your main README.md:

```markdown
![CI](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/CI/badge.svg)
![Docker Build](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/Docker%20Build/badge.svg)
![CodeQL](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/CodeQL/badge.svg)
```

## Local Testing

Before pushing, you can test locally:

### .NET API
```bash
dotnet restore
dotnet build --configuration Release
dotnet test KMSTraining.Tests/KMSTraining.Tests.csproj --configuration Release
```

### React UI
```bash
cd kmstraining.reactui
npm ci
npm run build
```

### Docker
```bash
# API
docker build -f KMSTraining.API/Dockerfile -t kmstraining-api .

# UI (dev)
docker build -f kmstraining.reactui/Dockerfile.dev -t kmstraining-ui:dev ./kmstraining.reactui

# UI (prod)
docker build -f kmstraining.reactui/Dockerfile -t kmstraining-ui:latest ./kmstraining.reactui
```

## Troubleshooting

### .NET 10 Not Found
If workflows fail with ".NET 10 not found", ensure:
- The `dotnet-version` in workflows matches your project
- .NET 10 is available in GitHub Actions (check https://github.com/actions/setup-dotnet)

### Node Module Issues
If npm install fails:
- Check `package-lock.json` is committed
- Verify Node.js version compatibility
- Clear cache by removing `cache: 'npm'` temporarily

### Docker Build Failures
- Check Dockerfile paths are correct
- Verify build context in workflow matches project structure
- Review build logs in Actions tab

## Customization

### Add New Workflow
1. Create new `.yml` file in `.github/workflows/`
2. Define trigger events (`on:`)
3. Add jobs and steps
4. Commit and push

### Modify Existing Workflow
1. Edit the workflow file
2. Test locally if possible
3. Commit changes
4. Monitor the Actions tab for results

## Best Practices

- ✅ Use specific action versions (e.g., `@v4` instead of `@latest`)
- ✅ Cache dependencies to speed up builds
- ✅ Run tests on every PR
- ✅ Use matrix builds for multiple .NET/Node versions (if needed)
- ✅ Separate build and deployment workflows
- ✅ Add status checks for branch protection

## Next Steps

Consider adding:
- **Deployment workflows** for Azure App Service or Container Apps
- **Security scanning** with CodeQL or Snyk
- **Dependency updates** with Dependabot
- **Performance testing** workflows
- **Release automation** with semantic versioning
