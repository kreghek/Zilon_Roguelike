#!/usr/bin/env bash

set -e

/opt/Unity/Editor/Unity \
  -quit \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile /dev/stdout \
  -manualLicenseFile .circleci/Unity_v2019.x.ulf

echo "Run succeeded, no failures occurred";

UNITY_EXIT_CODE=$?

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi