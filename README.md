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

![Poster A3 horizontal](https://user-images.githubusercontent.com/2405499/58764985-41faf600-8598-11e9-9220-277923ca7f5b.png)

This is rogue-like game about the randomly-generated vagabond, which trying to find the randomly-generated home in the randomly-generated world. Project at early stage of development. Development are slowly and with pleasure, but reguraly.

[Russian version of this file](\README_RU.md)

## The short game rules

You need to survive as long as you can. To survive, you need to collect resources. Resources can be found at the level or dropped from monsters.

Reasons why you will die:
- You will be attacked by monsters and get a mortal wound.
- You will die of hunger, because there will be nothing to eat.
- You will die of dehydration because you will not have drinking water.
- You will die from high intoxication because you are badly injured and you use too much medicine.
- You will catch a disease that will greatly weaken you, that you will not have the strength to fight off even a hungry rat.
- You write the command "dead" in the text version of the game.

## Join us

[Project group at VK](https://vk.com/last_imperial_vagabond)

[Logbook of the project (in russian)](https://lastimperialvagabond.home.blog)

GamePlay Video
[![Watch the GamePlay Video](https://img.youtube.com/vi/KJJ2ab35eFg/maxresdefault.jpg)](https://www.youtube.com/watch?v=KJJ2ab35eFg)

## How to build and launch

We strive to launch a project with one button. But now there are a few manual steps that must be performed before the game can be launched from the editor.

1. **Clone the repository https://github.com/kreghek/Zilon_Roguelike_Plugins into Zilon.Client/Assets/ Plugins.**
[Zenject](https://github.com/modesttree/Zenject) and [SQLiter](https://assetstore.unity.com/packages/tools/integration/sqliter-20660) must be installed into the `Assets/Plugins` folder. The fastest way is to clone the https://github.com/kreghek/Zilon_Roguelike_Plugins repository into `Assets/Plugins`. The project started before there was a package manager in Unity. And I didn't want to store other assets' source code inside my own repository. Perhaps someday these dependencies will be loaded through the package manager.

2. **Run the publish_core_to_plugins.bat script.**
Into the same `Assets/Plugins` folder, you need to publish the core functionality developed in a separate *Zilon.Core* project. The easiest way is to run the `publish_core_to_plugins.bat` script in the root of the project.

3. *Optional*. **Set the `ZILON_LIV_SCHEME_CATALOG` environment variable to specify the full path to `\Zilon.Client\Assets\Resources\Schemes`**.
This is needed to run tests in Zilon.Core. The path must be complete, that is, it must begin, for example, with `C:\MyProjects\Zilon_Roguelike` for Windows and `/home/runner/work/Zilon_Roguelike/Zilon_Roguelike/Zilon.Client/Assets/Resources/Schemes` for Linux.

4. *Optional*. **Install the Specflow for Visual Studio 2019 extension**.
It is necessary for convenient editing of specifications, written in *Gherkin*. Unfortunately, we do not see a way to install this tool within the project yet. Easiest way to find it in `Visual Studio / Extensions / Manage Extensions`

5. **Open `Assets/Zilon/Scenes/globe-selection` scene**.
The game must start from this or from `title` scenes. Because to start playing you must generate and select a game globe first. By default empty scene will be selected.

6. **Now you can click `Play` button in Unity editor**.
You are awesome!

## Contributing

We will be glad for any contribution to the development and support of the project. During development, we adhere to the [Code of Conduct] (/ CODE_OF_CONDUCT.md) and the [Source Code Conventions] (/ CODE_CONVENTIONS.md).
