# GitHub Actions Quick Reference

## ✅ What Was Added

Five GitHub Actions workflow files have been added to your repository:

### 1. **`.NET CI`** (`.github/workflows/dotnet-ci.yml`)
- Builds and tests the .NET 10 API
- Runs on push/PR to `main` and `develop` branches
- Generates code coverage reports

### 2. **`React UI CI`** (`.github/workflows/react-ci.yml`)
- Builds and validates the React frontend
- Runs linting and TypeScript checks
- Uploads build artifacts

### 3. **`Docker Build`** (`.github/workflows/docker-build.yml`)
- Validates Docker image builds
- Tests both API and UI containers
- Uses GitHub Actions cache for faster builds

### 4. **`Full CI/CD Pipeline`** (`.github/workflows/ci-cd.yml`)
- Comprehensive end-to-end validation
- Runs all tests and builds
- Recommended for production workflows

### 5. **`README.md`** (`.github/workflows/README.md`)
- Complete documentation
- Troubleshooting guide
- Best practices

---

## 🚀 How to Use

### View Workflows on GitHub
1. Go to: https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner
2. Click **Actions** tab
3. Watch your workflows run automatically!

### Workflows Trigger When:
- ✅ You push code to `main` or `develop`
- ✅ Someone creates a pull request
- ✅ You manually trigger them (optional)

---

## 📊 Add Build Badges to README

Add these to your main `README.md`:

```markdown
## Build Status

![.NET CI](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/.NET%20CI/badge.svg)
![React UI CI](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/React%20UI%20CI/badge.svg)
![Docker Build](https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/workflows/Docker%20Build/badge.svg)
```

---

## 🔧 What Each Workflow Does

### .NET CI Workflow
```yaml
Checkout code → Setup .NET 10 → Restore packages → Build → Run 49 tests → Code coverage
```

### React UI CI Workflow
```yaml
Checkout code → Setup Node 22 → Install deps → Lint → Type check → Build → Upload artifacts
```

### Docker Build Workflow
```yaml
Checkout code → Setup Buildx → Build API image → Build UI dev image → Build UI prod image
```

### Full CI/CD Pipeline
```yaml
Run .NET tests → Build React UI → Build Docker images (on main push only)
```

---

## 💡 Common Tasks

### Check If Workflows Are Running
```bash
# Visit GitHub Actions page
https://github.com/tuanmn-kms/dotnet-kms-training-tripplanner/actions
```

### Re-run Failed Workflow
1. Go to Actions tab
2. Click on failed workflow
3. Click "Re-run jobs" → "Re-run all jobs"

### Disable a Workflow
1. Go to Actions tab
2. Click workflow name (left sidebar)
3. Click "..." → Disable workflow

### Test Locally Before Push
```bash
# .NET
dotnet build --configuration Release
dotnet test --configuration Release

# React
cd kmstraining.reactui
npm ci
npm run build

# Docker
docker build -f KMSTraining.API/Dockerfile -t test-api .
docker build -f kmstraining.reactui/Dockerfile.dev -t test-ui ./kmstraining.reactui
```

---

## 🎯 Next Steps (Optional)

### 1. Enable Code Coverage with Codecov
```bash
# Sign up at https://codecov.io/
# Add CODECOV_TOKEN to repo secrets
# Settings → Secrets and variables → Actions → New repository secret
```

### 2. Add Branch Protection
```bash
# Settings → Branches → Add rule
# Require status checks to pass before merging
# Select: .NET CI, React UI CI, Docker Build
```

### 3. Add Deployment Workflow (Azure)
```yaml
# Create .github/workflows/deploy-azure.yml
# Add Azure credentials to secrets
# Deploy on successful main branch build
```

### 4. Add Dependabot
```yaml
# Create .github/dependabot.yml
# Auto-update dependencies weekly
```

---

## ⚠️ Troubleshooting

### Workflow Failed - .NET 10 Not Found
- .NET 10 may not be available in GitHub Actions yet
- Update to `dotnet-version: '9.0.x'` temporarily
- Or use `dotnet-version: '10.0.x-preview'`

### Workflow Failed - npm ci Error
- Check `package-lock.json` is committed
- Verify Node version matches (22.x)

### Workflow Failed - Docker Build
- Check Dockerfile paths
- Review build context in workflow
- Check build logs in Actions tab

---

## 📚 Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET Actions](https://github.com/actions/setup-dotnet)
- [Node Actions](https://github.com/actions/setup-node)
- [Docker Buildx Action](https://github.com/docker/build-push-action)

---

## 🎉 Success!

Your repository now has professional CI/CD pipelines that:
- ✅ Run tests automatically on every push
- ✅ Validate builds before merging PRs
- ✅ Catch bugs early
- ✅ Ensure code quality
- ✅ Build Docker images
- ✅ Generate coverage reports

Check the Actions tab to see them in action!
