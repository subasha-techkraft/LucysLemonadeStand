@ECHO off
ECHO Welcome to the Mom API Docker running script.
ECHO If this doesn't crash, at the end you'll be able to go to http://localhost:5062/swagger
ECHO You must have Docker Desktop installed, and probably running.

dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer

docker run -p 5062:80 mom:1.0.0