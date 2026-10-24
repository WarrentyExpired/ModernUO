#!/usr/bin/env bash
if ( pgrep -f ModernUO.dll > /dev/null)
then
  echo "________________________________"
  echo "| A Server is already running!! "
  echo "--------------------------------"
else
  echo ""
  echo "________________________________"
  echo "| Starting Server...."
  echo "--------------------------------"
  dotnet ./Distribution/ModernUO.dll
fi
