# GLTF plug-in for Envjoy

## Introduction

This project is a Unity plug-in that allows you to import and export GLTF files in Unity
with features includeds by Envjoy extension.

## Installation

### Install Manually

1. Clone or download the repository.
2. Inside the package manager click the '+' button and select 'Add package from disk...'.
3. Select the package.json file inside the repository folder.
4. Package is now installed.

### Install via git URL

1. Inside the package manager click the '+' button and select 'Add package from git URL...'.
2. Paste the repository URL and concatenate it with #upm at the end. It only contains the last version of the package but it may be out of sync compared to the master branch.
3. Package is now installed.

### Install via tarball

1. Download the tarball from the releases section.
2. Inside the package manager click the '+' button and select 'Add package from tarball...'.
3. Select the downloaded tarball.
4. Package is now installed.

### Dependencies

- [Unity GLTF](https://github.com/KhronosGroup/UnityGLTF)
- [Unitask](https://github.com/Cysharp/UniTask)

## Development

### Requirements

- Unity 2020.3.0f1 or higher.

### Workflow

The metodology used for this project is [Gitflow](https://www.atlassian.com/es/git/tutorials/comparing-workflows/gitflow-workflow). It is a metodology based on the division of the different stages of the development of a software project into branches. To this add a pair of changes to do more legible the temporal line of the project.

### How to use

- Follow the Gitflow workflow as the main guideline, with some tweaks.
  Developers rebase their branches with develop daily to avoid conflicts.

- When a developer's work can be included in the develop branch, a Pull Request is opened to begin the code review on the proposed changes.

- Once the changes are prepared, tested, reviewed and approved, the Pull Request is merged through non-fast-forward into develop, creating a new merge commit.

- From this moment we can have two temporary branches, the release and the hotfix

  - At the moment there are enough changes for a new version, a temporary release branch is created where small bugs are fixed and the metadata is prepared (version number, build number, changelog, documentation, etc.).

  - In the case that we have to fix an error in production immediately due to a critical error or any other circumstance, we will create a temporary hotfix branch where the necessary changes will be made to solve the problem without forgetting to prepare the metadata (version number, build number, changelog, documentation, etc.).

- Finally, once the Pull Request of the release or hotfix branch is made, a new version is ready to be published in the master branch. It is at this point that we perform a merge of the temporary branch (release or hotfix) consecutively through fast-forward in master and develop.

> In case there is a release branch open in develop during a hotfix, the merge through fast-forward will be done directly to the release branch.

### Naming conventions

- master

  - Note: It can always be deployed to production.
  - You should never push to it.

- develop

  - Origin: master
  - Note: It can only be merged through Pull Requests.

- release/\*

  - Origin: develop
  - Merge in: master and develop
  - Note: A version that is expected to be deployed soon.
  - It can only be merged through Pull Requests.
  - Ex: release/1.2.3

- hotfix/\*

  - Origin: master
  - Merge in: master and develop
  - Note: Fixing bugs in production.
  - It can only be merged through Pull Requests.
  - Ex: hotfix/critical-error

- feat/\*

  - Origin: develop
  - Merge in: develop
  - Note: New feature.
  - It can only be merged through Pull Requests.
  - Ex: feat/new-feature

- fix/\*

  - Origin: develop
  - Merge in: develop
  - Note: Fixing bugs in development.
  - Ex: fix/remove-unused-dependency

- ref/\*

  - Origin: develop
  - Merge in: develop
  - Note: Refactoring/improving existing features.
  - Ex: ref/improve-net-reliability

- chore/\*

  - Origin: develop
  - Merge in: develop
  - Note: Configuration changes, tool/library updates, build adjustments, etc.
  - Ex: chore/update-editor-package

### Commit message conventions

The commit message consists in a three-part structure separated by a blank line:

- Subject
- Body
- Footer

The subject line consists in a type and a description thats not longer than 50 characters, initial letter should be capitalized and no dot at the end.
The types allowed are:

- task: A integration or implementation task.
- feat: A new feature.
- fix: A bug fix.
- refactor: A code change that neither fixes a bug nor adds a feature.
- style: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc).
- docs: Documentation only changes.
- test: Adding missing tests or correcting existing tests.
- chore: Changes to the build process or auxiliary tools and libraries such as documentation generation.

The body is optional and not always the commits need it. It should be used to explain the 'what' and 'why' of the commit, not the 'how'. The body should be wrapped at 72 characters.

Finally, the footer is optional and should be used to reference issues of clickup tasks. Multiple issues can be referenced by adding multiple footers.

The format should be: `#<issue number> <description>`.
