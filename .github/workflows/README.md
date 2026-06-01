# GitHub Actions Workflows

This directory contains CI/CD workflows for the Trip Planner project.

## Workflows Overview

### 1. `.NET CI` (`dotnet-ci.yml`)
**Triggers:** Push/PR to `main` or `develop` branches

**Purpose:** Build and test the .NET API

**Steps:**
- Setup .NET 10
- Restore NuGet packages
- Build API in Release configuration
- Run unit tests with code coverage
- Upload coverage to Codecov (optional)

**Status:** Runs on every push and pull request

---

### 2. `React UI CI` (`react-ci.yml`)
**Triggers:** Push/PR to `main` or `develop` branches

**Purpose:** Build and validate the React frontend

**Steps:**
- Setup Node.js 22
- Install npm dependencies
- Run linting (if configured)
- TypeScript type checking
- Build production bundle
- Upload build artifacts

**Status:** Runs on every push and pull request

---

### 3. `Docker Build` (`docker-build.yml`)
**Triggers:** Push/PR to `main` branch

**Purpose:** Validate Docker image builds

**Steps:**
- Build API Docker image
- Build React UI Docker image (dev)
- Build React UI Docker image (prod)
- Use GitHub Actions cache for faster builds

**Status:** Runs on main branch only

---

### 4. `Full CI/CD Pipeline` (`ci-cd.yml`)
**Triggers:** Push/PR to `main` branch

**Purpose:** Complete end-to-end validation

**Steps:**
- Test .NET API with coverage reports
- Build React UI
- Build Docker images (on push only)
- Generate code coverage summary
- Upload artifacts for deployment

**Status:** Comprehensive workflow for production readiness

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
![.NET CI](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/.NET%20CI/badge.svg)
![React UI CI](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/React%20UI%20CI/badge.svg)
![Docker Build](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/Docker%20Build/badge.svg)
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
