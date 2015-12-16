The flow is

1. upgrade dnvm to latest unstable ( or install latest unstable and use that )
2. restore dependencies with `dnu restore`
3. run samples with `dnx run`/`dnx web` or tests with `dnx test`

NOTE:

Upgrading dnvm change the `default` alias and the dnvm version in `PATH`.

It's a global config per user, and `default` alias is used by ide like Visual Studio if sdk property 
in global.json file is not specified.

dnvm doesnt [care about global.json](https://github.com/aspnet/dnvm/issues/271)

## How to revert to latest stable version of dnvm

To revert to latest stable version of dnvm do

```
dnvm upgrade
```

### 1- Upgrade to latest unstable dnvm

Open a shell inside this repository directory

```
dnvm upgrade -u
```

### 2- Restore dependencies

You can use the command

```
dnu restore
```

# Useful tips for dnvm/dnu/dnx

## enable trace info for debugging

set `DNX_TRACE` environment variable to `1`


for example

```
set DNX_TRACE=1
dnu build
```

should print lots of logging info


