# LocalAppVeyor
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

### - `build` command
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

### - `jobs` command
```
Usage: LocalAppVeyor jobs [options]

Options:
  -?|-h|--help  Show help information
  -d|--dir      Local repository directory where appveyor.yml sits. If not specified current directory is used
```

## Supported build steps
Due to LocalAppVeyor's nature only a subset of [AppVeyor build steps](https://www.appveyor.com/docs/build-configuration/#build-pipeline)
are supported. Some of them might get some support later in time, after consideration, but others most likely won't ever be part 
of the build pipeline. For example, things like deployment or packaging have little to do with a local build execution.

| Step        | Support           | Notes  |
| ------------- |:-------------:| ----- |
| init_env | :white_check_mark: | Internal step where all job environment variables are initialized. As for the [standard AppVeyor variables](https://www.appveyor.com/docs/environment-variables/) these are the ones supported: `APPVEYOR`, `CI`, `APPVEYOR_BUILD_FOLDER`, `PLATFORM` and `CONFIGURATION` |
| init | :white_check_mark: | |
| clone | :white_check_mark: | Tries first to clone to specified `clone_folder`, if any; otherwise it will use `C:\Projects\LocalAppVeyorTempClone`. From this step on all script wil be executed as the clone folder being the working directory. |
| before_build | :white_check_mark: | |
| build | :white_check_mark: | |
| build_script | :white_check_mark: | |
| after_build | :white_check_mark: | |
| before_test | :red_circle: | (_support to be added_) |
| test | :red_circle: | (_support to be added_) |
| test_script | :red_circle: | (_support to be added_) |
| after_test | :red_circle: | (_support to be added_) |
| on_success | :white_check_mark: | |
| on_failure | :white_check_mark: | |

## Final notes
- 
