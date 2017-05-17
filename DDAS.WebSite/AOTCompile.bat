rem the file need to be deleted to enable Non AOT compilation:
copy app\main-aot.ts-forAOT app\main-aot.ts  
copy app\shared\utils\config.service.ts-forAOT app\shared\utils\config.service.ts
rem command runs in a separate window. type EXIT to close the new window to return to execute the next 
start /W   npm run build:aot 
copy aot\dist\*.*  ..\DDAS.API\dist
copy app\shared\utils\config.service.ts-forNonAOT app\shared\utils\config.service.ts 
copy aot\dist\build.js  ..\DDAS.API\dist\
copy aot\dist\build.js.map  ..\DDAS.API\dist\
start chrome "http://localhost:56846"

