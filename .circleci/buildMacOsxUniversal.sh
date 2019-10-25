#!/usr/bin/env bash

PROJECT_PATH=/root/project/Zilon.Client
BUILD_TARGET=StandaloneMacOsxUniversal
BUILD_NAME=LAST


set -e
set -x

mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/


echo "Building for $BUILD_TARGET"

export BUILD_PATH=$PROJECT_PATH/Builds/$BUILD_TARGET/
mkdir -p $BUILD_PATH

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
  -projectPath $PROJECT_PATH \
  -quit \
  -batchmode \
  -buildOSXUniversalPlayer ${BUILD_PATH}LAST.app \
  -logFile /dev/stdout \
  -stackTraceLogType Full

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

ls -la $BUILD_PATH
[ -n "$(ls -A $BUILD_PATH)" ] # fail job if build folder is empty
