FSharp.Dnx
===

This project is pinned to a specific version of [aspnet/dnx](github.com/aspnet/dnx) depending of branch


| branch |                                  |
|--------|----------------------------------|
| master | aligned with development version |
|        | of aspnet/dnx                    |

# Usage

Examples app:

- command line, see `sample/HelloFSharp`
- asp.net mvc, see `sample/HelloMvc`

# Development

more info in `docs/development.md`

Use the `FSharp.Dnx.sln` solution

or from a shell

prepare the environment

```
dnvm use 1.0.0-rc2-16308
dnu restore
```

## execute tests

from `test/FSharp.Dnx.Test` directory

```
dnu build
dnx test
```

## run a sample command line app 

from `sample/HelloFSharp` directory

``
dnu build
dnx run
```

expected

```
Hello from F#, running on DNX v4.5.1
```

## run a sample asp.net mvc app

from `sample/HelloMvc` directory

``
dnu build
dnx web
```

expected

```
Hosting environment: Production
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

open a brower in [http://localhost:5000](http://localhost:5000)

