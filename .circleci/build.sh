BUILD_PATH=./ClientBuild

mkdir -p $BUILD_PATH

/opt/Unity/Editor/Unity \
  -projectPath "./Empty/New Unity Project" \
  -buildWindows64Player $BUILD_PATH/LAST.exe \
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

ls -la $BUILD_PATH
[ -n "$(ls -A $BUILD_PATH)" ] # fail job if build folder is empty
