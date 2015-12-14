
# prerequisites

Not needed for visual studio solution, because Visual Studio read the dnvm  version from the `sdk` property of `global.json` ( https://github.com/aspnet/dnvm/issues/271 )


open a shell inside this repository directory

``
dnvm use 1.0.0-rc2-16308
dnu restore
```

dnvm doesnt care about global.json, so use the `version of sdk inside global.json`


# Useful tips for dnvm/dnu/dnx

## enable trace info for debugging

set `DNX_TRACE` environment variable to `1`


for example

```
set DNX_TRACE=1
dnu build
```

should print lots of logging info


