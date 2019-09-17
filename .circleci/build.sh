mkdir -p ./ClientBuild

/opt/Unity/Editor/Unity \
  -projectPath ./Zilon.Client \
  -buildWindows64Player ./ClientBuild/LAST.exe \
  -batchmode \
  -logFile /dev/stdout \
  -nographics \
  -quit
  
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
