powershell -Command "(gc Build/index.html) -replace 'width: 960px; height: 600px;', 'width: 100%%; height: 100%%;' | Out-File -encoding utf8 Build/index.html"
powershell -Command "(gc Build/index.html) -replace 'margin: 0;', 'margin: 0; position: absolute; top: 0; left: 0; width: 100%%; height: 100%%; display: flex;' | Out-File -encoding utf8 Build/index.html"
powershell -Command "(gc Build/index.html) -replace '</head>', '<script src=""https://yandex.ru/games/sdk/v2""""></script><script>YaGames.init().then(ysdk => {console.log(""Yandex SDK initialized"""");window.ysdk = ysdk;ysdk.features.LoadingAPI?.ready();});</script></head>' | Out-File -encoding utf8 Build/index.html"
@REM powershell -Command "(gc Build/index_.html) -replace 'width=960', 'width=100%%' | Out-File -encoding utf8 Build/index.html"
@REM powershell -Command "(gc Build/index.html) -replace 'height=600', 'height=100%%' | Out-File -encoding utf8 Build/index.html"
@REM powershell -Command "(gc Build/index.html) -replace '960px', '100%%' | Out-File -encoding utf8 Build/index.html"
@REM powershell -Command "(gc Build/index.html) -replace '600px', '100%%' | Out-File -encoding utf8 Build/index.html"

