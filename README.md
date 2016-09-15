# LocalAppVeyor
[![Build status](https://ci.appveyor.com/api/projects/status/hpi2lwuhrr2qbhfm?svg=true)](https://ci.appveyor.com/project/joaope/localappveyor)
[![Nuget](https://img.shields.io/nuget/v/LocalAppVeyor.svg?maxAge=2592000)](https://www.nuget.org/packages/LocalAppVeyor/)

Console application which brings _appveyor.yml_ to the center of your build process by making possible to execute 
its builds jobs, locally.

## How it works
LocalAppVeyor tries to strictly follow the same [build pipeline](https://www.appveyor.com/docs/build-configuration/#build-pipeline) 
from [AppVeyor CI](https://appveyor.com) itself.

1. Grabs _appveyor.yml_'s build configuration from current (or specified) local repository folder.
2. Reads the [supported build steps](#supported-build-steps) from it.
3. Executes the [build pipeline](https://www.appveyor.com/docs/build-configuration/#build-pipeline) for each job (or specified ones)
on the [build matrix](https://www.appveyor.com/docs/build-configuration/#build-matrix).

Build engine tries to be the less intrusive as possible printing only what it comes from the build output.

## Usage
```
Usage: LocalAppVeyor [options] [command]

Options:
  -?|-h|--help  Show help information
  -v|--version  Show version information

Commands:
  build  Executes one or all build jobs on specified repository directory
  jobs   List all build jobs available to execution.

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
```

### • `jobs` command
```
Usage: LocalAppVeyor jobs [options]

Options:
  -?|-h|--help  Show help information
  -d|--dir      Local repository directory where appveyor.yml sits. If not specified current directory is used
```

## Supported build steps
Due to LocalAppVeyor's nature only a subset of [AppVeyor build steps](https://www.appveyor.com/docs/build-configuration/#build-pipeline)
are supported. Some of them might get some support later in time, after consideration, but others most likely won't ever be part 
of the build pipeline.

:white_check_mark: Fully supported &emsp; :large_blue_circle: Partially supported &emsp; :red_circle: Not yet supported

| Step \ Option  | Support           | Notes  |
| ------------- |:-------------:| ----- |
| environment | :white_check_mark: | As for the [standard AppVeyor variables](https://www.appveyor.com/docs/environment-variables/) these are the ones supported: `APPVEYOR`, `CI`, `APPVEYOR_BUILD_FOLDER`, `PLATFORM` and `CONFIGURATION` |
| configuration | :white_check_mark: | |
| platform | :white_check_mark: | |
| os | :white_check_mark: | Relatively undocumented option but it exists apparently. It's usually a single value so it serves nothing other than to build the matrix job name. |
| init | :white_check_mark: | |
| clone | :white_check_mark: | Tries first to clone to specified `clone_folder`, if any; otherwise it will use `C:\Projects\LocalAppVeyorTempClone`. From this step on all scripts will be executed as the clone folder being the working directory. |
| matrix | :large_blue_circle: | `fast_finish` is the only working option. Support for `allow_failures` to be added. |
| install | :white_check_mark: | |
| assembly_info | :red_circle: | |
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