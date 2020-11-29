md "package/ProjectData~/Assets"
echo Starting template creation...

md "package/ProjectData~/Packages"
md "package/ProjectData~/ProjectSettings"

robocopy Assets package/ProjectData~/Assets /E
robocopy Packages package/ProjectData~/Packages /E
robocopy ProjectSettings package/ProjectData~/ProjectSettings /E

(
	echo {
	echo   "name": "%1",
	echo   "displayName": "%~2",
	echo   "unity": "2020.1",
	echo   "version": "0.0.1",
	echo   "description": "%~3",
	echo   "type": "template",
	echo   "host": "hub",
	echo   "dependencies": {}
	echo }
) > package/package.json

cd package/ProjectData~/ProjectSettings
del ProjectVersion.txt

cd ..
cd ..
cd ..

tar -czvf %1.tgz package/*.*

rd /s /q package