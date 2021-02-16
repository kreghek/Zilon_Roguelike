# LAST IMPERIAL VAGABOND
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/kreghek/Zilon_Roguelike)
[![CircleCI](https://circleci.com/gh/kreghek/Zilon_Roguelike/tree/master.svg?style=svg)](https://circleci.com/gh/kreghek/Zilon_Roguelike/tree/master)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/b8fa5561a70c401aa0e0a8be8d0ff696)](https://www.codacy.com/manual/kreghek/Zilon_Roguelike?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=kreghek/Zilon_Roguelike&amp;utm_campaign=Badge_Grade)
[![Codacy Badge](https://api.codacy.com/project/badge/Coverage/b8fa5561a70c401aa0e0a8be8d0ff696)](https://www.codacy.com/manual/kreghek/Zilon_Roguelike?utm_source=github.com&utm_medium=referral&utm_content=kreghek/Zilon_Roguelike&utm_campaign=Badge_Coverage)
[![CodeFactor](https://www.codefactor.io/repository/github/kreghek/zilon_roguelike/badge)](https://www.codefactor.io/repository/github/kreghek/zilon_roguelike)
[![Total alerts](https://img.shields.io/lgtm/alerts/g/kreghek/Zilon_Roguelike.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/kreghek/Zilon_Roguelike/alerts/)
[![BCH compliance](https://bettercodehub.com/edge/badge/kreghek/Zilon_Roguelike?branch=master)](https://bettercodehub.com/)
[![Maintainability](https://api.codeclimate.com/v1/badges/b4b300bf5efc3d73a268/maintainability)](https://codeclimate.com/github/kreghek/Zilon_Roguelike/maintainability)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=kreghek_Zilon_Roguelike&metric=alert_status)](https://sonarcloud.io/dashboard?id=kreghek_Zilon_Roguelike)
[![Coverage Status](https://coveralls.io/repos/github/kreghek/Zilon_Roguelike/badge.svg?branch=master)](https://coveralls.io/github/kreghek/Zilon_Roguelike?branch=master)
[![codecov](https://codecov.io/gh/kreghek/Zilon_Roguelike/branch/master/graph/badge.svg?token=mSMGtnXFOZ)](https://codecov.io/gh/kreghek/Zilon_Roguelike)
![GitHub branch checks state](https://img.shields.io/github/checks-status/kreghek/Zilon_Roguelike/master?label=Github%20Actions)

![Poster A3 horizontal](https://user-images.githubusercontent.com/2405499/58764985-41faf600-8598-11e9-9220-277923ca7f5b.png)

Это рогалик про случайно-сгенерированного парня-скитальца, который ищет случайно-сгенерированный дом в случайно-сгенерированном мире. Проект в ранней стадии разработки. Делаем не спешно и в удовольствие, но регулярно и методично.

## Краткие правила

Вам нужно выжить так долго, как можете. Чтобы выжить, вам необходимо собирать ресурсы. Ресурсы могут встречаться на уровне или выпадать из монстров.

Причины, по которым вы умрете:
- Вы будете атакованы монстрами и получите смертельную рану.
- Вы умрете от голода, потому что нечего будет есть.
- Вы умрёте от обезвоживания, потому что у вас не будет питьевой воды.
- Вы умретё от высокой интоксикации, потому что будете сильно ранены и используете слишком много медикаментов.
- Вы подхватите болезнь, которая сильно ослабит вас, что не будет сил отбиться даже от голодной крысы.
- Вы напишите команду "dead" в тесктовом вариенте игры.

## Присоединяйтесь к нам

[Группа проекта в VK](https://vk.com/last_imperial_vagabond)

[Бортовой журнал развития проекта (на русском)](https://lastimperialvagabond.home.blog)

GamePlay Video
[![Watch the GamePlay Video](https://img.youtube.com/vi/KJJ2ab35eFg/maxresdefault.jpg)](https://www.youtube.com/watch?v=KJJ2ab35eFg)

## Устранение неисправностей

Пожалуйста, не отчаивайтесь, если вам не удалось запустить игру или вы обнаружили баг.

1. **Убедитесь, что вы запускаете свежую версию игры.** Все релизы доступны в разделе https://github.com/kreghek/Zilon_Roguelike/releases. Мы стараемся исправить все обнаруженные проблемы каждый релиз. Возможно, ваша проблема уже была устранена.

2. **Сообщите о проблеме**, и мы как можно быстрее постараемся вам помочь. Вы можете:
- Написать о проблеме в обсуждении https://github.com/kreghek/Zilon_Roguelike/discussions/1031.
- Запостить в соц.сетях https://vk.com/last_imperial_vagabond.
- Отправить письмо с проблемой на почту kurkutovpa@gmail.com.

3. **Ознакомьтесь с логами проигрывателя Unity.** Возможно, в них содержится информация о решении вашей проблемы.

| Operating system | Log files |
|------------------------------|:--------------|
| Linux | ~/.config/unity3d/CompanyName/ProductName/Player.log |
| macOS | ~/Library/Logs/Company Name/Product Name/Player.log
| Windows| C:\Users\username\AppData\LocalLow\CompanyName\ProductName\Player.log |

Подробнее о логах написано в документации Unity https://docs.unity3d.com/Manual/LogFiles.html.

## Сборка и запуск в среде разработки

Мы стремимся к запуску проекта одной кнопкой. Чтобы как можно проще стартануть исследование игры. Но сейчас всё ещё требуются некоторые ручные настройки, прежде чем окружение будет готово.

1. **Склонировать репозиторий https://github.com/kreghek/Zilon_Roguelike_Plugins в папку Zilon.Client/Assets/ Plugins.**
Прежде чем клиент соберётся, нужно установить зависимости [Zenject](https://github.com/modesttree/Zenject) и [SQLiter](https://assetstore.unity.com/packages/tools/integration/sqliter-20660) в папку `Assets/Plugins`. Самый быстрый способ - это клонирование специального репозитория https://github.com/kreghek/Zilon_Roguelike_Plugins в `Assets/Plugins`. Возможно, можно установить эти ассеты через пакетный менеджер, но проект стартанул раньше, чем эту возможность можно было использовать в Unity. А хранить многочисленные исходные коды ассетов третьих лиц в этом репозитории казалось не самым удачным решением. Но всё же мы хотим в итоге подтягивать эти зависимости через пакетный менеджер.

2. **Запустить скрипт `publish_core_to_plugins.bat`.**
в ту же папку `Assets/Plugins` нужно опубликовать сборку с корневым функционалом, который разрабатывается в отдельном проекте*Zilon.Core*. самый простой способ - запустить скрипт `publish_core_to_plugins.bat` в корне проекта.

3. *Не обязательно*. **В переменной окружения `ZILON_LIV_SCHEME_CATALOG` указать полный путь до `\Zilon.Client\Assets\Resources\Schemes`**.
Это нужно для запуска тестов в *Zilon.Core*. Путь должен быть полным. Например, `C:\MyProjects\Zilon_Roguelike` для Windows и `/home/runner/work/Zilon_Roguelike/Zilon_Roguelike/Zilon.Client/Assets/Resources/Schemes` для Linux.

4. *Не обязательно*. **Установить расширение Specflow for Visual Studio 2019**.
Это нужно для удобства при разработке спецификаций, написанных на *Gherkin*. К сожалению, нет способа избежать ручной установки. Самый простой путь - это найти расширение в `Visual Studio / Extensions / Manage Extensions`

5. **Открыть сцену `Assets/Zilon/Scenes/globe-selection` в редакторе Unity**.
Игра должна начинаться с этой сцены или со сцены `title`. Потому что для основного геймплея нужен сгенерированный мир. По умолчанию в редакторе Unity при первом запуске будет выбрана пустая сцена.

6. Теперь в редакторе Unity вы можете нажать кнопку `Play` и играть.
Вы великолепны.

## Контрибьюция

Будем рады за любой вклад в разработку и поддержку проекта. Во время разработки мы придерживаемся [Правил поведения](/CODE_OF_CONDUCT.md) и [Соглашения об оформлении исходных кодов](/CODE_CONVENTIONS.md).
