build\paket.bootstrapper.exe 
IF EXIST paket.lock (build\paket.exe restore)
IF NOT EXIST paket.lock (build\paket.exe install)
"packages\build\FAKE\tools\Fake.exe" "build.fsx" "cmdline=%*"