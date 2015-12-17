FSharp.Dnx
===

This project depends on [aspnet/dnx](github.com/aspnet/dnx)

| branch  | aspnet/dnx version  |
|---------|---------------------|
| master  | latest development version ( now 1.0.0-rc2-* ) |
| release | 1.0.0-rc1-final     |

# Usage

Examples app:

- command line, see `sample/HelloFSharp`
- asp.net mvc, see `sample/HelloMvc`

# Development

more info in `docs/development.md`

prepare the environment

```
dnvm upgrade -u
dnu restore
```

After that, you can:

- use the `FSharp.Dnx.sln` solution
- execute commands from a shell

NOTE: the `dnvm upgrade -u` install latest unstable version and change
the `default` alias ( it's a global config per user, used by ides like Visual Studio )
To revert to latest stable, do `dnvm upgrade`

## From shell

### run a sample command line app 

from `sample/HelloFSharp` directory

```
dnx run
```

expected

```
Hello from F#, running on DNX v4.5.1
```

### run a sample asp.net mvc app

from `sample/HelloMvc` directory

```
dnx web
```

expected

```
Hosting environment: Production
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

open a brower in [http://localhost:5000](http://localhost:5000)


### execute tests

from `test/FSharp.Dnx.Test` directory

```
dnx test
```
