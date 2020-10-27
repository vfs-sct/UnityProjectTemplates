md "package/ProjectData~/Assets"
echo Starting template creation...
set /p tarballName=Enter tarball file name: 
set /p displayName=Enter template display name: 

md "package/ProjectData~/Packages"
md "package/ProjectData~/ProjectSettings"

robocopy Assets package/ProjectData~/Assets /E
robocopy Packages package/ProjectData~/Packages /E
robocopy ProjectSettings package/ProjectData~/ProjectSettings /E

(
	echo {
	echo   "name": "%tarballName%",
	echo   "displayName": "%displayName%",
	echo   "unity": "2020.1",
	echo   "version": "0.0.1",
	echo   "description": "A blank project with URP and recommended packages added.",
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

tar -czvf %tarballName%.tgz package/*.*

rd /s /q package