rem copy c:..\bak\main-aot.ts app\main-aot.ts
copy "app\shared\utils\config.service.ts"  "app\shared\utils\config.service.ts-forNonAOT"
copy app\shared\utils\config.service.ts-forAOT app\shared\utils\config.service.ts
npm run build:aot 
copy aot\dist\*.*  ..\DDAS.API\dist
copy app\shared\utils\config.service.ts-forNonAOT app\shared\utils\config.service.ts 
copy app\shared\utils\config.service.ts app\shared\utils\config.service.ts-forAOT 
pause