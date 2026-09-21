#!/usr/bin/env bash
if ( pgrep -f ModernUO.dll > /dev/null)
then
  echo "________________________________"
  echo "| A Server is already running!! "
  echo "--------------------------------"
else
  echo "________________________________"
  echo "| Compiling server files."
  echo "--------------------------------"
  dotnet publish Projects/Application/Application.csproj -c Release -r linux-x64 --self-contained=false
  echo ""
  echo ""
  echo "________________________________"
  echo "| Starting Server...."
  echo "--------------------------------"
  dotnet ./Distribution/ModernUO.dll
fi
