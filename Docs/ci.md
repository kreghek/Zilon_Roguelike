https://medium.com/@neuecc/using-circle-ci-to-build-test-make-unitypackage-on-unity-9f9fa2b3adfd

# Как генерировать файл лицензии

docker run -it gableroux/unity3d:2018.3.11f1 bash
cd /opt/Unity/Editor
./Unity -quit -batchmode -nographics -logFile -createManualActivationFile
cat Unity_v2018.3.11f1.alf

На выходе будет файл лицензии

Его скармливаем сюда https://license.unity3d.com/manual

В статье сказано, что нужно быть залогированным на сайте Unity.

Потом создаём шифрованный файл

openssl aes-256-cbc -e -in ./Unity_v2018.x.ulf -out ./Unity_v2018.x.ulf-cipher -k ${CIPHER_KEY}