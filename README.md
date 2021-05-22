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

This is rogue-like game about the randomly-generated vagabond, which trying to find the randomly-generated home in the randomly-generated world. Project at early stage of development. Development are slowly and with pleasure, but reguraly.

[Russian version of this file](/README_RU.md)

## The short game rules

You need to survive as long as you can. To survive, you need to collect resources. Resources can be found at the level or dropped from monsters.

Reasons why you will die:
- You will be attacked by monsters and get a mortal wound.
- You will die of hunger, because there will be nothing to eat.
- You will die of dehydration because you will not have drinking water.
- You will die from high intoxication because you are badly injured and you use too much medicine.
- You will catch a disease that will greatly weaken you, that you will not have the strength to fight off even a hungry rat.
- You write the command "dead" in the text version of the game.

[The Rules](https://last-imperial-vagabond.github.io/LAST_IMPERIAL_VAGABOND.github.io/)

## Join us

[Project group at VK](https://vk.com/last_imperial_vagabond)

[Logbook of the project (in russian)](https://lastimperialvagabond.home.blog)

GamePlay Video
[![Watch the GamePlay Video](https://img.youtube.com/vi/KJJ2ab35eFg/maxresdefault.jpg)](https://www.youtube.com/watch?v=KJJ2ab35eFg)

## How to build and launch

We strive to launch a project with one button. But now there are a few manual steps that must be performed before the game can be launched from the editor.

1. **Copy scheme catalog from `/Zilon.Client/Assets/Resources/Schemes` into `bin/Content`**.
The game required the scheme catalog in its `bin` directory. The easiest way to place the scheme catalog is executing `copy_scheme_catalog_to_game_bin.bat` from root of the repository.

2. *Optional*. **Set the `ZILON_LIV_SCHEME_CATALOG` environment variable to specify the full path to `\Zilon.Client\Assets\Resources\Schemes`**.
This is needed to run tests in Zilon.Core. The path must be complete, that is, it must begin, for example, with `C:\MyProjects\Zilon_Roguelike` for Windows and `/home/runner/work/Zilon_Roguelike/Zilon_Roguelike/Zilon.Client/Assets/Resources/Schemes` for Linux.

3. *Optional*. **Install the Test Generator NUnit extension for Visual Studio 2019**.
It will help to create NUnit tests from contextual menu in visual studio.

4. *Optional*. **Install the Specflow for Visual Studio 2019 extension**.
It is necessary for convenient editing of specifications, written in *Gherkin*. Unfortunately, we do not see a way to install this tool within the project yet. Easiest way to find it in `Visual Studio / Extensions / Manage Extensions`

5. *Optional*. **Install the ResX extension for Visual Studio 2019**.
The easies way to add or change localized string and other resources in the game. See https://github.com/dotnet/ResXResourceManager.

6. **Now you can launch the game from Visual Studio or `bin` directory after build**.
You are awesome!

## Contributing

We will be glad for any contribution to the development and support of the project. During development, we adhere to the [Code of Conduct](/CODE_OF_CONDUCT.md) and the [Source Code Conventions](/CODE_CONVENTIONS.md).

## License

- Released under [MIT](/LICENSE).
- Feel free to modify and reuse this project. You are required to include the license when using this code. Copy `LICENSE` to `LICENSE-source` and then modify `LICENSE` with your own name.
- Please link back to this repo as well.
