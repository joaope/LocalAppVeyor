.NET Core global tool which brings _**appveyor.yml**_ to the center of your build process by making possible to execute 
its build jobs, locally.

| Windows | OS X / Linux  | Nuget  |
| ------------- |:-------------:| ----- |
|[![Build status](https://ci.appveyor.com/api/projects/status/hpi2lwuhrr2qbhfm?svg=true)](https://ci.appveyor.com/project/joaope/localappveyor)|[![Build Status](https://github.com/joaope/LocalAppVeyor/workflows/Build/badge.svg)](https://github.com/joaope/LocalAppVeyor/actions)|[![Nuget](https://img.shields.io/nuget/v/LocalAppVeyor.svg?maxAge=0)](https://www.nuget.org/packages/LocalAppVeyor/)|

- [How it works](#how-it-works)
- [Install](#install)
- [Usage](#usage)
  - [• `build` command](#%e2%80%a2-build-command)
  - [• `jobs` command](#%e2%80%a2-jobs-command)
  - [• `lint` command](#%e2%80%a2-lint-command)
- [Supported build steps](#supported-build-steps)

## How it works
LocalAppVeyor tries to strictly follow same [build pipeline](https://www.appveyor.com/docs/build-configuration/#build-pipeline) 
as [AppVeyor CI](https://appveyor.com) itself.

1. Grabs _appveyor.yml_'s build configuration from current (or specified) local repository folder.
2. Reads [supported build steps](#supported-build-steps) from it.
3. Executes [build pipeline](https://www.appveyor.com/docs/build-configuration/#build-pipeline) for each job (or specified ones)
on the [build matrix](https://www.appveyor.com/docs/build-configuration/#build-matrix).

Build engine tries to be the less intrusive as possible printing only what it comes from the build output.

## Install

Install LocalAppVeyor as a [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) global tool using the following command:
```console
dotnet tool install -g localappveyor
```
You have it now available on your command line:

```console
LocalAppVeyor --help
```

<sup>*Note: to use CLI tool command you must have [.NET Core 2.1](https://www.microsoft.com/net/download) or higher.*</sup>

## Usage
```
Usage: LocalAppVeyor [options] [command]

Options:
  -?|-h|--help  Show help information
  -v|--version  Show version information

Commands:
  build  Executes one or all build jobs on specified repository directory
  jobs   List all build jobs available to execution.
  lint   Validates appveyor.yml YAML configuration. It requires internet connection.

Use "LocalAppVeyor [command] --help" for more information about a command.
```

### • `build` command

This is the main console command which allows one to execute all or a smaller set of jobs from the 
[build matrix](https://www.appveyor.com/docs/build-configuration/#build-matrix). `--job` command should be followed by a integer
corresponding to job index as listed on `jobs` command

```
Usage: LocalAppVeyor build [options]

Options:
  -?|-h|--help  Show help information
  -d|--dir      Local repository directory where appveyor.yml sits. If not specified current directory is used
  -j|--job      Job to build. You can specify multiple jobs. Use 'jobs' command to list all jobs
  -s|--skip     Step to skip from the build pipeline. You can specify multiple steps.
```

### • `jobs` command

Lists all available jobs on the specified appveyor YAML configuration file build matrix.

```
Usage: LocalAppVeyor jobs [options]

Options:
  -?|-h|--help  Show help information
  -d|--dir      Local repository directory where appveyor.yml sits. If not specified current directory is used
```

### • `lint` command

Validates appveyor.yml YAML configuration. It requires an active internet connection as it uses AppVeyor web API for a real and up to date validation.

```
Usage: LocalAppVeyor lint [options]

Options:
  -?|-h|--help  Show help information
  -t|--token    AppVeyor account API token. If not specified it tries to get it from LOCALAPPVEYOR_API_TOKEN environment variable. You can find it here: https://ci.appveyor.com/api-token
  -d|--dir      Local repository directory where appveyor.yml sits. If not specified current directory is used
```

## Supported build steps
Due to LocalAppVeyor's nature only a subset of [AppVeyor build steps](https://www.appveyor.com/docs/build-configuration/#build-pipeline)
are supported. Some of them might get some support later in time, after consideration, but others most likely won't ever be part 
of the build pipeline.

:white_check_mark: Fully supported &emsp; :large_blue_circle: Partially supported &emsp; :red_circle: Not yet supported

| Step \ Option  | Support           | Notes  |
| ------------- |:-------------:| ----- |
| version | :white_check_mark: | `{build}` placeholder is replaced by `0`
| environment | :white_check_mark: | As for the [standard AppVeyor variables](https://www.appveyor.com/docs/environment-variables/) these are the ones supported: `APPVEYOR`, `CI`, `APPVEYOR_BUILD_FOLDER`, `APPVEYOR_BUILD_NUMBER`, `APPVEYOR_BUILD_VERSION`, `PLATFORM` and `CONFIGURATION` |
| configuration | :white_check_mark: | |
| platform | :white_check_mark: | |
| os | :white_check_mark: | Relatively undocumented option but it exists apparently. It's usually a single value so it serves nothing other than to build the matrix job name. |
| init | :white_check_mark: | |
| clone_folder | :white_check_mark: | Tries first to clone to specified `clone_folder`, if any; otherwise it creates a random directory in user's temp folder. From this step on all scripts will be executed as the clone folder being the working directory. |
| matrix | :white_check_mark: | |
| install | :white_check_mark: | |
| assembly_info | :white_check_mark: | |
| before_build | :white_check_mark: | |
| build | :white_check_mark: | |
| build_script | :white_check_mark: | |
| after_build | :white_check_mark: | |
| before_test | :red_circle: | |
| test | :red_circle: | |
| test_script | :large_blue_circle: | It will always execute if it exists, no matter if other tests options are specified. |
| after_test | :red_circle: | |
| on_success | :white_check_mark: | |
| on_failure | :white_check_mark: | |
| on_finish | :white_check_mark: | |
