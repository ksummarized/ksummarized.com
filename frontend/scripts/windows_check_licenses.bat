cd .. &&^
for /f "usebackq tokens=*" %%a in (`type allowed-licenses.txt`) do echo %%a &&^
for /f "usebackq tokens=*" %%b in (`type licenses-exclude-packages.txt`) do echo %%b &&^
license-checker --excludePackage %%b --summary --onlyAllow %%a
